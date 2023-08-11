using Pelumi.Juicer;
using StansAssets.Foundation.Async;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem;
using UnityEngine;

namespace UISystem
{
    public class HeroProgressionMenu : BaseMenu<HeroProgressionMenu>
    {
        public event Func<HeroProgressionAttributeInfo[]> GetHeroAttributes;

        public Func<bool> OnUpButtonClicked;
        public Func<bool> OnDownButtonClicked;

        [SerializeField] private Transform container;
        [SerializeField] private HeroAttributeUI[] heroAttributeUIArray;
        [SerializeField] private TextMeshProUGUI infoText;

        JuicerRuntime openEffectBG;
        JuicerRuntime openEffectContainer;

        JuicerRuntime closeEffectBG;
        JuicerRuntime closeEffectContainer;

        private bool loadedHeroAttributes = false;

        // TODO: Remove this
        public HeroProgression heroProgressionMenu;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnUpButtonClicked = () =>
                {
                    return heroProgressionMenu.CanSpendSP();
                };

                OnDownButtonClicked = () =>
                {
                    return heroProgressionMenu.CanSpendSP();
                };

                GetHeroAttributes = () =>
                {
                    return heroProgressionMenu.HeroProgressionAttributeInfos;
                };

                heroProgressionMenu.OnLevelUp += (level) =>
                {
                    infoText.text = $"Level Up! You are now level {level}";
                };

                OnCreated();
                OnOpened();
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

        private void OnInfoButtonClickedEvent(HPAttributeSO sO)
        {
            infoText.text = sO.Description;
        }

        private bool OnDownButtonClickedEvent()
        {
            return OnUpButtonClicked.Invoke();
        }

        private bool OnUpButtonClickedEvent()
        {
            return OnDownButtonClicked.Invoke();
        }
    }
}
