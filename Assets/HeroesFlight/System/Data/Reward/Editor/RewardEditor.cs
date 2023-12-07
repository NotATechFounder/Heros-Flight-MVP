using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Reward))]
[CanEditMultipleObjects]
public class RewardEditor : Editor
{
    private SerializedProperty rewardType;
    private SerializedProperty chance;
    private SerializedProperty rewardObject;
    private SerializedProperty amount;
    private SerializedProperty rarity;

    private void OnEnable()
    {
        rewardType = serializedObject.FindProperty("rewardType");
        chance = serializedObject.FindProperty("chance");
        rewardObject = serializedObject.FindProperty("rewardObject");
        amount = serializedObject.FindProperty("amount");
        rarity = serializedObject.FindProperty("rarity");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(rewardType);
        EditorGUILayout.PropertyField(chance);
        EditorGUILayout.PropertyField(rewardObject);
        EditorGUILayout.PropertyField(amount);

        // check if rewardObject is itemso

        //if (rewardObject.objectReferenceValue is not CurrencySO)
        //{
        //    EditorGUILayout.PropertyField(rarity);
        //}

        serializedObject.ApplyModifiedProperties();
    }
}
