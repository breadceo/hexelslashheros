
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// \class  MotionBlur
/// \brief  Apply motion blur to the 2D owner.
public class MotionBlur2D : MotionBlurBase
{
    /// \class  TransformSave
    /// \brief  Use this to save and load transform
    protected class TransformSave2D : TransformSave
    {
        Color color;
        Sprite sprite;
        int sortingLayerID;
        int sortingOrder;

        /// \brief  Constructor
        public TransformSave2D(Transform a_rTransform, SpriteRenderer a_rRenderer)
            : base(a_rTransform)
        {
            color = a_rRenderer.color;
            sprite = a_rRenderer.sprite;
            sortingOrder = a_rRenderer.sortingOrder;
            sortingLayerID = a_rRenderer.sortingLayerID;
        }

        /// \brief  Load an interpolated transform 
        static public void LoadInterpolatedTransform(SpriteRenderer a_rSpriteRenderer, TransformSave2D a_rTransformBegin, TransformSave2D a_rTransformEnd, float a_fInterpolationPercent)
        {
            a_rSpriteRenderer.color = a_rTransformEnd.color;
            a_rSpriteRenderer.sprite = a_rTransformEnd.sprite;
            a_rSpriteRenderer.sortingLayerID = a_rTransformEnd.sortingLayerID;
            a_rSpriteRenderer.sortingOrder = a_rTransformEnd.sortingOrder;
        }
    }

    /// The owner sprite renderer
    private SpriteRenderer m_rSpriteRenderer;

    public MotionBlur2D(MotionBlur b, SpriteRenderer sr)
        : base(b)
    {
        m_rSpriteRenderer = sr;
    }


    /// \brief  Create ghosts
    protected override void CreateGhosts(int a_iNumberOfGhostToCreate)
    {
        GameObject rGhostGameObject;
        MotionBlurGhost2D rGhost;

        // Create the number max of ghosts allowed
        for (int i = 0; i < a_iNumberOfGhostToCreate; i++)
        {
            // Create the ghost game object
            rGhostGameObject = new GameObject(m_rMotionBlur.gameObject + "_Ghost");
            rGhostGameObject.transform.parent = m_rMotionBlur.transform;

            // Add the ghost component
            rGhost = rGhostGameObject.AddComponent<MotionBlurGhost2D>();
            rGhost.SetSpriteRendererToGhost(m_rSpriteRenderer);
            rGhost.Hide();

            // Add it to the ghosts list
            m_oGhosts.Add(rGhost);
        }

        // Order Ghosts layout
        foreach (MotionBlurGhost2D m in m_oGhosts)
        {
            m.GetComponent<Renderer>().enabled = false;
        }
        m_rMotionBlur.GetComponent<Renderer>().enabled = false;

        m_rMotionBlur.GetComponent<Renderer>().enabled = true;
        foreach (MotionBlurGhost2D m in m_oGhosts)
        {
            m.GetComponent<Renderer>().enabled = true;
        }
    }

    /// \brief  Update the transform history
    protected override void UpdateTransformHistory()
    {
        // If there is a additive scolling speed
        Vector3 f3ScrollingMovement = m_rMotionBlur.localVelocity * Time.deltaTime;
        if (f3ScrollingMovement != Vector3.zero)
        {
            // Move the history transform of the scrolling movement
            foreach (TransformSave rTransformSave in m_oTransformHistory)
            {
                rTransformSave.position += f3ScrollingMovement;
            }
        }

        // Add the current transform to the history
        m_oTransformHistory.Add(new TransformSave2D(m_rMotionBlur.transform, m_rSpriteRenderer));

        // Remove all the transform older than the persistance duration
        TransformSave rOlderSave;
        float fMinSaveTime = Time.time - m_rMotionBlur.persistenceDuration;
        while (m_oTransformHistory.Count > 2)
        {
            // Grab the older save
            rOlderSave = m_oTransformHistory[1];

            // If this time is passed
            if (rOlderSave.saveTime < fMinSaveTime)
            {
                // Remove the save
                m_oTransformHistory.RemoveAt(0);
            }
            else
            {
                // If not stop the removal
                break;
            }
        }
    }


    protected override void PlaceGhostInterpolate(MotionBlurGhost rGhost, TransformSave rTransformSaveBegin, TransformSave rTransformSaveEnd, float fCurrentGhostCurrentTimeRangePositionInPercent)
    {
        base.PlaceGhostInterpolate( rGhost, rTransformSaveBegin,  rTransformSaveEnd,  fCurrentGhostCurrentTimeRangePositionInPercent);

        // Render and transform smooth update or not
        if (m_rMotionBlur.progressiveSpriteColor)
        {
            TransformSave2D.LoadInterpolatedTransform( ((MotionBlurGhost2D)rGhost).m_rSpriteRendererFilter, (TransformSave2D)rTransformSaveBegin, (TransformSave2D)rTransformSaveEnd, fCurrentGhostCurrentTimeRangePositionInPercent);
        }
        else
        {
            ((MotionBlurGhost2D)rGhost).SetSpriteRendererToGhost(m_rSpriteRenderer);
        }
    }


   
}


