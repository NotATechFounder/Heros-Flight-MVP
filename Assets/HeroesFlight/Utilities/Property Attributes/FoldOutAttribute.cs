using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoldOutAttribute : PropertyAttribute
{
    public string ID;

    public FoldOutAttribute(string _ID)
    {
        ID = _ID;
    }
}
