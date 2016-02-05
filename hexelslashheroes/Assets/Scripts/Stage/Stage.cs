using UnityEngine;
using System.Collections;

public class Stage : MonoBehaviour {
	[SerializeField] protected BoxCollider area;
	[SerializeField] protected BoxCollider door;
	[SerializeField] protected Transform startingPoint;

	public void Loaded (Player player) {
		player.transform.position = startingPoint.position;
		player.Init ();
		door.enabled = false;
	}

	public bool CheckBlocked (GameObject go) {
		var goPos = go.transform.position;
		Vector3 blockedPos = new Vector3 (Mathf.Clamp (goPos.x, area.bounds.min.x, area.bounds.max.x),
			Mathf.Clamp (goPos.y, area.bounds.min.y, area.bounds.max.y),
			goPos.z);
		bool blocked = blockedPos != goPos;
		if (blocked) {
			go.transform.position = blockedPos;
		}
		return blocked;
	}
}
