using UnityEngine;
using System.Collections;

public class AutoDestoryParticle : MonoBehaviour {
	void OnEnable () {
		var system = gameObject.GetComponent <ParticleSystem> ();
		if (system.loop == false) {
			StartCoroutine (AutoDestroy ());
		}
	}

	IEnumerator AutoDestroy () {
		var system = gameObject.GetComponent <ParticleSystem> ();
		system.Stop ();
		system.Play ();
		yield return new WaitForSeconds (system.startLifetime);
		GameObject.Destroy (gameObject);
	}
}
