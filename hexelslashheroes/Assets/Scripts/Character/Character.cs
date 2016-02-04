using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour, RenderObject {
	[SerializeField] protected IController controller;
	[SerializeField] protected float moveSpeed = 7.5f;
	[SerializeField] protected Camera characterCamera;
	protected Vector3 dir = Vector3.zero;
	public Vector3 Dir {
		get {
			return dir;
		}
	}
	protected Coroutine attackCoroutine;
	[SerializeField] protected float attackDuration = 0.1f;
	[SerializeField] protected float attackRange = 10f;
	protected Vector3 attackStartPoint = Vector3.zero;
	protected Vector3 attackEndPoint = Vector3.zero;
	protected float attackStartTime;
	protected enum CharacterState
	{
		Idle,
		Move,
		Attack,
		Stuck,
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
	protected Vector3 rayOrigin {
		get {
			return new Vector3 (transform.position.x, transform.position.y, characterCamera.transform.position.z);
		}
	}
	protected bool controllable {
		get;
		set;
	}
	[SerializeField] protected float gravitySpeed = 20f;
	protected Vector3 gravityDir = Vector3.down;
	protected float fallStartTime;
	[SerializeField] protected BoxCollider body;
	[SerializeField] protected BoxCollider weapon;
	[SerializeField] protected TrailController trailController;

	void Awake () {
		visual = transform.Find ("Visual").GetComponent <Visual> ();

		transitable.Add (CharacterState.Idle, new List<CharacterState> { CharacterState.Attack, CharacterState.Move, CharacterState.Stuck });
		transitable.Add (CharacterState.Attack, new List<CharacterState> { CharacterState.Attack, CharacterState.Idle, CharacterState.Stuck });
		transitable.Add (CharacterState.Move, new List<CharacterState> { CharacterState.Attack, CharacterState.Idle, CharacterState.Move, CharacterState.Stuck });
		transitable.Add (CharacterState.Stuck, new List<CharacterState> { CharacterState.Attack, CharacterState.Idle, CharacterState.Move });

		controllable = true;
		stateMachine.Add (CharacterState.Idle, () => {
			CheckBlock ();
		});
		stateMachine.Add (CharacterState.Move, () => {
			transform.position += dir * moveSpeed * Time.deltaTime;
			CheckBlock ();
		});
		stateMachine.Add (CharacterState.Attack, () => {
			float t = (Time.time - attackStartTime) / 0.2f;
			t = Mathf.Clamp01 (t);
			transform.position = Vector3.Lerp (attackStartPoint, attackEndPoint, t);
			if (t >= 1f) {
				transform.position = attackEndPoint;
				RequestChangeState (CharacterState.Idle);
			}
			CheckBlock ();
		});
		stateMachine.Add (CharacterState.Stuck, () => {
		});

		stateStart.Add (CharacterState.Attack, () => {
			visual.SetBlurs (true);
			weapon.enabled = true;
			trailController.gameObject.SetActive (true);
		});
		stateStart.Add (CharacterState.Stuck, () => {
		});

		stateEnd.Add (CharacterState.Move, () => {
			dir = Vector3.zero;
		});
		stateEnd.Add (CharacterState.Attack, () => {
			visual.SetBlurs (false);
			visual.StopAnimation ();
			dir = Vector3.zero;
			attackStartPoint = Vector3.zero;
			attackEndPoint = Vector3.zero;
			weapon.enabled = false;
			trailController.gameObject.SetActive (false);
		});

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
		RenderOrderManager.GetInstance.Register (this);
		controller.OnChangeController += ChangeTrackPadState;
	}

	void OnDisable () {
		RenderOrderManager.GetInstance.UnRegister (this);
		controller.OnChangeController -= ChangeTrackPadState;
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

//	protected void OnDrawGizmos () {
//		Gizmos.color = Color.yellow;
//		Gizmos.DrawCube (map.center, map.bounds.size);
//	}

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
				var worldPosition = characterCamera.ScreenToWorldPoint (e.Vector);
				var clickPoint = new Vector3 (worldPosition.x, worldPosition.y, transform.position.z);
				dir = (clickPoint - attackStartPoint).normalized;
				attackEndPoint = attackStartPoint + dir * attackRange;
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

	public int MakeOrder (int start) {
		if (visual.anim.enabled) {
			var info = visual.anim.GetCurrentAnimatorStateInfo (0);
			if (info.shortNameHash == visual.UpRightAnimationHash || info.shortNameHash == visual.DownRightAnimationHash) {
				visual.bodySpr.sortingOrder = start + 1;
				visual.tailSpr.sortingOrder = start + 2;
				visual.weaponSpr.sortingOrder = start;
				trailController.SetSortingOrder (start + 3);
				return start + 4;
			} else {
				visual.bodySpr.sortingOrder = start;
				visual.weaponSpr.sortingOrder = start + 1;
				visual.tailSpr.sortingOrder = start + 2;
				trailController.SetSortingOrder (start + 3);
				return start + 4;
			}
		} else {
			return start + 4;
		}
	}

	public GameObject Target {
		get {
			return gameObject;
		}
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag ("EnemyWeapon")) {
			GameManager.GetInstance.InvokeHitEvent (other.gameObject, gameObject);
		} else if (other.gameObject.CompareTag ("Door")) {
			GameManager.GetInstance.InvokeNextStageEvent ();
		}
	}
}
