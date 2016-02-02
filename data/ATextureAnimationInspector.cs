using UnityEngine;
using System.Collections;
using UnityEditor;
[CanEditMultipleObjects]
[CustomEditor(typeof(ATextureAnimation), true)]
public class ATextureAnimationInspector : Editor
{
    ATextureAnimation anim
    {
        get
        {
            return (ATextureAnimation)target;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        anim.UpdateInEditor();
    }
}
