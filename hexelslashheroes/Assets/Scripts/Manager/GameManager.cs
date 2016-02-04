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

	void Start () {
		StageManager.GetInstance.LoadStage ("1");
	}

	void OnDestroy () {
		_instance = null;
	}

	public delegate void HitOccurs (GameObject attacker, GameObject defender);
	public event HitOccurs OnHitOccurs;
//	public delegate void 

	public void InvokeHitEvent (GameObject attacker, GameObject defender) {
		// INFO: player doesn't have character component, it's just weapon collider.
		if (OnHitOccurs != null) {
			OnHitOccurs (attacker, defender);
		}
		GameSpeedManager.GetInstance.RequestTimeStop (0.05f, 0.05f, 0.2f);
	}

	public void InvokeNextStageEvent () {
		StageManager.GetInstance.LoadStageRandomly ();
	}

	public Character player {
		get {
			Character character = null;
			var characters = GameObject.Find ("Root/Characters").gameObject;
			foreach (Transform c in characters.transform) {
				if (c.CompareTag ("Player")) {
					character = c.GetComponent <Character> ();
					break;
				}
			}
			if (character == null) {
				throw new UnityException ("failed to find player");
			} else {
				return character;
			}
		}
	}

	public Camera playerCamera {
		get {
			var cam = GameObject.Find ("Player Camera").GetComponent <Camera> ();
			if (cam != null) {
				return cam;
			} else {
				throw new UnityException ("failed to find player camera");
			}
		}
	}
}
