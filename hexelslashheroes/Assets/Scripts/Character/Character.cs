﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour {
	[SerializeField] protected TrackPad controller;
	[SerializeField] protected float moveSpeed = 7.5f;
	[SerializeField] protected Camera characterCamera;
	protected Vector3 dir = Vector3.zero;
	protected Coroutine attackCoroutine;
	[SerializeField] protected float attackDashSpeed = 30f;
	protected Vector3 attackStartPoint = Vector3.zero;
	protected Vector3 attackEndPoint = Vector3.zero;
	protected Vector3 attackMoveVector = Vector3.zero;
	protected enum CharacterState
	{
		Idle,
		Move,
		Attack,
		Stiff,
	}
	protected CharacterState currentState;
	protected System.Action currentStateAction;
	protected Dictionary <CharacterState, System.Action> stateStart = new Dictionary<CharacterState, System.Action> ();
	protected Dictionary <CharacterState, System.Action> stateMachine = new Dictionary<CharacterState, System.Action> ();
	protected Dictionary <CharacterState, System.Action> stateEnd = new Dictionary<CharacterState, System.Action> ();
	protected Visual visual;

	void Awake () {
		visual = transform.Find ("Visual").GetComponent <Visual> ();

		stateMachine.Add (CharacterState.Move, () => {
			transform.position += dir * moveSpeed * Time.deltaTime;
		});
		stateMachine.Add (CharacterState.Attack, () => {
			transform.position += dir * attackDashSpeed * Time.deltaTime;
			var t = (transform.position - attackStartPoint).magnitude / attackMoveVector.magnitude;
			t = Mathf.Clamp01 (t);
			if (t >= 1f) {
				transform.position = attackEndPoint;
				ChangeState (CharacterState.Idle);
			}
		});
		//		stateMachine.Add (CharacterState.Stiff, () => {
		//		});

		stateStart.Add (CharacterState.Attack, () => {
			visual.SetBlurs (true);
			visual.ForcePlayAnimation (controller.CreateTrackPadEventByDirection (attackMoveVector.normalized));
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
			attackMoveVector = Vector3.zero;
		});
		ChangeState (CharacterState.Idle);
	}

	void OnEnable () {
		controller.OnChangeTrackPadState += ChangeTrackPadState;
	}

	void OnDisable () {
		controller.OnChangeTrackPadState -= ChangeTrackPadState;
	}

	protected void Update () {
		if (currentStateAction != null)
			currentStateAction ();
	}

	protected void ChangeTrackPadState (TrackPadEvent e) {
		if (e.Kind == TrackPadEvent.EventKind.Swipe) {
			ChangeState (CharacterState.Move, () => {
				dir = e.Vector;
			});
		} else if (e.Kind == TrackPadEvent.EventKind.Touch) {
			ChangeState (CharacterState.Attack, () => {
				attackStartPoint = transform.position;
				var worldPosition = characterCamera.ScreenToWorldPoint (e.Vector);
				attackEndPoint = new Vector3 (worldPosition.x, worldPosition.y, 0f);
				attackMoveVector = attackEndPoint - attackStartPoint;
				dir = attackMoveVector.normalized;
			});
		}
	}

	protected void ChangeState (CharacterState state, System.Action initFunc = null) {
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
