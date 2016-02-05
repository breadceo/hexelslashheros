using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Text))]
public class StageTitleUI : MonoBehaviour {
	protected Text label;

	void Awake () {
		label = GetComponent <Text> ();
	}

	void OnEnable () {
		StageManager.GetInstance.OnStageChanged += OnStageChanged;
	}

	void OnDisable () {
		StageManager.GetInstance.OnStageChanged -= OnStageChanged;
	}

	void OnStageChanged (int floor) {
		label.text = string.Format ("도산지옥 <color=red>{0}</color>층", floor);
	}
}
