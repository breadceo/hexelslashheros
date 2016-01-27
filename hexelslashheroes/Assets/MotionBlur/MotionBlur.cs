using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("MotionBlur/MotionBlur")]
/// \class  MotionBlur
/// \brief  Apply motion blur to the owner. Automatic select 3D or 2D Motion Blur.
public class MotionBlur : MonoBehaviour 
{
    /// The motion blur mode
    public MotionBlurBase.EMotionBlurMode movementMode;

    /// The persistence duration
    public float persistenceDuration = 0.25f;

    /// The number max of ghosts
    public int quality = 25;

    /// The opacity factor
    public float opacity = 1.0f;

    /// Opacity decay
    public MotionBlurBase.EFadeInType fadeType = MotionBlurBase.EFadeInType.Circular;

    /// The local velocity
    public Vector3 localVelocity;

    /// The blur start position offset
    public Vector3 offset;

    /// Do we render color and sprite changement over time, or instantly
    public bool progressiveSpriteColor = false;

    /// Do we render transform changement over time, or instantly
    public bool progressiveScale = true;

    /// The Motion Blur processor
    private MotionBlurBase m_oMotionBlurBase;

    private void Awake()
    {
        if (GetComponent<SpriteRenderer>() != null)
        {
            m_oMotionBlurBase = new MotionBlur2D(this, GetComponent<SpriteRenderer>());
        }
        else if (GetComponent<MeshFilter>() != null)
        {
            m_oMotionBlurBase = new MotionBlur3D(this, GetComponent<MeshFilter>());
        }
        else
        {
            Debug.LogError("The motion blur cannot be apply on an object without renderer.");
        }

        m_oMotionBlurBase.Awake();
    }

     /// \brief  On Disable
    private void OnDisable()
    {
        m_oMotionBlurBase.OnDisable();
    }

    /// \brief  Late Update
    private void LateUpdate()
    {
        m_oMotionBlurBase.LateUpdate();
    }
}