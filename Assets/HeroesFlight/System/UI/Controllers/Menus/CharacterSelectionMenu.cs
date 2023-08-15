using System.Collections.Generic;
using HeroesFlight.System.Character.Enum;
using HeroesFlight.System.UI.Data;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class CharacterSelectionMenu : BaseMenu<CharacterSelectionMenu>
    {
        [Header("Data")]
        [SerializeField] CharacterUiViewData[] characters;
        [Header("Navigation")]
        [SerializeField] AdvanceButton previousButton;
        [SerializeField] AdvanceButton nextButton;
        [SerializeField] AdvanceButton selectButton;
        [Header("Lock image")
        ][SerializeField] Image lockImage;
        public CharacterType selectedType { get; private set; }
        UiSpineViewController viewController;
        List<CharacterType> unlockedTypes = new();
        Dictionary<int, CharacterUiViewData> dataCache = new();
        int currentIndex;

        protected override void Awake()
        {
            base.Awake();
            viewController = GetComponentInChildren<UiSpineViewController>();
            GenerateCache();
            unlockedTypes.Add(CharacterType.Tagon);
            unlockedTypes.Add(CharacterType.Lancer);
            selectedType = CharacterType.Tagon;
            previousButton.onClick.AddListener(() =>
            {
                ChangeCharacter(-1);
            });
            nextButton.onClick.AddListener(() =>
            {
                ChangeCharacter(1);
            });
            selectButton.onClick.AddListener(() =>
            {
                Close();
            });
            currentIndex = 0;
        }

        void GenerateCache()
        {
            for (int i = 0; i < characters.Length; i++)
            {
                dataCache.Add(i, characters[i]);
            }
        }

        public override void ResetMenu()
        {
        }

        public override void OnCreated()
        {
        }

        public override void OnOpened()
        {
            viewController.SetupView(dataCache[currentIndex]);
        }

        public override void OnClosed()
        {
            CloseMenu();
            Debug.Log("menu closed");
        }

        public void ChangeCharacter(int incrementer)
        {
            currentIndex += incrementer;
            if (currentIndex >= dataCache.Count)
            {
                currentIndex = 0;
            }
            else if (currentIndex < 0)
            {
                currentIndex = dataCache.Count - 1;
            }

            var characterUnlocked = unlockedTypes.Contains(dataCache[currentIndex].CharacterData.CharacterType);
            selectButton.interactable = characterUnlocked;
            lockImage.enabled = !characterUnlocked;
            if (characterUnlocked)
            {
                selectedType = dataCache[currentIndex].CharacterData.CharacterType;
            }
            viewController.SetupView(dataCache[currentIndex]);
        }
    }
}