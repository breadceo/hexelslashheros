using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public static GameManager GetInstance {
		get {
			return _instance;
		}
	}
	protected static GameManager _instance;
	protected int currentFloor = 1;

	void Awake () {
		_instance = this;
	}

	void Start () {
		StageManager.GetInstance.LoadStage ("1", currentFloor);
	}

	void OnDestroy () {
		_instance = null;
	}

	public delegate void HitOccurs (GameObject attacker, GameObject defender);
	public event HitOccurs OnHitOccurs;
	public void InvokeHitEvent (GameObject attacker, GameObject defender) {
		if (OnHitOccurs != null) {
			OnHitOccurs (attacker, defender);
		}
		if (attacker.GetComponentInParent <Player> () != null) {
			GameSpeedManager.GetInstance.RequestTimeStop (0.05f, 0.5f, 0.2f);
		}
	}

	public void InvokeNextStageEvent () {
		currentFloor++;
		StageManager.GetInstance.LoadStageRandomly (currentFloor);
	}

	public Player player {
		get {
			Player player = null;
			var characters = GameObject.Find ("Root/Characters").gameObject;
			foreach (Transform c in characters.transform) {
				if (c.CompareTag ("Player")) {
					player = c.GetComponent <Player> ();
					break;
				}
			}
			return player;
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

	public delegate void ObjectDelegate (GameObject obj);
	public event ObjectDelegate OnObjectDead;
	public void InvokeDeadEvent (GameObject obj) {
		if (OnObjectDead != null) {
			OnObjectDead (obj);
		}
		var player = obj.GetComponent <Player> ();
		if (player != null) {
			GameSpeedManager.GetInstance.RequestTimeStop (0.05f, 0.5f, 0.2f);
			StartCoroutine (StartNewGame (1f));
		}
	}
	public event ObjectDelegate OnObjectSpawn;
	public void InvokeSpawnEvent (GameObject obj) {
		if (OnObjectSpawn != null) {
			OnObjectSpawn (obj);
		}
	}

	public GameObject Instantiate (GameObject obj) {
		return GameObject.Instantiate (obj);
	}

	public void Destroy (GameObject obj) {
		GameObject.Destroy (obj);
	}

	IEnumerator StartNewGame (float wait) {
		yield return new WaitForSeconds (wait);
		Application.LoadLevel ("main");
	}
}
