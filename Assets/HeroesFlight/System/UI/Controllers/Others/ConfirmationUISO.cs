
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New ConfirmationUISO", menuName = "ScriptableObject/ConfirmationUISO")]
public class ConfirmationUISO : ScriptableObject
{
    [SerializeField] public Sprite Icon;
    [SerializeField] public string TitleText;

    [TextArea(3, 10)]
    [SerializeField] public string QuestionText;

    [TextArea(3, 10)]
    [SerializeField] public string DescriptionText;
}
