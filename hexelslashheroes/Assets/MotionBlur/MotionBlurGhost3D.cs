using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("")]
/// \class  MotionBlurGhost
/// \brief  The ghost used on the motion blur effect
public class MotionBlurGhost3D : MotionBlurGhost 
{
    /// The renderer
    private Renderer m_rRenderer;

    /// The vertex colors
    private Color[] m_oVertexColors;

    /// The mesh
    private Mesh m_oGhostMesh;
	
	/// \brief  Awake
	private void Awake()
	{
		// Select
		m_rRenderer = GetComponent<Renderer>();
        CreateGhosthMeshIfNeeded();
	}
	
	/// \brief  Set mesh to ghost
	public void SetMeshToGhost(Mesh a_rMeshToGhost)
	{
        /// Copy the mesh
        CreateGhosthMeshIfNeeded();
        MotionBlurMeshUtility.CopyMesh(a_rMeshToGhost, ref m_oGhostMesh);

        // Copy the vertex color
        m_oVertexColors = m_oGhostMesh.colors.Clone() as Color[];

        if (m_oVertexColors.Length <= 0)
        {
            m_oVertexColors = new Color[a_rMeshToGhost.vertexCount];

            // Default color
            for (int i = 0; i < m_oVertexColors.Length; i++)
            {
                m_oVertexColors[i] = Color.white;
            }
        }
	}
	
	/// \brief  Update the mesh uv accordingly to the mesh to ghost uv
	public void UpdateMeshUV(Mesh a_rMeshToGhost)
	{
        m_oGhostMesh.uv = a_rMeshToGhost.uv;
	}
	
	/// \brief  Set opacity
    public override void SetOpacity(float a_fOpacity)
	{
        // Change the vertex color buffer
        for (int i = 0; i < m_oVertexColors.Length; i++)
        {
            m_oVertexColors[i].a = a_fOpacity;
        }

        // Apply it
        m_oGhostMesh.colors = m_oVertexColors;
	}
	
	/// \brief  Show
    public override void Show()
	{
        if (m_rRenderer.enabled == false)
        {
            m_rRenderer.enabled = true;
        }
	}
	
	/// \brief  Hide
    public override void Hide()
	{
        if (m_rRenderer != null && m_rRenderer.enabled)
        {
            m_rRenderer.enabled = false;
        }
	}

    // Create ghost mesh if needed
    private void CreateGhosthMeshIfNeeded()
    {
        if (m_oGhostMesh == null)
        {
            m_oGhostMesh = new Mesh();
            MeshFilter rMeshFilter = GetComponent<MeshFilter>();
            rMeshFilter.sharedMesh = m_oGhostMesh;
        }
    }
}
