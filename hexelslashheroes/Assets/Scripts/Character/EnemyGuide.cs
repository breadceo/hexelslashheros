using UnityEngine;
using System.Collections;

public class EnemyGuide : MonoBehaviour {
	protected GameObject source;
	protected GameObject target;

	void OnEnable () {
		GameManager.GetInstance.OnObjectDead += OnObjectDead;
	}

	void OnDisable () {
		GameManager.GetInstance.OnObjectDead -= OnObjectDead;
	}

	void OnObjectDead (GameObject obj) {
		if (target == obj) {
			GameManager.GetInstance.Destroy (gameObject);
		}
	}

	void Update () {
		if (source != null && target != null) {
			var dir = (target.transform.position - source.transform.position).normalized;
			transform.localRotation = Quaternion.FromToRotation (Vector3.up, dir);
		}
	}

	public void Trace (GameObject _source, GameObject _target) {
		source = _source;
		target = _target;
	}
}
