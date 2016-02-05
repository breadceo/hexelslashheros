using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AI : IController {
	[SerializeField] protected BoxCollider body;
	[SerializeField] protected BoxCollider weapon;
	[SerializeField] protected float sight;
	protected Visual visual;
	protected enum AiState
	{
		Spawn,
		Idle,
		Roaming,
		Alarmed,
		Attack,
		Stuck,
	}
	protected AiState currentState;
	protected System.Action currentStateAction;
	protected AiState? nextState;
	protected System.Action nextStateInitAction;
	protected Dictionary <AiState, System.Action> stateStart = new Dictionary<AiState, System.Action> ();
	protected Dictionary <AiState, System.Action> stateMachine = new Dictionary<AiState, System.Action> ();
	protected Dictionary <AiState, System.Action> stateEnd = new Dictionary<AiState, System.Action> ();
	protected Dictionary <AiState, List<AiState>> transitable = new Dictionary<AiState, List<AiState>> ();
	[SerializeField] protected float roamingDuration = 0.5f;
	[SerializeField] protected float roamingDistance = 1f;
	protected float roamingStartTime;
	protected Vector3 roamingStartPoint = Vector3.zero;
	protected Vector3 roamingEndPoint = Vector3.zero;
	protected Vector2 roamingDir = Vector2.zero;
	[SerializeField] protected SightDrawer sightDrawer;

	void Awake () {
		visual = transform.Find ("Visual").GetComponent <Visual> ();

		transitable.Add (AiState.Spawn, new List<AiState> { AiState.Idle });
		transitable.Add (AiState.Idle, new List<AiState> { AiState.Alarmed, AiState.Roaming });
		transitable.Add (AiState.Roaming, new List<AiState> { AiState.Idle, AiState.Alarmed, AiState.Stuck });
		transitable.Add (AiState.Alarmed, new List<AiState> { AiState.Attack, AiState.Stuck });
		transitable.Add (AiState.Attack, new List<AiState> { AiState.Idle, AiState.Stuck });
		transitable.Add (AiState.Stuck, new List<AiState> { AiState.Idle });

		stateStart.Add (AiState.Idle, () => {
			roamingStartTime = Time.time + 1f;
		});
		stateMachine.Add (AiState.Idle, () => {
			if (Time.time > roamingStartTime) {
				RequestChangeState (AiState.Roaming, () => {
					roamingDir = TrackPad.RandomDirection;
					roamingStartPoint = transform.position;
				});
			}
			CheckBlock ();
			CheckPlayerInSight ();
		});

		stateStart.Add (AiState.Roaming, () => {
			var dir = new Vector3 (roamingDir.x, roamingDir.y, 0f);
			roamingEndPoint = roamingStartPoint + roamingDistance * dir;
			var e = CreateTrackPadEventByDirection (roamingDir);
			visual.ForcePlayAnimation (e);
		});
		stateMachine.Add (AiState.Roaming, () => {
			float t = (Time.time - roamingStartTime) / roamingDuration;
			t = Mathf.Clamp01 (t);
			transform.position = Vector3.Lerp (roamingStartPoint, roamingEndPoint, t);
			if (t >= 1f) {
				transform.position = roamingEndPoint;
				RequestChangeState (AiState.Idle);
			}
			CheckBlock ();
			CheckPlayerInSight ();
		});
		stateEnd.Add (AiState.Roaming, () => {
			visual.StopAnimation ();
		});

		sightDrawer.SetRadius (sight);
	}

	void OnEnable () {
		GameManager.GetInstance.OnHitOccurs += OnHitOccurs;
	}

	void OnDisable () {
		if (GameManager.GetInstance != null) {
			GameManager.GetInstance.OnHitOccurs -= OnHitOccurs;
		}
	}

	void Update () {
		if (nextState.HasValue) {
			ChangeState (nextState.Value, nextStateInitAction);
		}
		if (currentStateAction != null)
			currentStateAction ();
	}

	public override ControlEvent CreateTrackPadEventByDirection (Vector3 dir) {
		return new ControlEvent {
			Vector = new Vector2 (dir.x, dir.y),
			Kind = ControlEvent.EventKind.Swipe
		};
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag ("PlayerWeapon")) {
			GameManager.GetInstance.InvokeHitEvent (other.gameObject, gameObject);
		}
	}

	void OnHitOccurs (GameObject attacker, GameObject defender) {
		var ai = defender.GetComponentInParent <AI> ();
		if (ai == this) {
			var ec = attacker.GetComponentInParent <EffectContainer> ();
			if (ec != null) {
				var particle = GameObject.Instantiate (ec.hit) as GameObject;
				particle.SetActive (true);
				particle.transform.position = transform.position;
				var system = particle.GetComponent <ParticleSystem> ();
				system.Stop ();
				system.Play ();
			}
			GameManager.GetInstance.Destroy (gameObject);
			GameManager.GetInstance.InvokeDeadEvent (gameObject);
		}
	}

	public void Spawn (Vector3 spawnPosition) {
		transform.position = spawnPosition;
		gameObject.SetActive (true);
		ChangeState (AiState.Idle, () => {
			var e = CreateTrackPadEventByDirection (TrackPad.RandomDirection);
			visual.ForcePlayAnimation (e);
			visual.StopAnimation ();
		});
	}

	protected void RequestChangeState (AiState state, System.Action initFunc = null) {
		nextState = state;
		nextStateInitAction = initFunc;
	}
	
	protected void ChangeState (AiState state, System.Action initFunc) {
		List<AiState> can;
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

	protected void CheckBlock () {
		if (StageManager.GetInstance.currentStage.CheckBlocked (gameObject)) {
			RequestChangeState (AiState.Stuck);
		}
	}

	protected void CheckPlayerInSight () {
		var distance = (GameManager.GetInstance.player.transform.position - transform.position).magnitude;
		if (distance < sight) {
			RequestChangeState (AiState.Alarmed);
		}
	}
}
