using UnityEngine;
using System.Collections;

public class ScreenEffect : MonoBehaviour {
	[SerializeField] RectTransform focusEffect;
	protected float focusEndTime;
	protected float focusAngle;

	void Awake () {
		focusEndTime = Time.time;
	}

	void OnEnable () {
		GameManager.GetInstance.OnHitOccurs += OnHitOccurs;
	}

	void OnDisable () {
		GameManager.GetInstance.OnHitOccurs -= OnHitOccurs;
	}

	void Update () {
		if (Time.time < focusEndTime) {
			focusEffect.gameObject.SetActive (true);
			focusEffect.localRotation = Quaternion.Euler (0f, 0f, focusAngle);
		} else {
			focusEffect.gameObject.SetActive (false);
		}
	}

	void OnHitOccurs (GameObject player, GameObject enemy) {
		focusEndTime = Time.time + 0.25f;
		var dir = (enemy.transform.position - player.transform.position).normalized;
		dir = new Vector2 (dir.x, dir.y);
		focusAngle = Vector2.Angle (Vector2.up, dir);
		Debug.Log (focusAngle);
	}
}
