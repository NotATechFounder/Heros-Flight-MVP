using UnityEngine;

[System.Serializable]
public class TutorialStep
{
    public string stepName;
    public Sprite stepImage;
    [TextArea(3, 10)]
    public string stepDescription;
}
