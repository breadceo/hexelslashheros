using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Animator))]
public class Visual : MonoBehaviour {
	[SerializeField] protected TrackPad Controller;
	protected Animator anim;
	protected GameObject weapon;
	protected SpriteRenderer weaponSpr;
	protected List<MotionBlur> blurs;

	void Awake () {
		weapon = transform.Find ("Weapon").gameObject;
		weaponSpr = weapon.GetComponent <SpriteRenderer> ();
		anim = GetComponent <Animator> ();
		blurs = new List<MotionBlur> ();
		blurs.Add (transform.Find ("Body").GetComponent <MotionBlur> ());
		blurs.Add (transform.Find ("Tail").GetComponent <MotionBlur> ());
		blurs.Add (transform.Find ("Weapon").GetComponent <MotionBlur> ());
		SetBlurs (false);
	}

	public void ChangeWeaponOrder (int order) {
		weaponSpr.sortingOrder = order;
	}
	
	void OnEnable () {
		Controller.OnChangeTrackPadState += ChangeTrackPadState;
		anim.enabled = false;
	}
	
	void OnDisable () {
		Controller.OnChangeTrackPadState -= ChangeTrackPadState;
	}

	protected void ChangeTrackPadState (TrackPadEvent e) {
		if (e.Kind == TrackPadEvent.EventKind.Swipe) {
			anim.enabled = true;
			if (e.Vector == TrackPad.UpRight) {
				anim.Play ("UpRight", 0, 0f);
				blurs.ForEach (b => b.offset = new Vector3 (0, 0, -1));
			} else if (e.Vector == TrackPad.UpLeft) {
				anim.Play ("UpLeft", 0, 0f);
				blurs.ForEach (b => b.offset = new Vector3 (0, 0, -1));
			} else if (e.Vector == TrackPad.DownRight) {
				anim.Play ("DownRight", 0, 0f);
				blurs.ForEach (b => b.offset = new Vector3 (0, 0, 1));
			} else if (e.Vector == TrackPad.DownLeft) {
				anim.Play ("DownLeft", 0, 0f);
				blurs.ForEach (b => b.offset = new Vector3 (0, 0, 1));
			}
		}
	}

	public void SetBlurs (bool enable) {
		blurs.ForEach (b => b.enabled = enable);
	}

	public void StopAnimation () {
		anim.enabled = false;
	}

	public void ForcePlayAnimation (TrackPadEvent e) {
		ChangeTrackPadState (e);
	}
}
