using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(AdvanceButton))]
[CanEditMultipleObjects]
public class AdvancedButtonEditor : ButtonEditor
{
    private SerializedProperty buttonType;
    private SerializedProperty onClickToggle;

    protected override void OnEnable()
    {
        base.OnEnable();
        buttonType = serializedObject.FindProperty("buttonType");
        onClickToggle = serializedObject.FindProperty("onClickToggle");
    }

    public override void OnInspectorGUI()
    {
        AdvanceButton targetMyButton = (AdvanceButton)target;

        base.OnInspectorGUI();

        serializedObject.Update();
        EditorGUILayout.PropertyField(buttonType);
        EditorGUILayout.PropertyField(onClickToggle);
        serializedObject.ApplyModifiedProperties();
    }
}
