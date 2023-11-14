using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    public event Action<CharacterSelectUI> OnSelected;

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
    [SerializeField] private AdvanceButton selectButton;

    [Header("Locked UI")]
    [SerializeField] private GameObject lockedUI;
    [SerializeField] private Image characterLockedName;

    [Header("Selected UI")]
    [SerializeField] private Image selectedUIIcon;

    [Header("Unselected UI")]
    [SerializeField] private GameObject unSelectedUI;

    CharacterSO currentCharacter;

    public CharacterSO GetCharacterSO => currentCharacter;
    public State GetState => state;

    private void Awake()
    {
        selectButton.onClick.AddListener(() =>
        {
            SetState(State.Selected);
            OnSelected?.Invoke(this);
        });
    }

    public void Init(CharacterSO characterSO)
    {
        currentCharacter = characterSO;

        state = currentCharacter.CharacterData.isUnlocked ? State.Unselected : State.Locked;

        if (currentCharacter.CharacterData.isSelected)
        {
            state = State.Selected;
            OnSelected?.Invoke(this);
        }

        SetState(state);

        newlyUnlockedInfo.enabled = state == State.NewlyUnlocked;
        lockedUI.SetActive(state == State.Locked);
        unlockedUI.SetActive(state != State.Locked);
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
        selectButton.interactable = false;
    }

    public void SelectedState()
    {
        UnlockedState();
        unSelectedUI.SetActive(false);
        selectedUIIcon.gameObject.SetActive(true);
    }

    public void UnSelectedState()
    {
        UnlockedState();
        selectedUIIcon.gameObject.SetActive(false);
        unSelectedUI.SetActive(true);
    }

    public void UnlockedState()
    {
        selectButton.interactable = true;
        characterImage.sprite = currentCharacter.CharacterUiData.CharacterUnlockedImage;
        characterUnlockedName.sprite = currentCharacter.CharacterUiData.CharacterUnlockedName;
        characterClassIcon.sprite = currentCharacter.CharacterUiData.CharacterClassIcon;
        characterClassName.sprite = currentCharacter.CharacterUiData.CharacterClassName;
    }
}
