using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimatedFollow))]
public class AnimatedFollowEditor : Editor
{
    SerializedProperty curve;

    private void OnEnable()
    {
        curve = serializedObject.FindProperty("curve");
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        serializedObject.Update();
        AnimatedFollow script = (AnimatedFollow)target;
        GUILayoutOption heightOption = GUILayout.Height(EditorGUIUtility.currentViewWidth * .3f);
        GUI.enabled = false;
        EditorGUILayout.PropertyField(curve, new GUIContent("Preview"), heightOption);
    }
}
