using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenEffect : MonoBehaviour {
	[SerializeField] Image focusEffect;
	protected Coroutine focusCoroutine;

	void OnEnable () {
		GameManager.GetInstance.OnHitOccurs += OnHitOccurs;
	}

	void OnDisable () {
		GameManager.GetInstance.OnHitOccurs -= OnHitOccurs;
	}

	void OnHitOccurs (GameObject attacker, GameObject defender) {
		if (focusCoroutine != null) {
			StopCoroutine (focusCoroutine);
		}
		var character = attacker.GetComponentInParent <Character> ();
		var dir = new Vector3 (character.Dir.x, character.Dir.y, 0f).normalized;
		focusCoroutine = StartCoroutine (Focus (0.1f, dir, 0.15f));
	}

	IEnumerator Focus (float timeToFocus, Vector3 focusDir, float fadeOut) {
		float startTime = Time.time;
		float t = Mathf.Clamp01 ((Time.time - startTime) / timeToFocus);
		focusEffect.gameObject.SetActive (true);
		focusEffect.rectTransform.localRotation = Quaternion.FromToRotation (Vector3.up, focusDir);
		focusEffect.color = Color.white;
		while (t < 1f) {
			t = Mathf.Clamp01 ((Time.time - startTime) / timeToFocus);
			yield return null;
		}
		startTime = Time.time;
		t = Mathf.Clamp01 ((Time.time - startTime) / fadeOut);
		while (t < 1f) {
			t = Mathf.Clamp01 ((Time.time - startTime) / fadeOut);
			focusEffect.color = new Color (1f, 1f, 1f, 1f - t);
			yield return null;
		}
		focusEffect.gameObject.SetActive (false);
	}
}
