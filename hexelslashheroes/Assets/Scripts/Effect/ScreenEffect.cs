using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenEffect : MonoBehaviour {
	[SerializeField] Image focusEffect;
	protected Vector3 dir = Vector3.zero;

	void OnEnable () {
		Debug.Assert (GameManager.GetInstance != null);
		GameManager.GetInstance.OnHitOccurs += OnHitOccurs;
	}

	void OnDisable () {
		if (GameManager.GetInstance != null)
			GameManager.GetInstance.OnHitOccurs -= OnHitOccurs;
	}

	void OnHitOccurs (GameObject attacker, GameObject defender) {
		// TODO: 정리
		var player = attacker.GetComponentInParent <Player> ();
		if (player) {
			dir = new Vector3 (player.Dir.x, player.Dir.y, 0f).normalized;
		}
		var ai = attacker.GetComponentInParent <AI> ();
		if (ai) {
			dir = new Vector3 (ai.Dir.x, ai.Dir.y, 0f).normalized;
		}
	}

	void Update () {
		if (GameSpeedManager.GetInstance.NeedFocus) {
			focusEffect.gameObject.SetActive (true);
			focusEffect.rectTransform.localRotation = Quaternion.FromToRotation (Vector3.up, dir);
		} else {
			focusEffect.gameObject.SetActive (false);
		}
	}
}
