using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.NPC.Enum;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GameAreaModel))]
public class GameAreaModelEditor : Editor
{
    SerializedProperty areaNameProperty;
    SerializedProperty heroProgressionExpEarnedPerKillProperty;
    SerializedProperty portalPrefabProperty;
    SerializedProperty crystalPrefabProperty;
    SerializedProperty angelsGambitLevelProperty;
    SerializedProperty worldBossProperty;
    SerializedProperty bossMusicKeyProperty;
    SerializedProperty spawnModelProperty;
    SerializedProperty mobDifficultyProperty;
    SerializedProperty mobDifficultyArray;
    SerializedProperty mobDropTableArray;

    SerializedProperty bossDropProperty;
    SerializedProperty timeStopRestoreSpeedProperty;
    SerializedProperty timeStopDurationProperty;

    bool difficultyFoldout = false;
    Dictionary<int, bool> foldoutDictionary = new Dictionary<int, bool>();

    private void OnEnable()
    {
        areaNameProperty = serializedObject.FindProperty("areaName");
        heroProgressionExpEarnedPerKillProperty = serializedObject.FindProperty("heroProgressionExpEarnedPerKill");
        portalPrefabProperty = serializedObject.FindProperty("portalPrefab");
        crystalPrefabProperty = serializedObject.FindProperty("crystalPrefab");
        angelsGambitLevelProperty = serializedObject.FindProperty("angelsGambitLevel");
        worldBossProperty = serializedObject.FindProperty("worldBoss");
        bossMusicKeyProperty = serializedObject.FindProperty("bossMusicKey");
        spawnModelProperty = serializedObject.FindProperty("spawnModel");
        mobDifficultyProperty = serializedObject.FindProperty("mobDifficulty");
        mobDifficultyArray = mobDifficultyProperty.FindPropertyRelative("mobDifficulties");
        mobDropTableArray = serializedObject.FindProperty("mobDropTable");
        bossDropProperty = serializedObject.FindProperty("bossDrop");
        timeStopRestoreSpeedProperty = serializedObject.FindProperty("timeStopRestoreSpeed");
        timeStopDurationProperty = serializedObject.FindProperty("timeStopDuration");

        for (int i = 0; i < mobDifficultyArray.arraySize; i++)
        {
            foldoutDictionary.Add(i, false);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(areaNameProperty);
        EditorGUILayout.PropertyField(heroProgressionExpEarnedPerKillProperty);
        EditorGUILayout.PropertyField(portalPrefabProperty);
        EditorGUILayout.PropertyField(crystalPrefabProperty);
        EditorGUILayout.PropertyField(angelsGambitLevelProperty);
        EditorGUILayout.PropertyField(worldBossProperty);
        EditorGUILayout.PropertyField(bossDropProperty);
        EditorGUILayout.PropertyField(bossMusicKeyProperty);
        EditorGUILayout.PropertyField(spawnModelProperty);

        DisplayDiffiulties();

        EditorGUILayout.PropertyField(mobDropTableArray);

        EditorGUILayout.PropertyField(timeStopRestoreSpeedProperty);
        EditorGUILayout.PropertyField(timeStopDurationProperty);

        serializedObject.ApplyModifiedProperties();
    }

    public void DisplayDiffiulties()
    {
        difficultyFoldout = EditorGUILayout.Foldout(difficultyFoldout, "Mob Difficulties");
        if (!difficultyFoldout) return;

        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.LabelField("Mob Difficulties", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add New Difficulty"))
        {
            foldoutDictionary.Add(mobDifficultyArray.arraySize, false);
            mobDifficultyArray.InsertArrayElementAtIndex(mobDifficultyArray.arraySize);
        }

        if (GUILayout.Button("Remove Last Difficulty"))
        {
            foldoutDictionary.Remove(mobDifficultyArray.arraySize - 1);
            mobDifficultyArray.DeleteArrayElementAtIndex(mobDifficultyArray.arraySize - 1);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        for (int i = 0; i < mobDifficultyArray.arraySize; i++)
        {
            SerializedProperty mobDifficulty = mobDifficultyArray.GetArrayElementAtIndex(i);
            SerializedProperty enemyType = mobDifficulty.FindPropertyRelative("enemyType");

            if (!foldoutDictionary.ContainsKey(i))
            {
                foldoutDictionary.Add(i, false);
            }

            foldoutDictionary[i] = EditorGUILayout.Foldout(foldoutDictionary[i], Enum.GetName(typeof(EnemyType), enemyType.enumValueIndex) + " Difficulty ");

            if (!foldoutDictionary[i])
            {
                continue;
            }

            SerializedProperty healthStat = mobDifficulty.FindPropertyRelative("healthStat");
            SerializedProperty damageStat = mobDifficulty.FindPropertyRelative("damageStat");

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.PropertyField(enemyType);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Health", EditorStyles.boldLabel);
            DrawData(healthStat, i);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Damage", EditorStyles.boldLabel);
            DrawData(damageStat, i);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
        }

        EditorGUILayout.EndVertical();
    }

    public void DrawData(SerializedProperty customAnimationCurveProperty, int index)
    {
        SerializedProperty animationCurveProperty = customAnimationCurveProperty.FindPropertyRelative("animationCurve");
        SerializedProperty curveTypeProperty = customAnimationCurveProperty.FindPropertyRelative("curveType");
        SerializedProperty minLevelProperty = customAnimationCurveProperty.FindPropertyRelative("minLevel");
        SerializedProperty minValueProperty = customAnimationCurveProperty.FindPropertyRelative("minValue");
        SerializedProperty maxLevelProperty = customAnimationCurveProperty.FindPropertyRelative("maxLevel");
        SerializedProperty maxValueProperty = customAnimationCurveProperty.FindPropertyRelative("maxValue");

        HandleChangeCheck(curveTypeProperty, ()=>
        {
            if (curveTypeProperty.enumValueIndex != (int)CurveType.Custom)
            {
                UpdateCurve(animationCurveProperty, (CurveType)curveTypeProperty.enumValueIndex, minLevelProperty, minValueProperty, maxLevelProperty, maxValueProperty);
            }
        });

        HandleChangeCheck(animationCurveProperty,
        ()=>
        {
            if (curveTypeProperty.enumValueIndex != (int)CurveType.Custom)
            {
                UpdateValues(animationCurveProperty, minLevelProperty, minValueProperty, maxLevelProperty, maxValueProperty);
            }
        });

        if (curveTypeProperty.enumValueIndex != (int)CurveType.Custom)
        {
            HandleChangeCheck(minLevelProperty,
            () =>
            {
                UpdateCurve(animationCurveProperty, (CurveType)curveTypeProperty.enumValueIndex, minLevelProperty, minValueProperty, maxLevelProperty, maxValueProperty);
            }); 

            HandleChangeCheck(minValueProperty,
            () =>
            {
                UpdateCurve(animationCurveProperty, (CurveType)curveTypeProperty.enumValueIndex, minLevelProperty, minValueProperty, maxLevelProperty, maxValueProperty);
            });

            HandleChangeCheck(maxLevelProperty,
            () =>
            {
                UpdateCurve(animationCurveProperty, (CurveType)curveTypeProperty.enumValueIndex, minLevelProperty, minValueProperty, maxLevelProperty, maxValueProperty);
            });

            HandleChangeCheck(maxValueProperty, 
            () =>
            {
                UpdateCurve(animationCurveProperty, (CurveType)curveTypeProperty.enumValueIndex, minLevelProperty, minValueProperty, maxLevelProperty, maxValueProperty);
            });
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh Values"))
        {
            UpdateValues(animationCurveProperty, minLevelProperty, minValueProperty, maxLevelProperty, maxValueProperty);
        }

        if (GUILayout.Button("Refresh Curve"))
        {
            UpdateCurve(animationCurveProperty, (CurveType)curveTypeProperty.enumValueIndex, minLevelProperty, minValueProperty, maxLevelProperty, maxValueProperty);
        }

        if (GUILayout.Button("Reset"))
        {
            animationCurveProperty.animationCurveValue = new AnimationCurve();
            curveTypeProperty.enumValueIndex = (int)CurveType.Custom;
            minLevelProperty.floatValue = 0;
            minValueProperty.floatValue = 0;
            maxLevelProperty.floatValue = 0;
            maxValueProperty.floatValue = 0;
        }
        EditorGUILayout.EndHorizontal();
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

    public void UpdateCurve(SerializedProperty animationCurveP, CurveType curveType, SerializedProperty minLevelP, SerializedProperty minValueP, SerializedProperty maxLevelP, SerializedProperty maxValueP)
    {
        switch (curveType)
        {
            case CurveType.Custom:
                break;
            case CurveType.Linear:
                animationCurveP.animationCurveValue = AnimationCurve.Linear(minLevelP.floatValue, minValueP.floatValue, maxLevelP.floatValue, maxValueP.floatValue);
                break;
            case CurveType.EaseInOut:
                animationCurveP.animationCurveValue = AnimationCurve.EaseInOut(minLevelP.floatValue, minValueP.floatValue, maxLevelP.floatValue, maxValueP.floatValue);
                break;
        }
    }

    public void UpdateValues(SerializedProperty animationCurveP,SerializedProperty minLevelP, SerializedProperty minValueP, SerializedProperty maxLevelP, SerializedProperty maxValueP)
    {
        if (animationCurveP.animationCurveValue.keys.Length > 0)
        {
            minLevelP.floatValue = animationCurveP.animationCurveValue.keys[0].time;
            minValueP.floatValue = animationCurveP.animationCurveValue.keys[0].value;
            maxLevelP.floatValue = animationCurveP.animationCurveValue.keys[animationCurveP.animationCurveValue.keys.Length - 1].time;
            maxValueP.floatValue = animationCurveP.animationCurveValue.keys[animationCurveP.animationCurveValue.keys.Length - 1].value;
        }
    }
}
