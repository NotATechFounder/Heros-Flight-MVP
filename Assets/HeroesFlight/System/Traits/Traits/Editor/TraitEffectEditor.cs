using System;
using HeroesFlight.System.Stats.Traits.Effects;
using UnityEditor;
using UnityEngine;

namespace HeroesFlight.System.Stats.Traits.Editor
{
    [CustomEditor(typeof(TraitEffect))]
    public class TraitEffectEditor : UnityEditor.Editor
    {
        protected SerializedProperty a;
        protected SerializedProperty b;
        protected SerializedProperty c;
        protected SerializedProperty d;
        protected virtual void OnEnable()
        {
            a = serializedObject.FindProperty("traitType");
            b = serializedObject.FindProperty("value");
            c = serializedObject.FindProperty("canBeRerolled");
            d = serializedObject.FindProperty("valueRange");
           
        }

       
    }
}