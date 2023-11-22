using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(CustomAnimationCurve))]
public class CustomAnimationCurveEditor : PropertyDrawer
{
    public SerializedProperty animationCurveProperty;
    public SerializedProperty curveTypeProperty;
    public SerializedProperty minLevelProperty;
    public SerializedProperty minValueProperty;
    public SerializedProperty maxLevelProperty;
    public SerializedProperty maxValueProperty;
    CustomAnimationCurve customAnimationCurve;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        customAnimationCurve = (CustomAnimationCurve)fieldInfo.GetValue(property.serializedObject.targetObject);
        animationCurveProperty = property.FindPropertyRelative("animationCurve");
        curveTypeProperty = property.FindPropertyRelative("curveType");
        minLevelProperty = property.FindPropertyRelative("minLevel");
        minValueProperty = property.FindPropertyRelative("minValue");
        maxLevelProperty = property.FindPropertyRelative("maxLevel");
        maxValueProperty = property.FindPropertyRelative("maxValue");

        EditorGUI.BeginProperty(position, label, property);

        var animationCurveRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        var curveTypeRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        var minLevelRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight);
        var minValueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight);
        var maxLevelRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 4, position.width, EditorGUIUtility.singleLineHeight);
        var maxValueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 5, position.width, EditorGUIUtility.singleLineHeight);


        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(animationCurveRect, animationCurveProperty, label);
        EditorGUI.PropertyField (curveTypeRect, curveTypeProperty, GUIContent.none);
        EditorGUI.PropertyField(minLevelRect, minLevelProperty, GUIContent.none);
        EditorGUI.PropertyField(minValueRect, minValueProperty, GUIContent.none);
        EditorGUI.PropertyField(maxLevelRect, maxLevelProperty, GUIContent.none);
        EditorGUI.PropertyField(maxValueRect, maxValueProperty, GUIContent.none);

        EditorGUI.EndProperty();

        if (EditorGUI.EndChangeCheck())
        {
            UpdateCurveValues();
        }

        EditorGUI.EndProperty();
    }

    private void UpdateCurveValues()
    {
        CurveType curveType = (CurveType)curveTypeProperty.enumValueIndex;
        if (curveType != CurveType.Custom)
        {
            customAnimationCurve.UpdateCurve();
        }
    }
}