using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Camera))]
public class PlayerTrackingCamera : MonoBehaviour {
	[SerializeField]
	protected Character Player;
	protected Camera camera;

	void Awake () {
		camera = GetComponent <Camera> ();
	}

	void Update () {
		var targetPos = new Vector3 (Player.transform.position.x, Player.transform.position.y, -10f);
		camera.transform.position = Vector3.Lerp (camera.transform.position, targetPos, 0.1f);
	}
}
