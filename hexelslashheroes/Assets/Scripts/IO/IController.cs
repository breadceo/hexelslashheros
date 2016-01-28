using UnityEngine;
using System.Collections;

public abstract class IController : MonoBehaviour {
	public abstract ControlEvent CreateTrackPadEventByDirection (Vector3 dir);
	internal delegate void ChangeControllerEvent (ControlEvent e);
	internal event ChangeControllerEvent OnChangeController;
	protected void SpawnEvent (ControlEvent e) {
		if (OnChangeController != null) {
			OnChangeController (e);
		}
	}
}

public struct ControlEvent {
	public Vector2 Vector { get; set; }

	public enum EventKind {
		Touch,
		Swipe
	}
	public EventKind Kind { get; set; }

	public void DebugEvent () {
		Debug.LogFormat ("down: ({0}, {1}) kind : {2}", Vector.x, Vector.y, Kind);
	}
}
