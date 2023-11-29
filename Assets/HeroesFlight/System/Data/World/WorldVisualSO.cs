using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "World", menuName = "Game/World", order = 1)]
public class WorldVisualSO : ScriptableObject
{
    public WorldType worldType;
    public string worldName;
    public Sprite icon;
}
