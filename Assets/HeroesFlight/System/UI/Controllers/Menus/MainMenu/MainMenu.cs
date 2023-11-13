using HeroesFlight.Common.Enum;
using HeroesFlight.System.UI.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace UISystem
{
    public class MainMenu : BaseMenu<MainMenu>
    {
        public event Action<CharacterType, bool> OnCharacterSelected;

        public event Action OnPlayButtonPressed;
        public event Action OnSettingsButtonPressed;
        public event Action OnTraitButtonPressed;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI gemText;

        [SerializeField] private AdvanceButton playButton;
        [SerializeField] private AdvanceButton settingsButton;
        [SerializeField] private AdvanceButton traitsButton;
        [SerializeField] private CharacterUI characterUIPrefab;
        [SerializeField] private UISwipeArea swipeArea;
        [SerializeField] private UISelection uiSelection;

        [Header("Data")]
        [SerializeField] CharacterSO[] characters;

        [Header("Debug")]
        [SerializeField] private CharacterUI selectedCharacterUI;
        [SerializeField] private CharacterUI characterUIInView;

        Dictionary<RectTransform, CharacterUI> CharacterUICache = new();

        public override void OnCreated()
        {
            CharacterUI.OnSelected = OnCharacterUISelected;

            swipeArea.OnSwipe = OnSwipe;

            uiSelection.OnViewChanged = OnViewChange;

            playButton.onClick.AddListener(()=>
            {
                OnPlayButtonPressed?.Invoke();
            });

            settingsButton.onClick.AddListener(() =>
            {
                OnSettingsButtonPressed?.Invoke();
            });
            
            traitsButton.onClick.AddListener(() =>
            {
                OnTraitButtonPressed?.Invoke();
            });
            
            GenerateCache();
        }

        public override void OnOpened()
        {

        }

        public override void OnClosed()
        {
            CloseMenu();
        }

        public override void ResetMenu()
        {

        }

        public void UpdateGoldText(float gold)
        {
            goldText.text = gold.ToString();
        }

        public void UpdateGemText(float gem)
        {
            gemText.text = gem.ToString();
        }

        private void GenerateCache()
        {
            RectTransform[] rectTransforms = new RectTransform[characters.Length];

            for (int i = 0; i < characters.Length; i++)
            {
                CharacterUI characterUI = Instantiate(characterUIPrefab, uiSelection.transform);
                characterUI.SetCharacterUI(characters[i]);
                RectTransform rectTransform = characterUI.GetComponent<RectTransform>();
                CharacterUICache.Add(rectTransform, characterUI);
            }

            int index = 1;

            foreach (var item in CharacterUICache)
            {
                if (item.Value.GetCharacterSO.CharacterData.isSelected)
                {
                    rectTransforms[0] = item.Key;
                }
                else
                {
                    rectTransforms[index] = item.Key;
                    index++;
                }
            }

            uiSelection.Initialize(rectTransforms);
        }

        private void OnCharacterUISelected(CharacterUI characterUI)
        {
            if (selectedCharacterUI != null)
            {
                selectedCharacterUI.SetState(CharacterUI.State.Unselected);
                OnCharacterSelected?.Invoke(selectedCharacterUI.GetCharacterSO.CharacterType, false);
            }
            selectedCharacterUI = characterUI;
            selectedCharacterUI.SetState(CharacterUI.State.Selected);
            OnCharacterSelected?.Invoke(selectedCharacterUI.GetCharacterSO.CharacterType, true);
        }

        private void OnSwipe(UISwipeArea.SwipeDirection direction)
        {
            switch (direction)
            {
                case UISwipeArea.SwipeDirection.Left:
                    uiSelection.MoveTarget(UISelection.SwipeDirection.Left);
                    break;
                case UISwipeArea.SwipeDirection.Right:
                    uiSelection.MoveTarget(UISelection.SwipeDirection.Right);
                    break;
            }
        }

        private void OnViewChange(RectTransform transform)
        {
            if (characterUIInView)
            {
                characterUIInView.ToggleButtonVisibility(false);
            }

            characterUIInView = CharacterUICache[transform];
            if (!characterUIInView)
            {
                return;
            }

            characterUIInView.ToggleButtonVisibility(true);
        }
    }
}

