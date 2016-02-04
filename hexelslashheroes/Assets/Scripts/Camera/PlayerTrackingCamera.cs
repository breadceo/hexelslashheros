using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Camera))]
public class PlayerTrackingCamera : MonoBehaviour {
	[SerializeField]
	protected Character Player;
	protected Camera Target;

	void Awake () {
		Target = GetComponent <Camera> ();
	}

	void Update () {
		var targetPos = new Vector3 (Player.transform.position.x, Player.transform.position.y, -10f);
		Target.transform.position = targetPos;
	}
}
