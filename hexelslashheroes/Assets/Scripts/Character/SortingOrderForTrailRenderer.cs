using UnityEngine;
using System.Collections;

public class SortingOrderForTrailRenderer : SortingOrderable {
	[SerializeField] TrailRenderer trailRenderer;

	public override void SetSortingOrder (int order) {
		trailRenderer.sortingOrder = order;
	}
}
