using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
	[SerializeField]
	protected TrackPad TargetTrackPad;
	[SerializeField]
	protected float MoveSpeed = 7.5f;
	protected Vector3 Dir = Vector3.zero;

	void OnEnable () {
		TargetTrackPad.OnChangeTrackPadState += ChangeTrackPadState;
	}

	void OnDisable () {
		TargetTrackPad.OnChangeTrackPadState -= ChangeTrackPadState;
	}

	void Update () {
		transform.position += Dir * MoveSpeed * Time.deltaTime;
	}

	protected void ChangeTrackPadState (TrackPadEvent e) {
		e.DebugEvent ();
		if (e.Kind == TrackPadEvent.EventKind.Axis) {
			Move (e.Dir);
		} else if (e.Kind == TrackPadEvent.EventKind.Swipe) {
			Slash (e.Dir);
		}
	}

	void Move (Vector3 dir) {
		Dir = dir;
	}

	void Slash (Vector3 dir) {
	}
}
