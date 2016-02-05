using UnityEngine;
using System.Collections;

[RequireComponent (typeof(LineRenderer))]
public class SightDrawer : MonoBehaviour {
	[SerializeField] protected float ThetaScale = 0.01f;
	protected LineRenderer lineRenderer;
	protected float radius = 0f;
	private int Size;
	private float Theta = 0f;

	void Awake () {
		lineRenderer = GetComponent <LineRenderer> ();
		lineRenderer.sortingLayerName = "Background";
		lineRenderer.sortingOrder = 1;
	}

	void Update () {      
		Theta = 0f;
		Size = (int)((1f / ThetaScale) + 1f);
		lineRenderer.SetVertexCount(Size); 
		for(int i = 0; i < Size; i++){          
			Theta += (2.0f * Mathf.PI * ThetaScale);         
			float x = transform.position.x + radius * Mathf.Cos(Theta);
			float y = transform.position.y + radius * Mathf.Sin(Theta);          
			lineRenderer.SetPosition (i, new Vector3(x, y, 0));
		}
	}

	public void SetRadius (float radius) {
		this.radius = radius;
	}
}
