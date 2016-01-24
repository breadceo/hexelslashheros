using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
	[SerializeField] protected TrackPad Controller;
	[SerializeField] protected float MoveSpeed = 7.5f;
	protected Vector3 Dir = Vector3.zero;

	void OnEnable () {
		Controller.OnChangeTrackPadState += ChangeTrackPadState;
	}

	void OnDisable () {
		Controller.OnChangeTrackPadState -= ChangeTrackPadState;
	}

	void Update () {
		transform.position += Dir * MoveSpeed * Time.deltaTime;
	}

	protected void ChangeTrackPadState (TrackPadEvent e) {
		if (e.Kind == TrackPadEvent.EventKind.Swipe) {
			Move (e.Vector);
		}
	}

	void Move (Vector3 dir) {
		Dir = dir;
	}
}
