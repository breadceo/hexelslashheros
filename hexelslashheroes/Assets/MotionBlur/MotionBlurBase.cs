using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// \class  MotionBlur
/// \brief  Base motion blur for 3D en 2D object.
public abstract class MotionBlurBase
{
    /// \class  TransformSave
    /// \brief  Use this to save and load transform
    protected class TransformSave
    {

        public Vector3 position;
        public Quaternion rotation;
        public Vector3 lossyScale;

        /// Time of the save
        public float saveTime;

        /// \brief  Constructor
        public TransformSave(Transform a_rTransform)
        {
            SaveFromTransform(a_rTransform);
        }

        /// \brief  Save From Transform
        public void SaveFromTransform(Transform a_rTransform)
        {
            position = a_rTransform.position;
            rotation = a_rTransform.rotation;
            lossyScale = a_rTransform.lossyScale;

            // Remember the save time
            saveTime = Time.time;
        }

        /// \brief  Load the transform save to a transform 
        public virtual void LoadToTransform(Transform a_rTransform)
        {
            // Position
            a_rTransform.position = position;

            // Rotation
            a_rTransform.rotation = rotation;

            // Scale
            Transform rParentTransformSave = a_rTransform.parent;
            a_rTransform.parent = null;
            a_rTransform.localScale = lossyScale;
            a_rTransform.parent = rParentTransformSave;
        }

        /// \brief  Load an interpolated transform 
        static public void LoadInterpolatedTransform(Transform a_rTransform, TransformSave a_rTransformBegin, TransformSave a_rTransformEnd, float a_fInterpolationPercent)
        {
            // Linear interpolation the position between begin and end
            a_rTransform.position = a_rTransformBegin.position + (a_rTransformEnd.position - a_rTransformBegin.position) * a_fInterpolationPercent;

            // Rotation
            a_rTransform.rotation = a_rTransformBegin.rotation;

            // Scale
            Transform rParentTransformSave = a_rTransform.parent;
            a_rTransform.parent = null;
            a_rTransform.localScale = a_rTransformBegin.lossyScale;
            a_rTransform.parent = rParentTransformSave;
        }
    }



    /// \brief  Motion Blur mode
    public enum EMotionBlurMode
    {
        World,
        Local,
    }

    /// \brief  Motion Blur mode
    public enum EFadeInType
    {
        Linear,
        Circular,
        Exponential,
    }

    public MotionBlur m_rMotionBlur;

    /// The transform history
    protected List<TransformSave> m_oTransformHistory = new List<TransformSave>();

    /// The ghosts renderer
    protected List<MotionBlurGhost> m_oGhosts = new List<MotionBlurGhost>();

    public MotionBlurBase(MotionBlur b)
    {
        m_rMotionBlur = b;
    }

    /// \brief  Awake
    public void Awake()
    {
        CreateGhosts(m_rMotionBlur.quality);
    }
    /// \brief  Update Parameters
    protected virtual void TakeCareOfParametersUpdate()
    {
        // Update the number of ghosts
        int iCurrentNumberOfGhost = m_oGhosts.Count;
        int iWantedNumberOfGhost = m_rMotionBlur.quality;
        if (iWantedNumberOfGhost > iCurrentNumberOfGhost)
        {
			// Destroy current ghost for order in layer purpose
            DestroyGhosts(iCurrentNumberOfGhost);
			
            // Create new ghosts
            CreateGhosts(iWantedNumberOfGhost);
        }
        else if (iWantedNumberOfGhost < iCurrentNumberOfGhost)
        {
            // Remove the ghosts in excess
            DestroyGhosts(iCurrentNumberOfGhost - iWantedNumberOfGhost);
        }

    }

    /// \brief  On Disable
    public void OnDisable()
    {
        // Hide the ghosts
        HideGhosts();
        // Clear history
        m_oTransformHistory.Clear();
    }

    /// \brief  Late Update
    public void LateUpdate()
    {
        // Take care of the parameters update
        TakeCareOfParametersUpdate();

        // Update the transform history
        UpdateTransformHistory();

        // Update the ghosts
        UpdateGhosts();
    }


    /// \brief  Destroy ghosts
    private void DestroyGhosts(int a_iNumberOfGhostToDestroy)
    {
        a_iNumberOfGhostToDestroy = Mathf.Clamp(a_iNumberOfGhostToDestroy, 0, m_oGhosts.Count);
        for (int i = 0; i < a_iNumberOfGhostToDestroy; i++)
        {
            UnityEngine.Object.Destroy(m_oGhosts[i].gameObject);
        }
        m_oGhosts.RemoveRange(0, a_iNumberOfGhostToDestroy);
    }

    /// \brief  Update the transform history
    protected virtual void UpdateTransformHistory()
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
        m_oTransformHistory.Add(new TransformSave(m_rMotionBlur.transform));

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

    /// \brief  Update ghosts
    protected void ApplyGhostFade(MotionBlurGhost a_rGhost, float a_fPercentFade)
    {
        a_rGhost.SetOpacity(MotionBlurInterpolate.Ease(GetFadeEaseType())(0.0f, 1.0f, 1.0f - a_fPercentFade, 1.0f)
                           * m_rMotionBlur.opacity);
    }

    /// \brief  Get ease type
    private MotionBlurInterpolate.EaseType GetFadeEaseType()
    {
        switch (m_rMotionBlur.fadeType)
        {
            case EFadeInType.Linear:
                {
                    return MotionBlurInterpolate.EaseType.Linear;
                }

            case EFadeInType.Circular:
                {
                    return MotionBlurInterpolate.EaseType.EaseInCirc;
                }
            case EFadeInType.Exponential:
                {
                    return MotionBlurInterpolate.EaseType.EaseInExpo;
                }

            default:
                {
                    return MotionBlurInterpolate.EaseType.EaseInCirc;
                }
        }
    }

    /// \brief  Update ghosts
    protected void UpdateGhosts()
    {
        // Keep trace of the current ghost
        int iCurrentGhostIndex = 0;

        switch (m_rMotionBlur.movementMode)
        {
            case EMotionBlurMode.World:
                {
                    UpdateGhostWorldMode(ref iCurrentGhostIndex);
                }
                break;

            case EMotionBlurMode.Local:
                {
                    UpdateGhostLocalMode(ref iCurrentGhostIndex);
                }
                break;
        }

        HideUnusedGhost(ref iCurrentGhostIndex);
    }

    /// \brief  Update world motion blur
    private void HideUnusedGhost(ref int a_iCurrentGhostIndex)
    {
        // Loop through the unused Ghost and Ensure they are hidden
        int iGhostCount = m_oGhosts.Count;
        for (; a_iCurrentGhostIndex < iGhostCount; a_iCurrentGhostIndex++)
        {
            // Grab the next unused ghost
            MotionBlurGhost rGhost = m_oGhosts[a_iCurrentGhostIndex];

            // Make sure is invisible
            rGhost.Hide();
        }
    }

    /// \brief  Update world motion blur
    private void UpdateGhostLocalMode(ref int a_iCurrentGhostIndex)
    {
        // Simulate the transform history accordingly to the local velocity
        TransformSave rTransformSaveBegin = new TransformSave(m_rMotionBlur.transform);
        TransformSave rTransformSaveEnd = new TransformSave(m_rMotionBlur.transform);

        rTransformSaveEnd.position += m_rMotionBlur.persistenceDuration * m_rMotionBlur.localVelocity;
        rTransformSaveEnd.saveTime -= m_rMotionBlur.persistenceDuration;

        PlaceGhost(rTransformSaveBegin, rTransformSaveEnd, ref a_iCurrentGhostIndex);
    }

    /// \brief  Create ghosts
    protected abstract void CreateGhosts(int a_iNumberOfGhostToCreate);

    /// \brief Place Ghost
    protected virtual float PlaceGhost(TransformSave rTransformSaveBegin, TransformSave rTransformSaveEnd, ref int a_iCurrentGhostIndex)
    {
        // Compute the time between two ghost
        float fTimeBetweenTwoGhosts = m_rMotionBlur.persistenceDuration / m_rMotionBlur.quality;

        // Get the range time extremity
        float fCurrentRangeTimeBegin = rTransformSaveBegin.saveTime;
        float fCurrentRangeTimeEnd = rTransformSaveEnd.saveTime;

        // Compute the current time range duration 
        float fCurrentTimeRangeDuration = fCurrentRangeTimeBegin - fCurrentRangeTimeEnd;

        // Compute the current ghost backward time
        float fCurrentGhostBackwardTime = (a_iCurrentGhostIndex) * fTimeBetweenTwoGhosts;

        // Compute the current Ghost time
        float fCurrentTime = Time.time;
        float fCurrentGhostTime = fCurrentTime - fCurrentGhostBackwardTime;

        // Compute the remaining time since first ghost before the current time range end
        float fRemainingTimeSinceFirstGhostBeforeEndOfCurrentTimeRange = fCurrentGhostTime - fCurrentRangeTimeEnd;

        // Compute the number of ghost to place
        int iNumberOfGhostToPlaceInTheCurrentTimeRange = 1 + Mathf.FloorToInt(fRemainingTimeSinceFirstGhostBeforeEndOfCurrentTimeRange / fTimeBetweenTwoGhosts);
        iNumberOfGhostToPlaceInTheCurrentTimeRange = Mathf.Clamp(iNumberOfGhostToPlaceInTheCurrentTimeRange, 0, m_oGhosts.Count - a_iCurrentGhostIndex);

        // Place the number of ghost needed
        for (int k = 0; k < iNumberOfGhostToPlaceInTheCurrentTimeRange; k++)
        {
            // Update the current ghost backward time
            fCurrentGhostBackwardTime = (a_iCurrentGhostIndex) * fTimeBetweenTwoGhosts;

            // Update the current Ghost time
            fCurrentGhostTime = fCurrentTime - fCurrentGhostBackwardTime;

            // Compute at which percent of the current time range is placed the current ghost
            float fCurrentGhostCurrentTimeRangePositionInPercent = (fCurrentRangeTimeBegin - fCurrentGhostTime) / fCurrentTimeRangeDuration;

            // Grab the current ghost
            MotionBlurGhost rGhost = m_oGhosts[a_iCurrentGhostIndex];

            // Place the ghost
            fCurrentGhostCurrentTimeRangePositionInPercent = Mathf.Max(fCurrentGhostCurrentTimeRangePositionInPercent, 0.0f);
            PlaceGhostInterpolate(rGhost, rTransformSaveBegin, rTransformSaveEnd, fCurrentGhostCurrentTimeRangePositionInPercent);

            // Offset the ghost
            rGhost.transform.position += m_rMotionBlur.offset;

            // Fade out the ghost => the older it is the more transparent
            ApplyGhostFade(rGhost, fCurrentGhostBackwardTime / m_rMotionBlur.persistenceDuration);

            // Pass to the next ghost
            a_iCurrentGhostIndex++;
        }

        // Increment elapsed time since last ghost
        return fCurrentGhostTime - fCurrentRangeTimeEnd;
    }

    protected virtual void PlaceGhostInterpolate(MotionBlurGhost rGhost, TransformSave rTransformSaveBegin, TransformSave rTransformSaveEnd, float fCurrentGhostCurrentTimeRangePositionInPercent)
    {
        TransformSave.LoadInterpolatedTransform(rGhost.transform, rTransformSaveBegin, rTransformSaveEnd, fCurrentGhostCurrentTimeRangePositionInPercent);

        if (!m_rMotionBlur.progressiveScale)
        {
            rGhost.SetTransformToGhost(m_rMotionBlur.transform);
        }

    }

    /// \brief  Update world motion blur
    private void UpdateGhostWorldMode(ref int a_iCurrentGhostIndex)
    {
        // Compute the time between two ghost
        float fTimeBetweenTwoGhosts = m_rMotionBlur.persistenceDuration / m_rMotionBlur.quality;

        // Loop through the save transform from the newest to the oldest
        int iHistoryLength = m_oTransformHistory.Count;
        float fElapsedTimeSinceLastGhost = 0.0f;
        float fCurrentTimeRangeDuration;
        TransformSave rTransformSaveBegin;
        TransformSave rTransformSaveEnd;
        float fCurrentRangeTimeBegin;
        float fCurrentRangeTimeEnd;
        for (int i = iHistoryLength - 2; i >= 0; i--)
        {
            // Grab the current transform save
            rTransformSaveBegin = m_oTransformHistory[i + 1];
            rTransformSaveEnd = m_oTransformHistory[i];

            // Get the range time extremity
            fCurrentRangeTimeBegin = rTransformSaveBegin.saveTime;
            fCurrentRangeTimeEnd = rTransformSaveEnd.saveTime;

            // Compute the current time range duration 
            fCurrentTimeRangeDuration = fCurrentRangeTimeBegin - fCurrentRangeTimeEnd;

            // If we have to place at least one ghost in this time range
            if (fElapsedTimeSinceLastGhost + fCurrentTimeRangeDuration >= fTimeBetweenTwoGhosts)
            {
                fElapsedTimeSinceLastGhost = PlaceGhost(rTransformSaveBegin, rTransformSaveEnd, ref a_iCurrentGhostIndex);
            }
            else
            {
                // Increment elapsed time since last ghost
                fElapsedTimeSinceLastGhost += fCurrentTimeRangeDuration;
            }
        }
    }

    /// \brief  Hide all the ghosts
    void HideGhosts()
    {
        // Loop through all the ghosts and hide them
        foreach (MotionBlurGhost rGhost in m_oGhosts)
        {
            // Make sure is invisible
            rGhost.Hide();
        }
    }
}
