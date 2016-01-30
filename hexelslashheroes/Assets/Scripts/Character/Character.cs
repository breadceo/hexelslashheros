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
		Fall,
	}
	protected CharacterState currentState;
	protected System.Action currentStateAction;
	protected Dictionary <CharacterState, System.Action> stateStart = new Dictionary<CharacterState, System.Action> ();
	protected Dictionary <CharacterState, System.Action> stateMachine = new Dictionary<CharacterState, System.Action> ();
	protected Dictionary <CharacterState, System.Action> stateEnd = new Dictionary<CharacterState, System.Action> ();
	protected Dictionary <CharacterState, List<CharacterState>> transitable = new Dictionary<CharacterState, List<CharacterState>> ();
	protected Visual visual;
	[SerializeField] protected float rayOffsetY;
	protected Vector3 rayOrigin {
		get {
			return new Vector3 (transform.position.x, transform.position.y + rayOffsetY, characterCamera.transform.position.z);
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

	void Awake () {
		visual = transform.Find ("Visual").GetComponent <Visual> ();

		transitable.Add (CharacterState.Idle, new List<CharacterState> { CharacterState.Attack, CharacterState.Fall, CharacterState.Move });
		transitable.Add (CharacterState.Attack, new List<CharacterState> { CharacterState.Fall, CharacterState.Idle });
		transitable.Add (CharacterState.Move, new List<CharacterState> { CharacterState.Attack, CharacterState.Fall, CharacterState.Idle, CharacterState.Move });
		transitable.Add (CharacterState.Fall, new List<CharacterState> { });

		controllable = true;
		stateMachine.Add (CharacterState.Idle, () => {
			CheckDrop ();
		});
		stateMachine.Add (CharacterState.Move, () => {
			transform.position += dir * moveSpeed * Time.deltaTime;
			CheckDrop ();
		});
		stateMachine.Add (CharacterState.Attack, () => {
			float t = (Time.time - attackStartTime) / 0.2f;
			t = Mathf.Clamp01 (t);
			transform.position = Vector3.Lerp (attackStartPoint, attackEndPoint, t);
			if (t >= 1f) {
				transform.position = attackEndPoint;
				ChangeState (CharacterState.Idle);
			}
			CheckDrop ();
		});
		stateMachine.Add (CharacterState.Fall, () => {
			transform.position += gravityDir * gravitySpeed * Time.deltaTime;
			if (Time.time - fallStartTime > 2f) {
				Application.LoadLevel ("main");
			}
		});

		stateStart.Add (CharacterState.Attack, () => {
			visual.SetBlurs (true);
			visual.ForcePlayAnimation (controller.CreateTrackPadEventByDirection (dir));
			body.enabled = false;
			weapon.enabled = true;
		});
		stateStart.Add (CharacterState.Fall, () => {
			controllable = false;
			visual.StopAnimation ();
			fallStartTime = Time.time;
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
			body.enabled = true;
			weapon.enabled = false;
		});
		stateEnd.Add (CharacterState.Fall, () => {
			controllable = true;
		});
		ChangeState (CharacterState.Idle);
	}

	void OnEnable () {
		RenderOrderManager.GetInstance.Register (this);
		controller.OnChangeController += ChangeTrackPadState;
	}

	void OnDisable () {
		RenderOrderManager.GetInstance.UnRegister (this);
		controller.OnChangeController -= ChangeTrackPadState;
	}

	protected void Update () {
		if (currentStateAction != null)
			currentStateAction ();
	}

	const int BackgroundLayerMask = 1 << 8;
	protected void CheckDrop () {
		RaycastHit hitInfo;
		if (Physics.Raycast (rayOrigin, Vector3.forward, out hitInfo, 100f, BackgroundLayerMask)) {
			if (hitInfo.collider.CompareTag ("Outside")) {
				var outSide = hitInfo.collider.GetComponent <Outside> ();
				if (outSide != null) {
					outSide.enabled = true;
					ChangeState (CharacterState.Fall);
				}
			}
		} else {
			ChangeState (CharacterState.Fall);
		}
	}

	protected void ChangeTrackPadState (ControlEvent e) {
		if (controllable == false) {
			return;
		}
		if (e.Kind == ControlEvent.EventKind.Swipe) {
			ChangeState (CharacterState.Move, () => {
				dir = e.Vector;
			});
		} else if (e.Kind == ControlEvent.EventKind.Touch) {
			ChangeState (CharacterState.Attack, () => {
				attackStartTime = Time.time;
				attackStartPoint = transform.position;
				var worldPosition = characterCamera.ScreenToWorldPoint (e.Vector);
				var clickPoint = new Vector3 (worldPosition.x, worldPosition.y, transform.position.z);
				dir = (clickPoint - attackStartPoint).normalized;
				attackEndPoint = attackStartPoint + dir * attackRange;
			});
		}
	}

	protected void ChangeState (CharacterState state, System.Action initFunc = null) {
		List<CharacterState> can;
		if (transitable.TryGetValue (currentState, out can) == false) {
			throw new System.InvalidProgramException ("cannot find transitable list");
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
		}
	}

	public int MakeOrder (int start) {
		if (visual.anim.enabled) {
			var info = visual.anim.GetCurrentAnimatorStateInfo (0);
			if (info.shortNameHash == visual.UpRightAnimationHash || info.shortNameHash == visual.DownRightAnimationHash) {
				visual.bodySpr.sortingOrder = start + 1;
				visual.tailSpr.sortingOrder = start + 2;
				visual.weaponSpr.sortingOrder = start;
				return start + 3;
			} else {
				visual.bodySpr.sortingOrder = start;
				visual.weaponSpr.sortingOrder = start + 1;
				visual.tailSpr.sortingOrder = start + 2;
				return start + 3;
			}
		} else {
			return start + 3;
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
		}
	}
}
