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
	public List<RenderObject> Objects {
		get {
			return objects;
		}
	}

	void Awake () {
		_instance = this;
	}

	void OnDestroy () {
		_instance = null;
	}

	static protected Vector3 basePosition = new Vector3 (0f, 1000f, 0f);
	void Update () {
		objects = objects.OrderBy (o => {
			return Vector3.Distance (basePosition, o.Target.transform.position);
		}).ToList <RenderObject> ();

		int startOrder = 0;
		foreach (var ro in objects) {
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
	GameObject Target { get; }
}