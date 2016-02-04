using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlurController : MonoBehaviour {
	[SerializeField] protected IController Controller;
	[SerializeField] protected List<MotionBlur> blurs = new List<MotionBlur> ();

	void Awake () {
		SetBlurs (false);
	}

	void OnEnable () {
		Controller.OnChangeController += ChangeTrackPadState;
	}

	void OnDisable () {
		Controller.OnChangeController -= ChangeTrackPadState;
	}

	protected void ChangeTrackPadState (ControlEvent e) {
		if (e.Kind == ControlEvent.EventKind.Swipe) {
			if (e.Vector == TrackPad.UpRight) {
				blurs.ForEach (b => b.offset = new Vector3 (0, 0, -1));
			} else if (e.Vector == TrackPad.UpLeft) {
				blurs.ForEach (b => b.offset = new Vector3 (0, 0, -1));
			} else if (e.Vector == TrackPad.DownRight) {
				blurs.ForEach (b => b.offset = new Vector3 (0, 0, 1));
			} else if (e.Vector == TrackPad.DownLeft) {
				blurs.ForEach (b => b.offset = new Vector3 (0, 0, 1));
			}
		}
	}

	public void SetBlurs (bool enable) {
		blurs.ForEach (b => b.enabled = enable);
	}
}
