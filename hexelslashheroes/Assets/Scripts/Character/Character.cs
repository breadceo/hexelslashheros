using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
	[SerializeField]
	protected TrackPad Controller;
	[SerializeField]
	protected float MoveSpeed = 7.5f;
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
		e.DebugEvent ();
		if (e.Kind == TrackPadEvent.EventKind.Swipe) {
			Move (e.Vector);
		}
	}

	void Move (Vector3 dir) {
		var quat = Mathf.PI * 0.25f;
		var cos = Mathf.Cos (quat);
		var sin = Mathf.Sin (quat);
		var x = cos * dir.x + sin * dir.y;
		var y = -sin * dir.x + cos * dir.y;
		Dir = new Vector3 (x, y, 0f).normalized;
		Debug.LogFormat ("{0} {1}", dir, Dir);
	}
}
