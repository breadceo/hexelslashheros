using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AI : IController, RenderObject {
	[SerializeField] protected BoxCollider body;
	[SerializeField] protected BoxCollider weapon;
	[SerializeField] protected float sight;
	protected Visual visual;

	void Awake () {
		visual = transform.Find ("Visual").GetComponent <Visual> ();
	}

	void OnEnable () {
		RenderOrderManager.GetInstance.Register (this);
		GameManager.GetInstance.OnHitOccurs += OnHitOccurs;
	}

	void OnDisable () {
		RenderOrderManager.GetInstance.UnRegister (this);
		GameManager.GetInstance.OnHitOccurs -= OnHitOccurs;
	}

	public override ControlEvent CreateTrackPadEventByDirection (Vector3 dir) {
		return new ControlEvent {
			Vector = new Vector2 (dir.x, dir.y),
			Kind = ControlEvent.EventKind.Swipe
		};
	}

	public int MakeOrder (int start) {
		var info = visual.anim.GetCurrentAnimatorStateInfo (0);
		if (info.shortNameHash == visual.UpRightAnimationHash || info.shortNameHash == visual.DownRightAnimationHash) {
			visual.bodySpr.sortingOrder = start + 1;
			visual.tailSpr.sortingOrder = start + 2;
			visual.weaponSpr.sortingOrder = start;
			return start + 3;
		} else {
			visual.bodySpr.sortingOrder = start;
			visual.weaponSpr.sortingOrder = start + 1;
			visual.tailSpr.sortingOrder = start + 2;
			return start + 3;
		}
	}

	public GameObject Target {
		get {
			return gameObject;
		}
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
		}
	}
}
