using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using Pelumi.Juicer;
using Spine;
using Spine.Unity;
using TMPro;
using UISystem;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

using Random = UnityEngine.Random;

namespace HeroesFlight.System.UI.DIce
{
    public class DiceMenu : BaseMenu<DiceMenu>
    {
        [Header("Canvas groups")] 
        [SerializeField] private CanvasGroup mainCG;
        [SerializeField] private CanvasGroup rollCG;
        [SerializeField] private CanvasGroup infoCG;
        [SerializeField] private TextMeshProUGUI rollText;
        [SerializeField] private TextMeshProUGUI infoText;

        [Header("Buttons")]
        [SerializeField] private AdvanceButton rollButton;
        [SerializeField] private AdvanceButton backButton;
        [SerializeField] private AdvanceButton optionalROllButton;
        [SerializeField] private AdvanceButton infoButton;
        [SerializeField] private AdvanceButton closeInfoButton;

        [Header("Dice")]
        [SerializeField] private SkeletonGraphic skeletonAnimation;


        private WaitForSeconds rollPeriod;
        private Coroutine rollRoutine;

        private Action onRollAction;


        private int endValue;
        private Color endColor;
        private Action onRollEnd;

        public void ShowDiceMenu(int initialValue,Action OnRoll)
        {
            rollText.text = initialValue.ToString();
            onRollAction = OnRoll;
            ToggleCanvasGroup(rollCG, true);
            Open();
        }

        public void RollDiceUi(int resultValue, Color endColor, Action OnComplete)
        {
            endValue = resultValue;
            this.endColor = endColor;
            onRollEnd = OnComplete;
            RollDice(resultValue);
        }

        public void ShowDiceInfo(string info)
        {
            ToggleCanvasGroup(infoCG, true);
            if (!info.Equals(string.Empty)) infoText.text = info;
            Open();
        }

        public void ModifyDiceRollResultUi(string value)
        {
            rollText.text = value;
        }
        public override void ResetMenu()
        {
        }

        public override void OnCreated()
        {
            rollPeriod = new WaitForSeconds(0.1f);
            rollButton.onClick.AddListener(() => { onRollAction?.Invoke(); });
            infoButton.onClick.AddListener(()=>{ShowDiceInfo(string.Empty);});
            closeInfoButton.onClick.AddListener(() =>
            {
                Debug.Log($"disabling {infoCG.name}");
                ToggleCanvasGroup(infoCG,false);
            });

            backButton.onClick.AddListener(() =>
            {
                ToggleCanvasGroup(rollCG, false);
                Close();
            });

            skeletonAnimation.AnimationState.Complete += AnimationState_Complete;
        }

        public override void OnOpened()
        {
            ToggleCanvasGroup(mainCG,true);
        }

        public override void OnClosed()
        {
            ToggleCanvasGroup(mainCG,false);
            ToggleCanvasGroup(rollCG, false);
            ToggleCanvasGroup(infoCG, false);
            onRollAction = null;
            rollText.color = Color.white;
        }


        void ToggleCanvasGroup(CanvasGroup cg, bool isEnabled)
        {
            cg.alpha = isEnabled ? 1 : 0;
            cg.interactable = isEnabled;
            cg.blocksRaycasts = isEnabled;
        }

        public void RollDice(int endValue)
        {
            backButton.interactable = false;
            rollButton.interactable = false;
            infoButton.interactable = false;
            skeletonAnimation.AnimationState.SetAnimation(0, endValue.ToString(), false);
        }

        private void AnimationState_Complete(TrackEntry trackEntry)
        {
            rollText.text = endValue.ToString();
            rollText.color = endColor;
            onRollEnd?.Invoke();

            backButton.interactable = true;
            rollButton.interactable = true;
            infoButton.interactable = true;
        }
    }
}