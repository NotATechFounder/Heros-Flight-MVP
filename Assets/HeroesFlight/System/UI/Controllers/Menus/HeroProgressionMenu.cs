using Pelumi.Juicer;
using Plugins.Audio_System;
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
        public Func<HeroProgressionAttributeInfo[]> GetHeroAttributes;

        public Action<HeroProgressionAttributeInfo> OnUpButtonClickedEvent;
        public Action<HeroProgressionAttributeInfo> OnDownButtonClickedEvent;
        public Action OnCloseButtonPressed;
        public Action OnResetButtonPressed;

        [SerializeField] private Transform container;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI spText;
        [SerializeField] private HeroAttributeUI[] heroAttributeUIArray;
        [SerializeField] private AdvanceButton closeButton;
        [SerializeField] private AdvanceButton resetButton;

        JuicerRuntime openEffectBG;
        JuicerRuntime openEffectContainer;

        JuicerRuntime closeEffectBG;
        JuicerRuntime closeEffectContainer;

        private bool loadedHeroAttributes = false;

        private int currentSP;

        public override void OnCreated()
        {
            container.localScale = Vector3.zero;
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);
            openEffectContainer = container.JuicyScale(Vector3.one, 0.15f).SetEase(Ease.EaseInQuart).SetDelay(0.15f);
            closeEffectContainer = container.JuicyScale(Vector3.zero, 0.15f).SetEase(Ease.EaseInQuart);
            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f).SetDelay(0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

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

            foreach (var heroAttributeUI in heroAttributeUIArray)
            {
                heroAttributeUI.ToggleButtonActive(true);
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
            ResetHeroAttributeUIs();
            levelText.text = level.ToString();
            Open(); 
        }

        public void OnSpChanged(int sp)
        {
            currentSP = sp;
            spText.text = sp.ToString();
        }

        public void SetHeroAttributeUIs()
        {
            HeroProgressionAttributeInfo[] heroProgressionAttributeInfos = GetHeroAttributes?.Invoke();
            for (int i = 0; i < heroAttributeUIArray.Length; i++)
            {
                heroAttributeUIArray[i].SetAttribute(heroProgressionAttributeInfos[i]);
                heroAttributeUIArray[i].OnUpButtonClickedEvent = OnUpButtonClickedEvent;
                heroAttributeUIArray[i].OnDownButtonClickedEvent = OnDownButtonClickedEvent;
                heroAttributeUIArray[i].OnAddSpEffectStart = OnAddSpStart;
                heroAttributeUIArray[i].OnAddSpEffectCompleted = OnAddSpCompleted;
            }

            loadedHeroAttributes = true;
        }

        private void OnAddSpStart(HeroAttributeUI heroAttributeUI)
        {
            heroAttributeUI.ToggleButtonActive(false);
        }

        private void OnAddSpCompleted(HeroAttributeUI heroAttributeUI)
        {
            if (currentSP == 0)
            {
                foreach (var heroAttributeUIs in heroAttributeUIArray)
                {
                    heroAttributeUIs.ToggleButtonActive(false);
                }

                CoroutineUtility.WaitForSeconds(1f, () =>
                {
                    ForceClose();
                });
            }
            else
            {
                heroAttributeUI.ToggleButtonActive(true);
            }
        }

        public void ResetHeroAttributeUIs()
        {
            foreach (var heroAttributeUI in heroAttributeUIArray)
            {
                heroAttributeUI.OnModified(false);
            }
        }

        public void ForceClose()
        {
            OnCloseButtonPressed?.Invoke();
            Close();
        }

        private void ResetButtonPressed()
        {
            OnResetButtonPressed?.Invoke();
        }
    }
}
