using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Animator))]
public class Visual : MonoBehaviour {
	[SerializeField] protected IController Controller;
	public Animator anim;
	public SpriteRenderer bodySpr;
	public SpriteRenderer tailSpr;
	public SpriteRenderer weaponSpr;
	protected List<MotionBlur> blurs = new List<MotionBlur> ();

	public int UpRightAnimationHash {
		get;
		private set;
	}
	public int UpLeftAnimationHash {
		get;
		private set;
	}
	public int DownRightAnimationHash {
		get;
		private set;
	}
	public int DownLeftAnimationHash {
		get;
		private set;
	}

	void Awake () {
		UpRightAnimationHash = Animator.StringToHash ("UpRight");
		UpLeftAnimationHash = Animator.StringToHash ("UpLeft");
		DownRightAnimationHash = Animator.StringToHash ("DownRight");
		DownLeftAnimationHash = Animator.StringToHash ("DownLeft");

		var weapon = transform.Find ("Weapon").gameObject;
		weaponSpr = weapon.GetComponent <SpriteRenderer> ();
		var body = transform.Find ("Body").gameObject;
		bodySpr = body.GetComponent <SpriteRenderer> ();
		var tail = transform.Find ("Tail").gameObject;
		tailSpr = tail.GetComponent <SpriteRenderer> ();
		anim = GetComponent <Animator> ();
		var bodyBlur = body.GetComponent <MotionBlur> ();
		if (bodyBlur) {
			blurs.Add (bodyBlur);
		}
		var tailBlur = tail.GetComponent <MotionBlur> ();
		if (tailBlur) {
			blurs.Add (tailBlur);
		}
		var weaponBlur = weapon.GetComponent <MotionBlur> ();
		if (weaponBlur) {
			blurs.Add (weaponBlur);
		}
		SetBlurs (false);
	}

	void OnEnable () {
		Controller.OnChangeController += ChangeTrackPadState;
		anim.enabled = false;
	}
	
	void OnDisable () {
		Controller.OnChangeController -= ChangeTrackPadState;
	}

	protected void ChangeTrackPadState (ControlEvent e) {
		if (e.Kind == ControlEvent.EventKind.Swipe) {
			anim.enabled = true;
			if (e.Vector == TrackPad.UpRight) {
				anim.Play (UpRightAnimationHash, 0, 0f);
				blurs.ForEach (b => b.offset = new Vector3 (0, 0, -1));
			} else if (e.Vector == TrackPad.UpLeft) {
				anim.Play (UpLeftAnimationHash, 0, 0f);
				blurs.ForEach (b => b.offset = new Vector3 (0, 0, -1));
			} else if (e.Vector == TrackPad.DownRight) {
				anim.Play (DownRightAnimationHash, 0, 0f);
				blurs.ForEach (b => b.offset = new Vector3 (0, 0, 1));
			} else if (e.Vector == TrackPad.DownLeft) {
				anim.Play (DownLeftAnimationHash, 0, 0f);
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

	public void ForcePlayAnimation (ControlEvent e) {
		ChangeTrackPadState (e);
	}
}
