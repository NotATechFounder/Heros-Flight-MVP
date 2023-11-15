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

    [Header("InView UI")]
    [SerializeField] private Image InViewIcon;


    CharacterSO currentCharacter;

    public CharacterSO GetCharacterSO => currentCharacter;
    public State GetState => state;

    private void Awake()
    {
        selectButton.onClick.AddListener(() =>
        {
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
    }

    public void SetState(State unselected)
    {
        state = unselected;

        lockedUI.SetActive(state == State.Locked);
        unlockedUI.SetActive(state != State.Locked);

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
        selectedUIIcon.gameObject.SetActive(true);
    }

    public void UnSelectedState()
    {
        UnlockedState();
        selectedUIIcon.gameObject.SetActive(false);
    }

    public void UnlockedState()
    {
        characterImage.sprite = currentCharacter.CharacterUiData.CharacterUnlockedImage;
        characterUnlockedName.sprite = currentCharacter.CharacterUiData.CharacterUnlockedName;
        characterClassIcon.sprite = currentCharacter.CharacterUiData.CharacterClassIcon;
        characterClassName.sprite = currentCharacter.CharacterUiData.CharacterClassName;
    }

    public void ToggleInView(bool state)
    {
        InViewIcon.gameObject.SetActive(state);
    }
}
