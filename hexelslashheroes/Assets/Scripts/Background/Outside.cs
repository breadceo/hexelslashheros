using UnityEngine;
using System.Collections;

public class Outside : MonoBehaviour {
	public string sortingLayerName;
	public SpriteRenderer TargetSprite;
	protected string savedSortingLayerName;

	void OnEnable () {
		if (TargetSprite != null) {
			savedSortingLayerName = TargetSprite.sortingLayerName;
			TargetSprite.sortingLayerName = sortingLayerName;
		}
	}

	void OnDisable () {
		if (TargetSprite != null) {
			TargetSprite.sortingLayerName = savedSortingLayerName;
			savedSortingLayerName = string.Empty;
		}
	}
}
