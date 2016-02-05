using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class StageManager : MonoBehaviour {
	public static StageManager GetInstance {
		get {
			return _instance;
		}
	}
	protected static StageManager _instance;
	public Stage currentStage;
	protected List<Stage> stages = new List<Stage> ();
	protected Dictionary<int, int> designRequireSouls = new Dictionary<int, int> ();
	protected int currentRequireSoul;

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
		designRequireSouls.Clear ();
		var balanceText = Resources.Load<TextAsset> ("Design/stagebalance");
		Debug.Assert (balanceText != null);
		using (StringReader r = new StringReader (balanceText.text)) {
			string line = string.Empty;
			do {
				line = r.ReadLine ();
				if (string.IsNullOrEmpty (line) == false) {
					line = line.Replace ('\r', '\0').Replace ('\n', '\0');
					if (line.Length > 0) {
						var splitted = line.Split (new char[] {','});
						Debug.Assert (splitted.Length == 2);
						int level = int.Parse (splitted [0]);
						int soul = int.Parse (splitted [1]);
						designRequireSouls.Add (level, soul);
					}
				}
			} while (string.IsNullOrEmpty (line) == false);
		}
	}
	
	void OnEnable () {
		GameManager.GetInstance.OnObjectDead += OnObjectDead;
	}
	
	void OnDisable () {
		GameManager.GetInstance.OnObjectDead -= OnObjectDead;
	}

	public delegate void RequireSoulChanged (int requireSoul);
	public event RequireSoulChanged OnRequireSoulChanged;
	void OnObjectDead (GameObject obj) {
		currentRequireSoul = Mathf.Max (0, currentRequireSoul - 1);
		if (OnRequireSoulChanged != null) {
			OnRequireSoulChanged (currentRequireSoul);
		}
		currentStage.ActivateDoor (currentRequireSoul <= 0);
	}

	public delegate void StageChanged (int floor);
	public event StageChanged OnStageChanged;
	public void LoadStage (string stageName, int floor) {
		int requireSoul = 0;
		if (designRequireSouls.TryGetValue (floor, out requireSoul)) {
			currentRequireSoul = requireSoul;
			stages.ForEach (s => {
				s.gameObject.SetActive (s.name == stageName);
				if (s.gameObject.activeSelf) {
					currentStage = s;
				}
			});
			currentStage.Init (currentRequireSoul);
			currentStage.Loaded (GameManager.GetInstance.player);
			if (OnStageChanged != null) {
				OnStageChanged (floor);
			}
			if (OnRequireSoulChanged != null) {
				OnRequireSoulChanged (currentRequireSoul);
			}
		} else {
			throw new UnityException ("failed to find require soul");
		}
	}

	public void LoadStageRandomly (int floor) {
		int idx = Random.Range (0, stages.Count);
		LoadStage (stages [idx].name, floor);
	}
}
