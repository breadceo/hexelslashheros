using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageManager : MonoBehaviour {
	public static StageManager GetInstance {
		get {
			return _instance;
		}
	}
	protected static StageManager _instance;
	public Stage currentStage;
	protected List<Stage> stages = new List<Stage> ();

	void Awake () {
		_instance = this;
		foreach (Transform c in transform) {
			if (c.CompareTag ("Stage")) {
				var s = c.gameObject.GetComponent <Stage> ();
				if (s != null) {
					stages.Add (s);
				} else {
					throw new UnityException ("stage tag gameobject must have stage component");
				}
			}
		}
	}

	public void LoadStage (string stageName) {
		stages.ForEach (s => {
			s.gameObject.SetActive (s.name == stageName);
			if (s.gameObject.activeSelf) {
				currentStage = s;
			}
		});
		currentStage.Loaded (GameManager.GetInstance.player);
	}

	public void LoadStageRandomly () {
		int idx = Random.Range (0, stages.Count);
		LoadStage (stages [idx].name);
	}
}
