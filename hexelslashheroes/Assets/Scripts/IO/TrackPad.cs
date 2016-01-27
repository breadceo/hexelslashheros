using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TrackPad : MonoBehaviour, IPointerDownHandler, IPointerClickHandler {
	protected Vector2 PointerDownPos;
	protected TrackPadEvent Tpe = new TrackPadEvent ();
	[SerializeField] float TrackPadSensibility = 50f;

	public void OnPointerDown (PointerEventData eventData) {
		PointerDownPos = eventData.position;
	}

	public void OnPointerClick (PointerEventData eventData) {
		var diff = eventData.position - PointerDownPos;
		if (diff.magnitude > TrackPadSensibility) {
			diff = diff.normalized;
			Tpe = CreateTrackPadEventByDirection (diff);
		} else {
			Tpe.Kind = TrackPadEvent.EventKind.Touch;
			Tpe.Vector = eventData.position;
		}
		if (OnChangeTrackPadState != null) {
			OnChangeTrackPadState (Tpe);
		}
	}

	public TrackPadEvent CreateTrackPadEventByDirection (Vector3 dir) {
		TrackPadEvent ret = new TrackPadEvent ();
		ret.Kind = TrackPadEvent.EventKind.Swipe;
		float ur = Vector2.Angle (UpRight, dir);
		float ul = Vector2.Angle (UpLeft, dir);
		float dr = Vector2.Angle (DownRight, dir);
		float dl = Vector2.Angle (DownLeft, dir);
		float choose = Mathf.Min (new float [] { ur, ul, dr, dl });
		if (choose == ur) {
			ret.Vector = UpRight;
		} else if (choose == ul) {
			ret.Vector = UpLeft;
		} else if (choose == dr) {
			ret.Vector = DownRight;
		} else {
			ret.Vector = DownLeft;
		}
		return ret;
	}

	void Update () {
		bool dispatchEvent = false;
		if (Input.GetKey (KeyCode.W) && Input.GetKey (KeyCode.A)) {
			Tpe.Kind = TrackPadEvent.EventKind.Swipe;
			Tpe.Vector = UpLeft;
			dispatchEvent = true;
		} else if (Input.GetKey (KeyCode.W) && Input.GetKey (KeyCode.D)) {
			Tpe.Kind = TrackPadEvent.EventKind.Swipe;
			Tpe.Vector = UpRight;
			dispatchEvent = true;
		} else if (Input.GetKey (KeyCode.S) && Input.GetKey (KeyCode.A)) {
			Tpe.Kind = TrackPadEvent.EventKind.Swipe;
			Tpe.Vector = DownLeft;
			dispatchEvent = true;
		} else if (Input.GetKey (KeyCode.S) && Input.GetKey (KeyCode.D)) {
			Tpe.Kind = TrackPadEvent.EventKind.Swipe;
			Tpe.Vector = DownRight;
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

	internal static Vector2 UpRight = new Vector2 (0.7f, 0.7f);
	internal static Vector2 UpLeft = new Vector2 (-0.7f, 0.7f);
	internal static Vector2 DownRight = new Vector2 (0.7f, -0.7f);
	internal static Vector2 DownLeft = new Vector2 (-0.7f, -0.7f);
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
