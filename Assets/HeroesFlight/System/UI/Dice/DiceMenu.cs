using System;
using Pelumi.Juicer;
using Spine;
using Spine.Unity;
using TMPro;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private CanvasGroup diceView;
        [SerializeField] private SkeletonGraphic skeletonAnimation;


        private Action onRollAction;

        private int endValue;
        private Color endColor;
        private Action onRollEnd;

        private JuicerRuntime diceRollStartEffect;
        private JuicerRuntime diceRollEndEffect;

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
            RollDice();
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
            rollButton.onClick.AddListener(() => { onRollAction?.Invoke(); });
            infoButton.onClick.AddListener(()=>{ShowDiceInfo(string.Empty);});
            closeInfoButton.onClick.AddListener(() =>
            {
                ToggleCanvasGroup(infoCG,false);
            });

            backButton.onClick.AddListener(() =>
            {
                ToggleCanvasGroup(rollCG, false);
                Close();
            });

            diceRollStartEffect = diceView.JuicyAlpha(1, .5f).SetOnCompleted(() =>
            {
                diceView.interactable = true;
                diceView.blocksRaycasts = true;
                skeletonAnimation.AnimationState.SetAnimation(0, endValue.ToString(), false);
            });

            diceRollEndEffect = diceView.JuicyAlpha(0, .5f).SetOnCompleted(() =>
            {
                diceView.interactable = false;
                diceView.blocksRaycasts = false;

                backButton.interactable = true;
                rollButton.interactable = true;
                infoButton.interactable = true;
            });

            skeletonAnimation.AnimationState.Complete += AnimationState_Complete;

            ToggleCanvasGroup (diceView, false);
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

        public void RollDice()
        {
            backButton.interactable = false;
            rollButton.interactable = false;
            infoButton.interactable = false;
            diceRollStartEffect.Start();
        }

        private void AnimationState_Complete(TrackEntry trackEntry)
        {
            rollText.text = endValue.ToString();
            rollText.color = endColor;
            onRollEnd?.Invoke();

            diceRollEndEffect.Start();
        }
    }
}