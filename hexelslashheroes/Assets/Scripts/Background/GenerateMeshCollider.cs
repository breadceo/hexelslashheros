using UnityEngine;
using System.Collections;

public class GenerateMeshCollider : MonoBehaviour {
	[SerializeField] protected Vector3[] Vertices;
	[SerializeField] protected int[] Triangles;

	void Awake () {
		Mesh mesh = new Mesh ();
		gameObject.AddComponent <MeshFilter> ().mesh = mesh;
		mesh.vertices = Vertices;
		mesh.triangles = Triangles;
		gameObject.AddComponent <MeshCollider> ().sharedMesh = mesh;
	}
}
