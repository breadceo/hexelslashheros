using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class TrackPad : IController, IPointerDownHandler, IPointerClickHandler {
	protected Vector2 pointerDownPos;
	protected ControlEvent controlEvent = new ControlEvent ();
	[SerializeField] float trackPadSensibility = 50f;

	void Awake () {
		var x = Mathf.Cos (Mathf.PI / 6f);
		var y = Mathf.Sin (Mathf.PI / 6f);
		UpRight = new Vector2 (x, y).normalized;
		UpLeft = new Vector2 (-x, y).normalized;
		DownRight = new Vector2 (x, -y).normalized;
		DownLeft = new Vector2 (-x, -y).normalized;
		Directions = new List<Vector2> { UpLeft, UpRight, DownLeft, DownRight };
	}

	public void OnPointerDown (PointerEventData eventData) {
		pointerDownPos = eventData.position;
	}

	public void OnPointerClick (PointerEventData eventData) {
		var diff = eventData.position - pointerDownPos;
		if (diff.magnitude > trackPadSensibility) {
			diff = diff.normalized;
			controlEvent = CreateTrackPadEventByDirection (diff);
		} else {
			controlEvent.Kind = ControlEvent.EventKind.Touch;
			controlEvent.Vector = eventData.position;
		}
		SpawnEvent (controlEvent);
	}

	public override ControlEvent CreateTrackPadEventByDirection (Vector3 dir) {
		ControlEvent ret = new ControlEvent ();
		ret.Kind = ControlEvent.EventKind.Swipe;
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
			controlEvent.Kind = ControlEvent.EventKind.Swipe;
			controlEvent.Vector = UpLeft;
			dispatchEvent = true;
		} else if (Input.GetKey (KeyCode.W) && Input.GetKey (KeyCode.D)) {
			controlEvent.Kind = ControlEvent.EventKind.Swipe;
			controlEvent.Vector = UpRight;
			dispatchEvent = true;
		} else if (Input.GetKey (KeyCode.S) && Input.GetKey (KeyCode.A)) {
			controlEvent.Kind = ControlEvent.EventKind.Swipe;
			controlEvent.Vector = DownLeft;
			dispatchEvent = true;
		} else if (Input.GetKey (KeyCode.S) && Input.GetKey (KeyCode.D)) {
			controlEvent.Kind = ControlEvent.EventKind.Swipe;
			controlEvent.Vector = DownRight;
			dispatchEvent = true;
		}
		if (dispatchEvent) {
			SpawnEvent (controlEvent);
		}
	}

	internal static Vector2 UpRight;
	internal static Vector2 UpLeft;
	internal static Vector2 DownRight;
	internal static Vector2 DownLeft;
	internal static List<Vector2> Directions;
	internal static Vector2 RandomDirection {
		get {
			return TrackPad.Directions [Random.Range (0, TrackPad.Directions.Count)];
		}
	}
}
