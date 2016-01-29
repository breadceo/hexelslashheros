using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AI : IController, RenderObject {
	[SerializeField] GameObject trackCharacter;
	[SerializeField] int trackOffset;
	protected Visual visual;
	protected List <Vector3> trackPositions = new List<Vector3> ();
	protected Vector3 dir;
	protected Vector3 prevDir;

	void Awake () {
		visual = transform.Find ("Visual").GetComponent <Visual> ();
	}

	void OnEnable () {
		RenderOrderManager.GetInstance.Register (this);
	}

	void OnDisable () {
		RenderOrderManager.GetInstance.UnRegister (this);
	}

	public override ControlEvent CreateTrackPadEventByDirection (Vector3 dir) {
		return new ControlEvent {
			Vector = new Vector2 (dir.x, dir.y),
			Kind = ControlEvent.EventKind.Swipe
		};
	}

	public int MakeOrder (int start) {
		var info = visual.anim.GetCurrentAnimatorStateInfo (0);
		if (info.shortNameHash == visual.UpLeftAnimationHash || info.shortNameHash == visual.DownLeftAnimationHash) {
			visual.bodySpr.sortingOrder = start;
			visual.weaponSpr.sortingOrder = start + 1;
			visual.tailSpr.sortingOrder = start + 2;
			return start + 3;
		} else {
			visual.bodySpr.sortingOrder = start + 1;
			visual.tailSpr.sortingOrder = start + 2;
			visual.weaponSpr.sortingOrder = start;
			return start + 3;
		}
	}

	public Vector3 RenderObjectPosition {
		get {
			return gameObject.transform.position;
		}
	}

	void Update () {
		trackPositions.Add (trackCharacter.transform.position);
		if (trackPositions.Count > trackOffset) {
			var diff = trackPositions [1] - trackPositions [0];
			dir = diff.normalized;
			if (prevDir != dir) {
				visual.ForcePlayAnimation (CreateTrackPadEventByDirection (dir));
			}
			transform.position = trackPositions.First <Vector3> () - dir * 0.25f;
			trackPositions.RemoveAt (0);
			prevDir = dir;
		} // else if stopped
	}
}
