using System;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(StatSO), true)]
public class StatSOEditor : Editor
{
    public SerializedProperty animationCurveProperty;
    public SerializedProperty curveTypeProperty;
    public SerializedProperty minLevelProperty;
    public SerializedProperty minValueProperty;
    public SerializedProperty maxLevelProperty;
    public SerializedProperty maxValueProperty;

    StatSO statSO;

    private void OnEnable()
    {
        animationCurveProperty = serializedObject.FindProperty("statCurve").FindPropertyRelative("animationCurve");
        curveTypeProperty = serializedObject.FindProperty("statCurve").FindPropertyRelative("curveType");
        minLevelProperty = serializedObject.FindProperty("statCurve").FindPropertyRelative("minLevel");
        minValueProperty = serializedObject.FindProperty("statCurve").FindPropertyRelative("minValue");
        maxLevelProperty = serializedObject.FindProperty("statCurve").FindPropertyRelative("maxLevel");
        maxValueProperty = serializedObject.FindProperty("statCurve").FindPropertyRelative("maxValue");
        statSO = (StatSO)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(curveTypeProperty);

        EditorGUILayout.PropertyField(animationCurveProperty);

        if (statSO.StatCurve.curveType != CurveType.Custom)
        {
            EditorGUILayout.PropertyField(minLevelProperty);
            EditorGUILayout.PropertyField(minValueProperty);
            EditorGUILayout.PropertyField(maxLevelProperty);
            EditorGUILayout.PropertyField(maxValueProperty);
        }

        serializedObject.ApplyModifiedProperties();
    }

    public void HandleChangeCheck(SerializedProperty serializedProperty, Action OnChange = null, bool includeChildren = false)
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedProperty, includeChildren);
        if (EditorGUI.EndChangeCheck())
        {
            OnChange?.Invoke();
        }
    }
}