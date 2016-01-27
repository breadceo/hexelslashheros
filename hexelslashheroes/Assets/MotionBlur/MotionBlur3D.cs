using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// \class  MotionBlur
/// \brief  Apply motion blur to the 3D owner.
public class MotionBlur3D : MotionBlurBase 
{
	/// The owner mesh filter
	private MeshFilter m_rMeshFilter;
	
	/// The last set mesh to ghost
	private Mesh m_rLastSetMeshToGhost;
	
	/// Is a mesh uv update has been requested?
	private bool m_bUpdateMeshUVRequested;

    public MotionBlur3D(MotionBlur b, MeshFilter mf) : base(b)
    {
        m_rMeshFilter = mf;
    }
	
	/// \brief  Update Parameters
	protected override void TakeCareOfParametersUpdate() 
	{
        base.TakeCareOfParametersUpdate();
		
		if(m_oGhosts.Count > 0)
		{
			// Grab the first ghost
			MotionBlurGhost rFirstGhost = m_oGhosts[0];
			
			// If the material has changed update the ghost shared material
			TakeCareOfMaterialUpdate(rFirstGhost);
			
			// If the mesh to ghost has changed update the ghosts meshes
			TakeCareOfMeshUpdate();
			
			// Update mesh uv if needed
			if(m_bUpdateMeshUVRequested)
			{
				UpdateMeshUV();
				m_bUpdateMeshUVRequested = false;
			}
		}
	}
	
	/// \brief  If the material has changed update the ghost shared material
	private void TakeCareOfMaterialUpdate(MotionBlurGhost a_rFirstGhost)
	{
        //if (AreMaterialsEqual(m_rMotionBlur.renderer.materials, a_rFirstGhost.renderer.sharedMaterials) == false)
        if (m_rMotionBlur.GetComponent<Renderer>().sharedMaterial != a_rFirstGhost.GetComponent<Renderer>().sharedMaterial)
		{
			foreach(MotionBlurGhost3D rGhost in m_oGhosts)
			{
                rGhost.GetComponent<Renderer>().sharedMaterials = m_rMotionBlur.GetComponent<Renderer>().sharedMaterials;//CopyMaterials(m_rMotionBlur.renderer.materials);
			}
		}
	}

    // Compare Materials
    private bool AreMaterialsEqual(Material[] a_rMaterialsA, Material[] a_rMaterialsB)
    {
        if (a_rMaterialsA.Length != a_rMaterialsB.Length)
        {
            return false;
        }

        foreach (Material rMaterialA in a_rMaterialsA)
        {
            // Contains?
            bool bContains = false;
            foreach (Material rMaterialB in a_rMaterialsB)
            {
                if (rMaterialA == rMaterialB)
                {
                    bContains = true;
                }
            }
            if (bContains == false)
            {
                return false;
            }
        }

        return true;
    }

    // Compare Materials
    private Material[] CopyMaterials(Material[] a_rMaterialsFrom)
    {
        Material[] a_rMaterialsTo;

        a_rMaterialsTo = new Material[a_rMaterialsFrom.Length];
        a_rMaterialsFrom.CopyTo(a_rMaterialsTo, 0);

        return a_rMaterialsTo;
    }
	
	/// \brief  Update the ghost mesh uv accordingly to the mesh to ghost uv
	private void UpdateMeshUV()
	{
		foreach(MotionBlurGhost3D rGhost in m_oGhosts)
		{
			rGhost.UpdateMeshUV(m_rLastSetMeshToGhost);
		}
	}
	
	/// \brief  Update the ghost mesh uv accordingly to the mesh to ghost uv
	public void RequestMeshUVUpdate()
	{
		m_bUpdateMeshUVRequested = true;
	}
		
	/// \brief  If the mesh to ghost has changed update the ghosts meshes
	private void TakeCareOfMeshUpdate()
	{
        if (m_rMeshFilter.sharedMesh != m_rLastSetMeshToGhost)
        {
            m_rLastSetMeshToGhost = m_rMeshFilter.sharedMesh;
            foreach (MotionBlurGhost3D rGhost in m_oGhosts)
            {
                rGhost.SetMeshToGhost(m_rLastSetMeshToGhost);
            }
        }
	}
	
		/// \brief  Create ghosts
    protected override void CreateGhosts(int a_iNumberOfGhostToCreate)
	{
		GameObject rGhostGameObject;
		Renderer rGhostRenderer;
		MotionBlurGhost3D rGhost;
		
		// Get the mesh set to ghost
        m_rLastSetMeshToGhost = m_rMeshFilter.sharedMesh;
		
		// Create the number max of ghosts allowed
		for(int i = 0; i < a_iNumberOfGhostToCreate; i++)
		{
			// Create the ghost game object
            rGhostGameObject = new GameObject(m_rMotionBlur.gameObject + "_Ghost");
			rGhostGameObject.transform.parent = m_rMotionBlur.transform;
			
			// Add a renderer with the same material as the owner
			rGhostRenderer = rGhostGameObject.AddComponent<MeshRenderer>();
            rGhostRenderer.sharedMaterials = m_rMotionBlur.GetComponent<Renderer>().sharedMaterials;// CopyMaterials(m_rMotionBlur.renderer.sharedMaterials);
			rGhostRenderer.enabled = false;
			
			// Add a mesh filter
			rGhostGameObject.AddComponent<MeshFilter>();
			
			// Add the ghost component
			rGhost = rGhostGameObject.AddComponent<MotionBlurGhost3D>();
			
			// Set the mesh to ghost
			rGhost.SetMeshToGhost(m_rLastSetMeshToGhost);
			
			// Add it to the ghosts list
			m_oGhosts.Add(rGhost);
		}
	}

    protected override void PlaceGhostInterpolate(MotionBlurGhost rGhost, TransformSave rTransformSaveBegin, TransformSave rTransformSaveEnd, float fCurrentGhostCurrentTimeRangePositionInPercent)
    {
        base.PlaceGhostInterpolate(rGhost, rTransformSaveBegin, rTransformSaveEnd, fCurrentGhostCurrentTimeRangePositionInPercent);

        // Make sure is visible
        rGhost.Show();
    }
	
	
}
