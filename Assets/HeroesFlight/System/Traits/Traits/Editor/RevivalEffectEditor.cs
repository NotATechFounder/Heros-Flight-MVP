using HeroesFlight.System.Stats.Traits.Effects;
using UnityEditor;

namespace HeroesFlight.System.Stats.Traits.Editor
{
    [CustomEditor(typeof(RevivalEffect))]
    public class RevivalEffectEditor : TraitEffectEditor
    {
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

            serializedObject.ApplyModifiedProperties();
        }
    }
}