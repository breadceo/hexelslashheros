using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (Animator))]
public class Visual : MonoBehaviour, RenderObject {
	[SerializeField] protected IController Controller;
	[SerializeField] protected List<SortingOrderable> leftOrder = new List<SortingOrderable> ();
	[SerializeField] protected List<SortingOrderable> rightOrder = new List<SortingOrderable> ();
	protected Animator anim;

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
		anim = GetComponent <Animator> ();
	}

	void OnEnable () {
		RenderOrderManager.GetInstance.Register (this);
		Controller.OnChangeController += ChangeTrackPadState;
		anim.enabled = false;
	}
	
	void OnDisable () {
		if (RenderOrderManager.GetInstance != null) {
			RenderOrderManager.GetInstance.UnRegister (this);
		}
		Controller.OnChangeController -= ChangeTrackPadState;
	}

	protected void ChangeTrackPadState (ControlEvent e) {
		if (e.Kind == ControlEvent.EventKind.Swipe) {
			anim.enabled = true;
			if (e.Vector == TrackPad.UpRight) {
				anim.Play (UpRightAnimationHash, 0, 0f);
			} else if (e.Vector == TrackPad.UpLeft) {
				anim.Play (UpLeftAnimationHash, 0, 0f);
			} else if (e.Vector == TrackPad.DownRight) {
				anim.Play (DownRightAnimationHash, 0, 0f);
			} else if (e.Vector == TrackPad.DownLeft) {
				anim.Play (DownLeftAnimationHash, 0, 0f);
			}
		}
	}

	public void StopAnimation () {
		anim.enabled = false;
	}

	public void ForcePlayAnimation (ControlEvent e) {
		ChangeTrackPadState (e);
	}

	public int MakeOrder (int start) {
		if (anim.enabled) {
			var info = anim.GetCurrentAnimatorStateInfo (0);
			if (info.shortNameHash == UpRightAnimationHash || info.shortNameHash == DownRightAnimationHash) {
				for (int i = 0; i < rightOrder.Count; ++i) {
					rightOrder [i].SetSortingOrder (start + i);
				}
				return start + rightOrder.Count;
			} else {
				for (int i = 0; i < leftOrder.Count; ++i) {
					leftOrder [i].SetSortingOrder (start + i);
				}
				return start + leftOrder.Count;
			}
		}
		return start + rightOrder.Count;
	}

	public GameObject Target {
		get {
			return transform.parent.gameObject;
		}
	}
}
