using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor (typeof(RenderOrderManager))]
public class RenderOrderManagerEditor : Editor {
	public override void OnInspectorGUI () {
		base.OnInspectorGUI ();
		var script = (RenderOrderManager)target;
		if (script != null) {
			foreach (var obj in script.Objects) {
				EditorGUILayout.LabelField (obj.Target.name);
				EditorGUI.indentLevel++;
				var visualTransform = obj.Target.transform.Find ("Visual");
				if (visualTransform != null) {
					var visual = visualTransform.GetComponent <Visual> ();
					if (visual != null) {
						List<OrderAndName> orderAndName = new List<OrderAndName> ();
						List<SpriteRenderer> sprites = new List<SpriteRenderer> ();
						List<TrailRenderer> trails = new List<TrailRenderer> ();
						foreach (Transform c in visual.transform) {
							var spr = c.GetComponent <SpriteRenderer> ();
							if (spr != null) {
								sprites.Add (spr);
							}
							var trail = c.GetComponent <TrailRenderer> ();
							if (trail != null) {
								trails.Add (trail);
							}
						}
						sprites.ForEach (spr => {
							orderAndName.Add (new OrderAndName { order = spr.sortingOrder, name = spr.name });
						});
						trails.ForEach (trail => {
							orderAndName.Add (new OrderAndName { order = trail.sortingOrder, name = trail.name });
						});
						orderAndName = orderAndName.OrderBy (o => {
							return o.order;
						}).ToList ();
						orderAndName.ForEach (o => {
							EditorGUILayout.LabelField (string.Format ("{0}({1})", o.name, o.order));
						});
					}
				}
				EditorGUI.indentLevel--;
			}
			Repaint ();
		}
	}

	public class OrderAndName {
		public int order;
		public string name;
	}
}
