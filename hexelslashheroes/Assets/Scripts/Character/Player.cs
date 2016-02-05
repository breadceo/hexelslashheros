using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	[SerializeField] protected IController controller;
	[SerializeField] protected float moveSpeed = 7.5f;
	protected Vector3 dir = Vector3.zero;
	public Vector3 Dir {
		get {
			return dir;
		}
	}
	[SerializeField] protected float attackDuration = 0.1f;
	protected Vector3 attackStartPoint = Vector3.zero;
	protected Vector3 attackEndPoint = Vector3.zero;
	protected float attackStartTime;
	protected enum CharacterState
	{
		Spawn,
		Idle,
		Move,
		Attack,
		Stuck,
		Dead
	}
	protected CharacterState currentState;
	protected System.Action currentStateAction;
	protected CharacterState? nextState;
	protected System.Action nextStateInitAction;
	protected Dictionary <CharacterState, System.Action> stateStart = new Dictionary<CharacterState, System.Action> ();
	protected Dictionary <CharacterState, System.Action> stateMachine = new Dictionary<CharacterState, System.Action> ();
	protected Dictionary <CharacterState, System.Action> stateEnd = new Dictionary<CharacterState, System.Action> ();
	protected Dictionary <CharacterState, List<CharacterState>> transitable = new Dictionary<CharacterState, List<CharacterState>> ();
	protected Visual visual;
	protected bool controllable {
		get;
		set;
	}
	[SerializeField] protected BoxCollider body;
	[SerializeField] protected BoxCollider weapon;
	[SerializeField] protected TrailController trailController;
	[SerializeField] protected BlurController blurController;
	[SerializeField] protected GameObject arrowPrefab;

	void Awake () {
		visual = transform.Find ("Visual").GetComponent <Visual> ();

		transitable.Add (CharacterState.Spawn, new List<CharacterState> { CharacterState.Idle });
		transitable.Add (CharacterState.Idle, new List<CharacterState> { CharacterState.Attack, CharacterState.Dead, CharacterState.Move, CharacterState.Stuck });
		transitable.Add (CharacterState.Attack, new List<CharacterState> { CharacterState.Attack, CharacterState.Dead, CharacterState.Idle, CharacterState.Stuck });
		transitable.Add (CharacterState.Move, new List<CharacterState> { CharacterState.Attack, CharacterState.Dead, CharacterState.Idle, CharacterState.Move, CharacterState.Stuck });
		transitable.Add (CharacterState.Stuck, new List<CharacterState> { CharacterState.Attack, CharacterState.Dead, CharacterState.Idle, CharacterState.Move });
		transitable.Add (CharacterState.Dead, new List<CharacterState> { });

		controllable = true;
		stateMachine.Add (CharacterState.Idle, () => {
			CheckBlock ();
		});

		stateMachine.Add (CharacterState.Move, () => {
			transform.position += dir * moveSpeed * Time.deltaTime;
			CheckBlock ();
		});
		stateEnd.Add (CharacterState.Move, () => {
			dir = Vector3.zero;
		});

		stateStart.Add (CharacterState.Attack, () => {
			blurController.SetBlurs (true);
			weapon.enabled = true;
			trailController.gameObject.SetActive (true);
		});
		stateMachine.Add (CharacterState.Attack, () => {
			float t = (Time.time - attackStartTime) / attackDuration;
			t = Mathf.Clamp01 (t);
			if (t >= 1f) {
				transform.position = attackEndPoint;
				RequestChangeState (CharacterState.Idle);
				return;
			}
			t *= 2f;
			var diff = attackEndPoint - attackStartPoint;
			if (t < 1) {
				transform.position = diff * 0.5f * t * t + attackStartPoint;
			} else {
				t--;
				transform.position = -diff * 0.5f * (t * (t - 2f) - 1f) + attackStartPoint;
			}
			CheckBlock ();
		});
		stateEnd.Add (CharacterState.Attack, () => {
			blurController.SetBlurs (false);
			visual.StopAnimation ();
			dir = Vector3.zero;
			attackStartPoint = Vector3.zero;
			attackEndPoint = Vector3.zero;
			weapon.enabled = false;
			trailController.gameObject.SetActive (false);
		});

		stateStart.Add (CharacterState.Dead, () => {
			controllable = false;
		});
	}

	void Start () {
		Init ();
	}

	public void Init () {
		controllable = true;
		trailController.gameObject.SetActive (false);
		ChangeState (CharacterState.Idle, () => {
			visual.StopAnimation ();
		});
	}

	void OnEnable () {
		controller.OnChangeController += ChangeTrackPadState;
		GameManager.GetInstance.OnObjectSpawn += OnObjectSpawn;
	}

	void OnDisable () {
		controller.OnChangeController -= ChangeTrackPadState;
		GameManager.GetInstance.OnObjectSpawn -= OnObjectSpawn;
	}

	void Update () {
		if (nextState.HasValue) {
			ChangeState (nextState.Value, nextStateInitAction);
		}
		if (currentStateAction != null)
			currentStateAction ();
	}

	protected void CheckBlock () {
		if (StageManager.GetInstance.currentStage.CheckBlocked (gameObject)) {
			RequestChangeState (CharacterState.Stuck);
		}
	}

	protected void ChangeTrackPadState (ControlEvent e) {
		if (controllable == false) {
			return;
		}
		if (e.Kind == ControlEvent.EventKind.Swipe) {
			RequestChangeState (CharacterState.Move, () => {
				dir = e.Vector;
			});
		} else if (e.Kind == ControlEvent.EventKind.Touch) {
			RequestChangeState (CharacterState.Attack, () => {
				attackStartTime = Time.time;
				attackStartPoint = transform.position;
				var worldPosition = GameManager.GetInstance.playerCamera.ScreenToWorldPoint (e.Vector);
				var clickPoint = new Vector3 (worldPosition.x, worldPosition.y, transform.position.z);
				dir = (clickPoint - attackStartPoint).normalized;
				attackEndPoint = clickPoint;
				visual.ForcePlayAnimation (controller.CreateTrackPadEventByDirection (dir));
			});
		}
	}

	protected void ChangeState (CharacterState state, System.Action initFunc) {
		List<CharacterState> can;
		if (transitable.TryGetValue (currentState, out can) == false) {
			throw new UnityException ("cannot find transitable list");
		}
		if (can.FindIndex (s => s == state) != -1) {
			if (currentState != state) {
				System.Action stateEndFunc;
				if (stateEnd.TryGetValue (currentState, out stateEndFunc) == false) {
					stateEndFunc = null;
				}
				if (stateEndFunc != null) {
					stateEndFunc ();
				}
				if (initFunc != null)
					initFunc ();
				System.Action stateStartFunc;
				if (stateStart.TryGetValue (state, out stateStartFunc) == false) {
					stateStartFunc = null;
				}
				if (stateStartFunc != null) {
					stateStartFunc ();
				}
				if (stateMachine.TryGetValue (state, out currentStateAction) == false) {
					currentStateAction = null;
				}
			} else {
				if (initFunc != null)
					initFunc ();
			}
			currentState = state;
			nextState = null;
		}
	}

	protected void RequestChangeState (CharacterState state, System.Action initFunc = null) {
		nextState = state;
		nextStateInitAction = initFunc;
	}

	void OnTriggerEnter (Collider other) {
		if (currentState != CharacterState.Dead) {
			if (other.gameObject.CompareTag ("EnemyWeapon")) {
				GameManager.GetInstance.InvokeHitEvent (other.gameObject, gameObject);
				GameManager.GetInstance.InvokeDeadEvent (gameObject);
				GameManager.GetInstance.Destroy (gameObject);
			} else if (other.gameObject.CompareTag ("Door")) {
				GameManager.GetInstance.InvokeNextStageEvent ();
			}
		}
	}

	void OnObjectSpawn (GameObject obj) {
		var ai = obj.GetComponent <AI> ();
		if (ai != null) {
			var arrow = GameManager.GetInstance.Instantiate (arrowPrefab);
			arrow.transform.SetParent (arrowPrefab.transform.parent);
			arrow.transform.localPosition = Vector3.zero;
			var enemyGuide = arrow.GetComponent <EnemyGuide> ();
			enemyGuide.Trace (gameObject, obj);
			arrow.SetActive (true);
		}
	}
}
