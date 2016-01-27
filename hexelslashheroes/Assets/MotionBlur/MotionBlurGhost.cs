using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[AddComponentMenu("")]
/// \class  MotionBlurGhost
/// \brief  The base ghost used on the motion blur 2D and 3D effect
public abstract class MotionBlurGhost : MonoBehaviour 
{
    /// \brief  Set opacity
    public abstract void SetOpacity(float a_fOpacity);

	/// \brief  Hide
    public abstract void Hide();

    /// \brief  Show
    public abstract void Show();

    public void SetTransformToGhost(Transform a_rTransformToGhost)
    {
        // Scale
        transform.localScale = Vector3.one;
    }


    // \brief Copy a component into a gameObject
    protected static SpriteRenderer CopyComponent(SpriteRenderer original, GameObject destination)
    {
        System.Type type = original.GetType();
        Component copy = destination.GetComponent<SpriteRenderer>();
        if (copy == null)
            copy = destination.AddComponent(type);

        // Attribute Without accessor
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }

        // Attribute With accessor
        System.Reflection.PropertyInfo[] properties = type.GetProperties();
        foreach (System.Reflection.PropertyInfo property in properties)
        {
            // Debug.Log(property.Name);
            if (property.Name == "material" || property.Name == "materials")
            {
                continue;
            }
            if (property.CanWrite)
                property.SetValue(copy, property.GetValue(original, null), null);
        }

        return copy as SpriteRenderer;
    }
  
}
