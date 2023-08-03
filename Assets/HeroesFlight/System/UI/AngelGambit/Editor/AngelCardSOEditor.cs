
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CanEditMultipleObjects]
//[CustomEditor(typeof(AngelCardSO))]
public class AngelCardSOEditor : Editor
{
    private SerializedProperty cardName;
    private SerializedProperty descriptionTemplateProperty;
    private SerializedProperty cardDescriptions;
    private SerializedProperty cardImage;
    private SerializedProperty cardType;
    private SerializedProperty affterEffectBonus;
    private SerializedProperty effectsProperty;
    private SerializedProperty chanceProperty;

    private bool showDescriptions = false;
    private bool showAfterEffect = false;
    private bool showEffects = false;
    public List<bool> showEffectValues = new List<bool>();
    private AngelCardSO targetObject;

    private void OnEnable()
    {
        targetObject = (AngelCardSO)target;
        cardName = serializedObject.FindProperty("cardName");
        descriptionTemplateProperty = serializedObject.FindProperty("descriptionTemplate");
        cardDescriptions = serializedObject.FindProperty("cardDescriptions");
        cardImage = serializedObject.FindProperty("cardImage");
        cardType = serializedObject.FindProperty("cardType");
        affterEffectBonus = serializedObject.FindProperty("affterBonusEffect");
        effectsProperty = serializedObject.FindProperty("effects");
        chanceProperty = serializedObject.FindProperty("chance");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(cardName);
        DisplayTierDescriptions();
        EditorGUILayout.PropertyField(cardImage);
        EditorGUILayout.PropertyField(cardType);

        DisplayAfterEffect();
        DisplayEffects();

        EditorGUILayout.PropertyField(chanceProperty);

        serializedObject.ApplyModifiedProperties();
    }

    public void DisplayTierDescriptions()
    {
        EditorGUILayout.PropertyField(descriptionTemplateProperty);

        showDescriptions = EditorGUILayout.Foldout(showDescriptions, "Tier Descriptions", true);

        if (showDescriptions && GUILayout.Button("Add Description"))
        {
            if (cardDescriptions.arraySize >= 6)
            {
                return;
            }
            cardDescriptions.arraySize++;
        }

        if (showDescriptions)
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < cardDescriptions.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(cardDescriptions.GetArrayElementAtIndex(i), new GUIContent("Tier " + (i + 1)));
                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    cardDescriptions.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }
    }

    public void DisplayAfterEffect()
    {
        GUIStyle customBoxStyle = new GUIStyle(GUI.skin.window);
        customBoxStyle.normal.background = MakeBackgroundTexture(Color.black);

        GUILayoutOption[] options = { GUILayout.MaxWidth(300), GUILayout.MinWidth(200) };
        EditorGUILayout.BeginVertical(options);

        showAfterEffect = EditorGUILayout.Foldout(showAfterEffect, "AffterBonusEffect ");
        if (showAfterEffect)
        {
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginVertical(customBoxStyle);

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(affterEffectBonus.FindPropertyRelative("effect"));
            EditorGUILayout.PropertyField(affterEffectBonus.FindPropertyRelative("targetType"));
            EditorGUILayout.EndVertical();

            DrawCustomTierValuesArray(targetObject.AffterBonusEffect);
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);
    }

    private void DisplayEffects()
    {
        GUIStyle customBoxStyle = new GUIStyle(GUI.skin.window);
        customBoxStyle.normal.background = MakeBackgroundTexture(Color.black);

        showEffects = EditorGUILayout.Foldout(showEffects, "Effects", true);
        if (!showEffects) return;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Effect"))
        {
            effectsProperty.arraySize++;
        }

        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < targetObject.Effects.Length; i++)
        {
            GUILayoutOption[] options = { GUILayout.MaxWidth(300), GUILayout.MinWidth(200) };
            EditorGUILayout.BeginVertical("box", options);

            targetObject.Effects[i].showStatEffect = EditorGUILayout.Foldout(targetObject.Effects[i].showStatEffect, "Effect " + (i + 1), true);
            if (targetObject.Effects[i].showStatEffect)
            {
                EditorGUILayout.Space(10);

                EditorGUILayout.BeginVertical(customBoxStyle);

                EditorGUILayout.BeginVertical();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("effects").GetArrayElementAtIndex(Array.IndexOf(targetObject.Effects, targetObject.Effects[i])).FindPropertyRelative("statKey"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("effects").GetArrayElementAtIndex(Array.IndexOf(targetObject.Effects, targetObject.Effects[i])).FindPropertyRelative("effect"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("effects").GetArrayElementAtIndex(Array.IndexOf(targetObject.Effects, targetObject.Effects[i])).FindPropertyRelative("targetType"));
                EditorGUILayout.EndVertical();

                DrawCustomTierValuesArray(targetObject.Effects[i]);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);
        }

        if (GUILayout.Button("Remove Effect"))
        {
            if (effectsProperty.arraySize == 0)
            {
                return;
            }
            effectsProperty.arraySize--;
        }
    }

    private void DrawCustomTierValuesArray(StatEffect statEffect)
    {
        statEffect.showTierValues = EditorGUILayout.Foldout(statEffect.showTierValues, "Tier Values", true);
        if (!statEffect.showTierValues) return;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Value"))
        {
            if (statEffect.tierValues.Length >= 6)
            {
                return;
            }
            ArrayUtility.Add(ref statEffect.tierValues, 0);
        }

        if (GUILayout.Button("Remove Effect"))
        {
            if (statEffect.tierValues.Length == 0)
            {
                return;
            }
            ArrayUtility.Remove(ref statEffect.tierValues, statEffect.tierValues[^1]);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel++; // Indent the following elements

        for (int i = 0; i < statEffect.tierValues.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            int tierNumber = i + 1;
            string tierLabel = "Tier " + tierNumber;
            statEffect.tierValues[i] = EditorGUILayout.FloatField(tierLabel, statEffect.tierValues[i]);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUI.indentLevel--; // Reduce the indentation level
    }

    private Texture2D MakeBackgroundTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
}
