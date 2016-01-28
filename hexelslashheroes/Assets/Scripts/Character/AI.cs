using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AI : IController {
	[SerializeField] GameObject trackCharacter;
	[SerializeField] int trackOffset;
	protected Visual visual;
	protected List <Vector3> trackPositions = new List<Vector3> ();
	protected Vector3 dir;
	protected Vector3 prevDir;

	void Awake () {
		visual = transform.Find ("Visual").GetComponent <Visual> ();
	}

	public override ControlEvent CreateTrackPadEventByDirection (Vector3 dir) {
		return new ControlEvent {
			Vector = new Vector2 (dir.x, dir.y),
			Kind = ControlEvent.EventKind.Swipe
		};
	}

	void Update () {
		trackPositions.Add (trackCharacter.transform.position);
		if (trackPositions.Count > trackOffset) {
			var diff = trackPositions [1] - trackPositions [0];
			dir = diff.normalized;
			if (prevDir != dir) {
				visual.ForcePlayAnimation (CreateTrackPadEventByDirection (dir));
			}
			transform.position = trackPositions.First <Vector3> ();
			trackPositions.RemoveAt (0);
			prevDir = dir;
		} // else if stopped
	}
}
