using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(RenderOrderManager))]
public class RenderOrderManagerEditor : Editor {
	public override void OnInspectorGUI () {
		base.OnInspectorGUI ();
		var script = (RenderOrderManager)target;
		if (script != null) {
			foreach (var obj in script.Objects) {
				EditorGUILayout.LabelField (obj.Target.name);
			}
			Repaint ();
		}
	}
}
