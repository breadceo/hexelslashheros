using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("")]
/// \class  MotionBlurGhost
/// \brief  The ghost used on the motion blur effect
public class MotionBlurGhost2D : MotionBlurGhost 
{
    // The sprite renderer
    public SpriteRenderer m_rSpriteRendererFilter;

    // Previous color. Used if hided and showed.
    private Color m_rPreviousColor;
	
	/// \brief  Set mesh to ghost
	public void SetSpriteRendererToGhost(SpriteRenderer a_rMeshToGhost)
	{
        m_rSpriteRendererFilter = CopyComponent(a_rMeshToGhost, gameObject);
	}

    /// \brief  Set opacity
    public override void SetOpacity(float a_fOpacity)
	{
        m_rPreviousColor = m_rSpriteRendererFilter.color;
        Color newColor = m_rSpriteRendererFilter.color;
        newColor.a = a_fOpacity;
        m_rSpriteRendererFilter.color = newColor;
	}
	
	/// \brief  Hide
	public override void Hide()
	{
        SetOpacity(0);
	}

    /// \brief  Show
    public override void Show()
    {
        SetOpacity(m_rPreviousColor.a);
    }

  
}