using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// \class  MotionBlur mesh utility
/// \brief  Apply motion blur to the owner.
public static class MotionBlurMeshUtility
{
	// Copy mesh
	public static void CopyMesh(Mesh a_rMeshFrom, ref Mesh a_rMeshTo) 
	{
		if(a_rMeshTo == null)
		{
			a_rMeshTo = new Mesh();
		}
		
		a_rMeshTo.Clear();
		
		if(a_rMeshFrom == null)
		{
			return;
		}
		
		a_rMeshTo.vertices = a_rMeshFrom.vertices;
		a_rMeshTo.colors = a_rMeshFrom.colors;
		a_rMeshTo.uv = a_rMeshFrom.uv;
		a_rMeshTo.uv2 = a_rMeshFrom.uv2;
		a_rMeshTo.normals = a_rMeshFrom.normals;
		a_rMeshTo.tangents = a_rMeshFrom.tangents;
		
		// Copy triangles without destroying submeshes
		int iSubMeshCount = a_rMeshFrom.subMeshCount;
		a_rMeshTo.subMeshCount = iSubMeshCount;
		for(int i = 0; i<iSubMeshCount; i++)
		{
			a_rMeshTo.SetTriangles(a_rMeshFrom.GetTriangles(i), i);
		}
	}
}
