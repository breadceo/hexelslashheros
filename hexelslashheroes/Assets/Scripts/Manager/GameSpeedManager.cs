using UnityEngine;
using System.Collections;

public class GameSpeedManager : MonoBehaviour {
	public static GameSpeedManager GetInstance {
		get {
			return _instance;
		}
	}
	protected static GameSpeedManager _instance;
	
	void Awake () {
		_instance = this;
	}
	
	void OnDestroy () {
		_instance = null;
	}

	public bool NeedFocus {
		get {
			return timeStopCoroutine != null;
		}
	}

	Coroutine timeStopCoroutine;
	public void RequestTimeStop (float fadeIn, float fadeOut, float timeToStop) {
		if (timeStopCoroutine != null) {
			StopCoroutine (timeStopCoroutine);
		}
		timeStopCoroutine = StartCoroutine (TimeStop (fadeIn, fadeOut, timeToStop));
	}

	IEnumerator TimeStop (float fadeIn, float fadeOut, float timeToStop) {
		Time.timeScale = 1f;
		if (fadeIn > 0f) {
			float rt = Time.realtimeSinceStartup;
			float t = Mathf.Clamp01 ((Time.realtimeSinceStartup - rt) / fadeIn);
			while (t < 1.0f) {
				t = Mathf.Clamp01 ((Time.realtimeSinceStartup - rt) / fadeIn);
				Time.timeScale = 1.0f - t;
				yield return null;
			}
		}
		if (timeToStop > 0f) {
			Time.timeScale = 0f;
			float rt = Time.realtimeSinceStartup;
			float t = Mathf.Clamp01 ((Time.realtimeSinceStartup - rt) / timeToStop);
			while (t < 1.0f) {
				t = Mathf.Clamp01 ((Time.realtimeSinceStartup - rt) / timeToStop);
				yield return null;
			}
		}
		if (fadeOut > 0f) {
			float rt = Time.realtimeSinceStartup;
			float t = Mathf.Clamp01 ((Time.realtimeSinceStartup - rt) / fadeOut);
			while (t < 1.0f) {
				t = Mathf.Clamp01 ((Time.realtimeSinceStartup - rt) / fadeOut);
				Time.timeScale = t;
				yield return null;
			}
		}
		Time.timeScale = 1f;
		timeStopCoroutine = null;
	}
}
