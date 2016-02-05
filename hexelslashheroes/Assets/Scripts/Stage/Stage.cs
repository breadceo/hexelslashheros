using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stage : MonoBehaviour {
	[SerializeField] protected BoxCollider area;
	[SerializeField] protected BoxCollider door;
	[SerializeField] protected Transform startingPoint;
	protected List<AI> enemyPool = new List<AI> ();
	protected List<AI> enemyList = new List<AI> ();

	void Awake () {
		var enemies = transform.Find ("Enemies");
		foreach (Transform c in enemies) {
			var ai = c.GetComponent <AI> ();
			if (ai != null) {
				enemyPool.Add (ai);
			}
		}
	}

	void OnDisable () {
		enemyList.ForEach (e => {
			if (e != null) {
				GameManager.GetInstance.Destroy (e.gameObject);
			}
		});
	}

	public void Loaded (Player player) {
		player.transform.position = startingPoint.position;
		player.Init ();
		ActivateDoor (false);
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

	public void Init (int requireSoulCount) {
		for (int i = 0; i < requireSoulCount; ++i) {
			var template = enemyPool [Random.Range (0, enemyPool.Count)];
			var enemy = GameManager.GetInstance.Instantiate (template.gameObject);
			var ai = enemy.GetComponent <AI> ();
			var x = Random.Range (area.bounds.min.x, area.bounds.max.x);
			var y = Random.Range (area.bounds.min.y, area.bounds.max.y);
			ai.Spawn (new Vector3 (x, y, 0f));
			enemy.SetActive (true);
			enemyList.Add (ai);
		}
	}

	public void ActivateDoor (bool active) {
		door.enabled = active;
	}
}
