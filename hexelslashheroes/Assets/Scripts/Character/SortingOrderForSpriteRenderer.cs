using UnityEngine;
using System.Collections;

public class SortingOrderForSpriteRenderer : SortingOrderable {
	[SerializeField] SpriteRenderer sprRenderer;

	public override void SetSortingOrder (int order) {
		sprRenderer.sortingOrder = order;
	}
}
