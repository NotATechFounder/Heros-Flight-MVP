
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New ConfirmationUISO", menuName = "ScriptableObject/ConfirmationUISO")]
public class ConfirmationUISO : ScriptableObject
{
    [SerializeField] public Image Icon;
    [SerializeField] public string TitleText;
    [SerializeField] public string QuestionText;
    [SerializeField] public string DescriptionText;
}
