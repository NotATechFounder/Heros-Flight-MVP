using HeroesFlight.System.Stats.Traits.Effects;
using UnityEditor;

namespace HeroesFlight.System.Stats.Traits.Editor
{
    [CustomEditor(typeof(StatBoostEffect))]
    public class StatBoostEffectEditor : TraitEffectEditor
    {
        private SerializedProperty e;

        protected override void OnEnable()
        {
            base.OnEnable();
            e = serializedObject.FindProperty("targetStat");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(a);
            EditorGUILayout.PropertyField(e);
            EditorGUILayout.PropertyField(b);
            EditorGUILayout.PropertyField(c);
            if (c.boolValue)
            {
                EditorGUILayout.PropertyField(d);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}