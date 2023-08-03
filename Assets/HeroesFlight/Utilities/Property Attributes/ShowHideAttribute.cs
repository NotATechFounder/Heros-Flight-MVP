using UnityEditor;
using UnityEngine;

public class ShowHideAttribute : PropertyAttribute
{
    public string boolCondition = null;
    public string eumName;
    public object theEnum;

    public ShowHideAttribute(string _condition)
    {
        boolCondition = _condition;
    }

    public ShowHideAttribute(string _enumName, object _enumCondition)
    {
        eumName = _enumName;
        theEnum = _enumCondition;
    }
}

public class ShowIfAttribute : ShowHideAttribute
{
    public ShowIfAttribute(string _condition) : base(_condition)
    {
        boolCondition = _condition;
    }

    public ShowIfAttribute(string _enumName, object _enumCondition) : base( _enumName,  _enumCondition)
    {
        eumName = _enumName;
        theEnum = _enumCondition;
    }
}

public class HideIfAtttribute : ShowHideAttribute
{
    public HideIfAtttribute(string _condition) : base(_condition)
    {
        boolCondition = _condition;
    }

    public HideIfAtttribute(string _enumName, object _enumCondition) : base(_enumName, _enumCondition)
    {
        eumName = _enumName;
        theEnum = _enumCondition;
    }
}


[CustomPropertyDrawer(typeof(ShowIfAttribute)), CustomPropertyDrawer(typeof(HideIfAtttribute))]
public class ShowHideDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label, true);
    }
}