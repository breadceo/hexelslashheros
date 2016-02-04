using UnityEngine;
using System.Collections;

public class TrailController : MonoBehaviour {
	[SerializeField] protected string SortingLayerName = "Default";
	protected TrailRenderer Trail;

	void Awake () {
		Trail = GetComponent <TrailRenderer> ();
		if (Trail != null) {
			Trail.sortingLayerName = SortingLayerName;
		}
	}
}
