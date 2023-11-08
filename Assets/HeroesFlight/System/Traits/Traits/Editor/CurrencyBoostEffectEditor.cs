using HeroesFlight.System.Stats.Traits.Effects;
using UnityEditor;

namespace HeroesFlight.System.Stats.Traits.Editor
{
    [CustomEditor(typeof(CurrencyBoostEffect))]
    public class CurrencyBoostEffectEditor : TraitEffectEditor
    {
        private SerializedProperty e;
        protected override void OnEnable()
        {
            base.OnEnable();
            e = serializedObject.FindProperty("targetType");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(a);
            EditorGUILayout.PropertyField(b);
            EditorGUILayout.PropertyField(c);
            if (c.boolValue)
            {
                EditorGUILayout.PropertyField(d);
            }
            EditorGUILayout.PropertyField(e);
            serializedObject.ApplyModifiedProperties();
        }
    }
}