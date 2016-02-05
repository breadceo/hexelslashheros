using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Text))]
public class NextStageConditionUI : MonoBehaviour {
	protected Text label;

	void Awake () {
		label = GetComponent <Text> ();
	}

	void OnEnable () {
		StageManager.GetInstance.OnRequireSoulChanged += OnRequireSoulChanged;
	}

	void OnDisable () {
		StageManager.GetInstance.OnRequireSoulChanged -= OnRequireSoulChanged;
	}

	void OnRequireSoulChanged (int requireSoul) {
		if (requireSoul > 0) {
			label.text = string.Format ("다음 층까지 <color=red>{0}</color> 영혼", requireSoul);
		} else {
			label.text = string.Format ("다음 층으로 가는 문이 열렸습니다.");
		}
	}
}
