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
        public event Action OnAdsRollPressed;
        public event Action OnGemRollPressed;

        [Header("Ads")]
        [SerializeField] private TextMeshProUGUI adsRollText;

        [Header("Canvas groups")] 
        [SerializeField] private CanvasGroup mainCG;
        [SerializeField] private CanvasGroup rollCG;
        [SerializeField] private CanvasGroup infoCG;
        [SerializeField] private TextMeshProUGUI rollText;
        [SerializeField] private TextMeshProUGUI infoText;

        [Header("Buttons")]
        [SerializeField] private AdvanceButton adsRollButton;
        [SerializeField] private AdvanceButton gemROllButton;
        [SerializeField] private AdvanceButton backButton;
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
            adsRollButton.onClick.AddListener(() => 
            {
                OnAdsRollPressed?.Invoke();
            });

            gemROllButton.onClick.AddListener(() =>
            {
                OnGemRollPressed?.Invoke();
            });

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
                adsRollButton.interactable = true;
                gemROllButton.interactable = true;
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

        public void TriggerRollAction()
        {
            onRollAction?.Invoke();
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
            adsRollButton.interactable = false;
            gemROllButton.interactable = false;
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

        public void SetAdsCount(int index)
        {
            adsRollText.text = index.ToString();
            adsRollButton.SetVisibility( index > 0 ? GameButtonVisiblity.Visible : GameButtonVisiblity.Hidden);
        }
    }
}