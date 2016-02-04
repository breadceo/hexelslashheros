using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Camera))]
public class PlayerTrackingCamera : MonoBehaviour {
	[SerializeField]
	protected Player player;
	protected Camera trackTarget;

	void Awake () {
		trackTarget = GetComponent <Camera> ();
	}

	void Update () {
		var targetPos = new Vector3 (player.transform.position.x, player.transform.position.y, -10f);
		trackTarget.transform.position = targetPos;
	}
}
