using Pelumi.Juicer;
using StansAssets.Foundation.Async;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class HeroProgressionMenu : BaseMenu<HeroProgressionMenu>
    {
        public event Func<HeroProgressionAttributeInfo[]> GetHeroAttributes;

        public Action<HeroProgressionAttributeInfo> OnUpButtonClickedEvent;
        public Action<HeroProgressionAttributeInfo> OnDownButtonClickedEvent;
        public Action OnCloseButtonPressed;
        public Action OnResetButtonPressed;

        [SerializeField] private Transform container;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI spText;
        [SerializeField] private HeroAttributeUI[] heroAttributeUIArray;
        [SerializeField] private TextMeshProUGUI infoText;
        [SerializeField] private AdvanceButton closeButton;
        [SerializeField] private AdvanceButton resetButton;

        JuicerRuntime openEffectBG;
        JuicerRuntime openEffectContainer;

        JuicerRuntime closeEffectBG;
        JuicerRuntime closeEffectContainer;

        private bool loadedHeroAttributes = false;

        // TODO: Remove this
        public HeroProgression heroProgression;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                OnUpButtonClickedEvent = (HeroProgressionAttributeInfo) =>
                {
                     heroProgression.DecrementAttributeSP(HeroProgressionAttributeInfo);
                };

                OnDownButtonClickedEvent = (HeroProgressionAttributeInfo) =>
                {
                    heroProgression.IncrementAttributeSP(HeroProgressionAttributeInfo);
                };

                GetHeroAttributes = () =>
                {
                    return heroProgression.HeroProgressionAttributeInfos;
                };

                heroProgression.OnLevelUp += (level) =>
                {
                    OnLevelUp(level);
                };

                heroProgression.OnSpChanged += (sp) =>
                {
                    OnSpChanged(sp);
                };

                OnCloseButtonPressed = () =>
                {
                    heroProgression.Confirm();
                    Close();
                };

                OnResetButtonPressed = () =>
                {
                    heroProgression.ResetSP();
                };

                OnCreated();
                OnOpened();

                Close();
            }
        }

        public override void OnCreated()
        {
            container.localScale = Vector3.zero;
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);
            openEffectContainer = container.JuicyScale(Vector3.one, 0.15f).SetEase(Ease.EaseInQuart).SetDelay(0.15f);
            closeEffectContainer = container.JuicyScale(Vector3.zero, 0.15f).SetEase(Ease.EaseInQuart);
            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f).SetDelay(0.15f);
            closeEffectBG.SetOnComplected(CloseMenu);

            closeButton.onClick.AddListener(ForceClose);
            resetButton.onClick.AddListener(ResetButtonPressed);
        }

        public override void OnOpened()
        {
            openEffectBG.Start();
            openEffectContainer.Start();

            if (!loadedHeroAttributes)
            {
                SetHeroAttributeUIs();
            }
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
            closeEffectContainer.Start();
        }

        public override void ResetMenu()
        {
            loadedHeroAttributes = false;
        }

        public void OnLevelUp(int level)
        {
            levelText.text = $"Level {level}";
            ResetHeroAttributeUIs();
            Open(); 
        }

        public void OnSpChanged(int sp)
        {
            spText.text = $"Avaliable SP : {sp}";
            closeButton.interactable = (sp == 0);
        }

        public void SetHeroAttributeUIs()
        {
            HeroProgressionAttributeInfo[] heroProgressionAttributeInfos = GetHeroAttributes?.Invoke();
            for (int i = 0; i < heroAttributeUIArray.Length; i++)
            {
                heroAttributeUIArray[i].SetAttribute(heroProgressionAttributeInfos[i]);
                heroAttributeUIArray[i].OnUpButtonClickedEvent = OnUpButtonClickedEvent;
                heroAttributeUIArray[i].OnDownButtonClickedEvent = OnDownButtonClickedEvent;
                heroAttributeUIArray[i].OnInfoButtonClickedEvent = OnInfoButtonClickedEvent;
            }

            loadedHeroAttributes = true;
        }

        public void ResetHeroAttributeUIs()
        {
            foreach (var heroAttributeUI in heroAttributeUIArray)
            {
                heroAttributeUI.ResetSpTextColor();
            }
        }

        private void OnInfoButtonClickedEvent(HPAttributeSO sO)
        {
            infoText.text = sO.Description;
        }

        public void ForceClose()
        {
            OnCloseButtonPressed?.Invoke();
            Close();
        }

        private void ResetButtonPressed()
        {
            ResetHeroAttributeUIs();
            OnResetButtonPressed?.Invoke();
        }
    }
}
