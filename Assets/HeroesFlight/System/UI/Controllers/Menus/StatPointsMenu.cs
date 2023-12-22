using HeroesFlight.Common.Progression;
using Pelumi.Juicer;
using Plugins.Audio_System;
using StansAssets.Foundation.Async;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class StatPointsMenu : BaseMenu<StatPointsMenu>
    {
        public event Action<StatAttributeType, int> OnDiceClicked;
        public event Action OnCompletePressed;
        public event Action OnResetButtonPressed;
        public event Func< int> GetAvailabletSp;
        public event Func<StatAttributeType, int> GetCurrentSpLevel;
        public event Func<StatAttributeType, bool> OnAddSpClicked;
        public event Func<StatAttributeType, bool> OnRemoveSpClicked;
        public event Func <StatAttributeType, int> GetDiceRollValue;

        [SerializeField] private AdvanceButton completeButton;
        [SerializeField] private AdvanceButton resetButton;

        [SerializeField] private Transform container;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI spText;
        [SerializeField] private StatPointSO[] statPointSOArray;
        [SerializeField] private StatPointUI statPointUIPrefab;
        [SerializeField] private Transform statPointContainer;

        JuicerRuntime openEffectBG;
        JuicerRuntime openEffectContainer;
        JuicerRuntime closeEffectBG;
        JuicerRuntime closeEffectContainer;

        private Dictionary<StatAttributeType, StatPointUI> statPointUIDic = new Dictionary<StatAttributeType, StatPointUI>();

        public StatPointUI[] GetStatPointUIs => statPointUIDic.Values.ToArray();

        public override void OnCreated()
        {
            container.localScale = Vector3.zero;
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);
            openEffectContainer = container.JuicyScale(Vector3.one, 0.15f).SetEase(Ease.EaseInQuart).SetDelay(0.15f);
            closeEffectContainer = container.JuicyScale(Vector3.zero, 0.15f).SetEase(Ease.EaseInQuart);
            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f).SetDelay(0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            completeButton.onClick.AddListener(ForceClose);
            resetButton.onClick.AddListener(ResetButtonPressed);

            CacheStatUI();
        }

        public override void OnOpened()
        {
            openEffectBG.Start();
            openEffectContainer.Start();

            InitStatUI();
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
            closeEffectContainer.Start();
        }

        public override void ResetMenu()
        {
       
        }

        public void CacheStatUI()
        {
            foreach (StatPointSO statpointSo in statPointSOArray)
            {
                StatPointUI statPointUI = Instantiate(statPointUIPrefab, statPointContainer);

                statPointUI.GetCurrentSpLevel += (stat) =>
                {
                    return GetCurrentSpLevel.Invoke(stat);
                };

                statPointUI.OnAddSpClicked += (stat) =>
                {
                    return OnAddSpClicked.Invoke(stat);
                };

                statPointUI.OnRemoveSpClicked += (stat) =>
                {
                    return OnRemoveSpClicked.Invoke(stat);
                };

                statPointUI.OnSpChanged += () =>
                {
                    spText.text = GetAvailabletSp?.Invoke().ToString();
                };

                statPointUI.GetDiceRollValue += (stat) =>
                {
                    return GetDiceRollValue.Invoke(stat);
                };

                statPointUI.OnDiceClicked += (statAttributeType, value) =>
                {
                    OnDiceClicked?.Invoke(statAttributeType, value);
                };

                statPointUI.Init(statpointSo);

                statPointUIDic.Add(statpointSo.StatAttributeType, statPointUI);
            }
        }

        public void InitStatUI()
        {
            foreach (KeyValuePair<StatAttributeType, StatPointUI> statPointUI in statPointUIDic)
            {
                statPointUI.Value.LoadCurrentValues();
            }

            spText.text = GetAvailabletSp?.Invoke().ToString();
        }

        public void ForceClose()
        {
            bool thereIsUpgrade = false;
            foreach (KeyValuePair<StatAttributeType, StatPointUI> statPointUI in statPointUIDic)
            {
                if(statPointUI.Value.TryUpgradeCurrentValue())
                {
                    thereIsUpgrade = true;
                }
            }

            OnCompletePressed?.Invoke();

            if (thereIsUpgrade)
            {
                CoroutineUtility.WaitForSeconds(1.5f, () =>
                {
                    Close();
                });
            }
            else
            {
                Close();
            }
        }

        private void ResetButtonPressed()
        {
            OnResetButtonPressed?.Invoke();
        }

        public void OnNewDiceRoll(StatAttributeType statAttributeType, int rolledValue)
        {
            statPointUIDic[statAttributeType].OnNewDiceRoll(rolledValue);
            //InitStatUI();
        }
    }
}
