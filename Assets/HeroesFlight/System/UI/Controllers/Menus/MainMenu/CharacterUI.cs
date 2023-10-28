using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    public static Action<CharacterUI> OnSelected;

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
    [SerializeField] private Image characterUnlockedName;
    [SerializeField] private Image characterClassIcon;
    [SerializeField] private Image characterClassName;
    [SerializeField] private AdvanceButton toggleSelectionButton;
    [SerializeField] private Image selectedButtonImage;

    [Header("Locked UI")]
    [SerializeField] private GameObject lockedUI;
    [SerializeField] private Image characterLockedName;

    [Header("Selected UI")]
    [SerializeField] private GameObject selectedUI;
    [SerializeField] private Sprite selectedButtonSprite;

    [Header("Unselected UI")]
    [SerializeField] private GameObject unSelectedUI;
    [SerializeField] private Sprite unselectedButtonSprite;

    CharacterSO currentCharacter;

    public CharacterSO GetCharacterSO => currentCharacter;

    private void Awake()
    {
        toggleSelectionButton.onClick.AddListener(() =>
        {
            OnSelected?.Invoke(this);
        });

        ToggleButtonVisibility(false);
    }

    public void ToggleButtonVisibility(bool STATE)
    {
        toggleSelectionButton.gameObject.SetActive(STATE);
    }

    public void SetCharacterUI(CharacterSO characterSO)
    {
        currentCharacter = characterSO;

        state = currentCharacter.CharacterData.isUnlocked ? State.Unselected : State.Locked;

        if (currentCharacter.CharacterData.isSelected)
        {
            state = State.Selected;
        }

        SetState(state);

        newlyUnlockedInfo.enabled = state == State.NewlyUnlocked;
        lockedUI.SetActive(state == State.Locked);
        unlockedUI.SetActive(state != State.Locked);

        if (currentCharacter.CharacterData.isSelected)
        {
            OnSelected?.Invoke(this);
        }
    }

    public void SetState(State unselected)
    {
        state = unselected;
        switch (state)
        {
            case State.Unselected:
                UnSelectedState();
                break;
            case State.Selected:
                SelectedState();
                break;
            case State.Locked:
                LockedState();
                break;
        }
    }

    public void LockedState()
    {
        characterImage.sprite = currentCharacter.CharacterUiData.CharacterLockedImage;
        characterLockedName.sprite = currentCharacter.CharacterUiData.CharacterLockedName;
    }

    public void SelectedState()
    {
        UnlockedState();
        selectedButtonImage.sprite = selectedButtonSprite;
        unSelectedUI.SetActive(false);
        selectedUI.SetActive(true);

    }

    public void UnSelectedState()
    {
        UnlockedState();
        selectedButtonImage.sprite = unselectedButtonSprite;
        selectedUI.SetActive(false);
        unSelectedUI.SetActive(true);
    }

    public void UnlockedState()
    {
        characterImage.sprite = currentCharacter.CharacterUiData.CharacterUnlockedImage;
        characterUnlockedName.sprite = currentCharacter.CharacterUiData.CharacterUnlockedName;
        characterClassIcon.sprite = currentCharacter.CharacterUiData.CharacterClassIcon;
        characterClassName.sprite = currentCharacter.CharacterUiData.CharacterClassName;
    }
}
