using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Animator))]
public class Visual : MonoBehaviour {
	[SerializeField] protected TrackPad Controller;
	protected Animator Anim;
	protected GameObject Weapon;
	protected SpriteRenderer WeaponSpr;

	void Awake () {
		Weapon = transform.Find ("Weapon").gameObject;
		WeaponSpr = Weapon.GetComponent <SpriteRenderer> ();
		Anim = GetComponent <Animator> ();
	}

	public void ChangeWeaponOrder (int order) {
		WeaponSpr.sortingOrder = order;
	}
	
	void OnEnable () {
		Controller.OnChangeTrackPadState += ChangeTrackPadState;
	}
	
	void OnDisable () {
		Controller.OnChangeTrackPadState -= ChangeTrackPadState;
	}

	protected void ChangeTrackPadState (TrackPadEvent e) {
		if (e.Kind == TrackPadEvent.EventKind.Swipe) {
			if (e.Vector == TrackPad.UpRight) {
				Anim.Play ("UpRight", 0, 0f);
			} else if (e.Vector == TrackPad.UpLeft) {
				Anim.Play ("UpLeft", 0, 0f);
			} else if (e.Vector == TrackPad.DownRight) {
				Anim.Play ("DownRight", 0, 0f);
			} else if (e.Vector == TrackPad.DownLeft) {
				Anim.Play ("DownLeft", 0, 0f);
			}
		}
	}
}
