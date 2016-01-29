using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RenderOrderManager : MonoBehaviour {
	public static RenderOrderManager GetInstance {
		get {
			return _instance;
		}
	}
	protected static RenderOrderManager _instance;
	protected List<RenderObject> objects = new List<RenderObject> ();

	void Awake () {
		_instance = this;
	}

	void OnDestroy () {
		_instance = null;
	}

	void Update () {
		var sorted = objects.OrderByDescending (o => {
			return o.RenderObjectPosition.y;
		}).ToList <RenderObject> ();

		int startOrder = 0;
		foreach (var ro in sorted) {
			startOrder = ro.MakeOrder (startOrder);
		}
	}

	public void Register (RenderObject obj) {
		objects.Add (obj);
	}

	public void UnRegister (RenderObject obj) {
		objects.Remove (obj);
	}
}

public interface RenderObject {
	int MakeOrder (int start);
	Vector3 RenderObjectPosition { get; }
}