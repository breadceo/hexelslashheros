using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TrackPad : MonoBehaviour, IPointerDownHandler, IPointerClickHandler {
	protected Vector2 PointerDownPos;
	protected TrackPadEvent Tpe = new TrackPadEvent ();
//	protected const Vector2 Up =

	public void OnPointerDown (PointerEventData eventData) {
		PointerDownPos = eventData.position;
	}

	public void OnPointerClick (PointerEventData eventData) {
		var diff = eventData.position - PointerDownPos;
		if (diff.magnitude > 0) {
			Tpe.Kind = TrackPadEvent.EventKind.Swipe;
			diff = diff.normalized;
			float up = Vector2.Angle (Vector2.up, diff);
			float down = Vector2.Angle (Vector2.down, diff);
			float right = Vector2.Angle (Vector2.right, diff);
			float left = Vector2.Angle (Vector2.left, diff);
			float choose = Mathf.Min (new float [] { up, down, left, right });
			if (choose == up) {
				Tpe.Vector = Vector2.up;
			} else if (choose == down) {
				Tpe.Vector = Vector2.down;
			} else if (choose == left) {
				Tpe.Vector = Vector2.left;
			} else {
				Tpe.Vector = Vector2.right;
			}
		} else {
			Tpe.Kind = TrackPadEvent.EventKind.Touch;
			Tpe.Vector = eventData.position;
		}
		if (OnChangeTrackPadState != null) {
			OnChangeTrackPadState (Tpe);
		}
	}

	void Update () {
		bool dispatchEvent = false;
		if (Input.GetKeyUp (KeyCode.LeftArrow)) {
			Tpe.Kind = TrackPadEvent.EventKind.Swipe;
			Tpe.Vector = Vector2.left;
			dispatchEvent = true;
		} else if (Input.GetKeyUp (KeyCode.RightArrow)) {
			Tpe.Kind = TrackPadEvent.EventKind.Swipe;
			Tpe.Vector = Vector2.right;
			dispatchEvent = true;
		} else if (Input.GetKeyUp (KeyCode.UpArrow)) {
			Tpe.Kind = TrackPadEvent.EventKind.Swipe;
			Tpe.Vector = Vector2.up;
			dispatchEvent = true;
		} else if (Input.GetKeyUp (KeyCode.DownArrow)) {
			Tpe.Kind = TrackPadEvent.EventKind.Swipe;
			Tpe.Vector = Vector2.down;
			dispatchEvent = true;
		}
		if (dispatchEvent) {
			if (OnChangeTrackPadState != null) {
				OnChangeTrackPadState (Tpe);
			}
		}
	}

	internal delegate void ChangeTrackPadState (TrackPadEvent e);
	internal event ChangeTrackPadState OnChangeTrackPadState;
}

public struct TrackPadEvent {
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

