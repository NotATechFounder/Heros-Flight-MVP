using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    public enum State
    {
        Unselected,
        Selected,
        Locked,
        NewlyUnlocked
    }

    [SerializeField] private State state;

    [Header("Common UI")]
    [SerializeField] private Image characterImage;

    [Header("NewlyUnlocked UI")]
    [SerializeField] private Image newlyUnlockedInfo;

    [Header("Unlocked UI")]
    [SerializeField] private GameObject unlockedUI;
    [SerializeField] private Image characterName;
    [SerializeField] private Image characterClassIcon;
    [SerializeField] private Image characterClassName;
    [SerializeField] private AdvanceButton toggleSelectionButton;

    [Header("Locked UI")]
    [SerializeField] private GameObject lockedUI;
    [SerializeField] private Image characterNameLocked;

    public void SetCharacterUI(RectTransform target)
    {
        characterImage.sprite = target.GetComponent<Image>().sprite;
        characterName.sprite = target.GetComponent<CharacterUI>().characterName.sprite;
        characterClassIcon.sprite = target.GetComponent<CharacterUI>().characterClassIcon.sprite;
        characterClassName.sprite = target.GetComponent<CharacterUI>().characterClassName.sprite;
        characterNameLocked.sprite = target.GetComponent<CharacterUI>().characterName.sprite;
    }

    public void LockedState()
    {

    }

    public void NewlyUnlockedState()
    {

    }

    public void SelectedState()
    {

    }

    public void UnselectedState()
    {

    }
}
