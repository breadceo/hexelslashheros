using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TrackPad : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {
	[SerializeField]
	protected float SwipeThreshold = 0.2f;
	[SerializeField]
	protected float MoveThreshold = 5f;
	protected float PointerDownStartTime;
	protected bool IsSwipe = false;
	protected TrackPadEvent Tpe = new TrackPadEvent ();

	public void OnPointerDown (PointerEventData eventData) {
		PointerDownStartTime = Time.time;
		Tpe.PointerDownPos = eventData.position;
	}

	public void OnBeginDrag (PointerEventData eventData) {
		if (Time.time - PointerDownStartTime <= SwipeThreshold) {
			Tpe.Kind = TrackPadEvent.EventKind.Swipe;
		} else {
			Tpe.Kind = TrackPadEvent.EventKind.Axis;
		}
	}

	public void OnEndDrag (PointerEventData eventData) {
		Tpe.PointerUpPos = eventData.position;
		Tpe.Dir = Vector2.zero;
		if (OnChangeTrackPadState != null) {
			OnChangeTrackPadState (Tpe);
		}
	}

	public void OnDrag (PointerEventData eventData) {
		var diff = eventData.position - Tpe.PointerDownPos;
		if (Tpe.Kind == TrackPadEvent.EventKind.Axis) {
			if (diff.magnitude > MoveThreshold) {
				Tpe.Dir = diff.normalized;
				Tpe.PointerDownPos = eventData.position;
			}
			if (OnChangeTrackPadState != null) {
				OnChangeTrackPadState (Tpe);
			}
		}
	}

	public delegate void ChangeTrackPadState (TrackPadEvent e);
	public event ChangeTrackPadState OnChangeTrackPadState;
}

public struct TrackPadEvent {
	public Vector2 PointerDownPos { get; set; }
	public Vector2 PointerUpPos { get; set; }
	public Vector2 Dir { get; set; }

	public enum EventKind {
		Axis,
		Swipe
	}
	public EventKind Kind { get; set; }
}

