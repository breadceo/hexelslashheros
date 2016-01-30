using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public static GameManager GetInstance {
		get {
			return _instance;
		}
	}
	protected static GameManager _instance;

	void Awake () {
		_instance = this;
	}

	void OnDestroy () {
		_instance = null;
	}

	public delegate void HitOccurs (GameObject player, GameObject enemy);
	public event HitOccurs OnHitOccurs;

	public void InvokeHitEvent (GameObject player, GameObject enemy) {
		// INFO: player doesn't have character component, it's just weapon collider.
		if (OnHitOccurs != null) {
			OnHitOccurs (player, enemy);
		}
		GameSpeedManager.GetInstance.RequestTimeStop (0.05f, 0.05f, 0.2f);
	}
}
