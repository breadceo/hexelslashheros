using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AI : IController {
	[SerializeField] protected BoxCollider body;
	[SerializeField] protected BoxCollider weapon;
	[SerializeField] protected float sight;
	protected Visual visual;

	void Awake () {
		visual = transform.Find ("Visual").GetComponent <Visual> ();
	}

	void OnEnable () {
		GameManager.GetInstance.OnHitOccurs += OnHitOccurs;
	}

	void OnDisable () {
		if (GameManager.GetInstance != null) {
			GameManager.GetInstance.OnHitOccurs -= OnHitOccurs;
		}
	}

	public override ControlEvent CreateTrackPadEventByDirection (Vector3 dir) {
		return new ControlEvent {
			Vector = new Vector2 (dir.x, dir.y),
			Kind = ControlEvent.EventKind.Swipe
		};
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag ("PlayerWeapon")) {
			GameManager.GetInstance.InvokeHitEvent (other.gameObject, gameObject);
		}
	}

	void OnHitOccurs (GameObject attacker, GameObject defender) {
		var ai = defender.GetComponentInParent <AI> ();
		if (ai == this) {
			var ec = attacker.GetComponentInParent <EffectContainer> ();
			if (ec != null) {
				var particle = GameObject.Instantiate (ec.hit) as GameObject;
				particle.SetActive (true);
				particle.transform.position = transform.position;
				var system = particle.GetComponent <ParticleSystem> ();
				system.Stop ();
				system.Play ();
			}
			GameManager.GetInstance.Destroy (gameObject);
			GameManager.GetInstance.InvokeDeadEvent (gameObject);
		}
	}
}
