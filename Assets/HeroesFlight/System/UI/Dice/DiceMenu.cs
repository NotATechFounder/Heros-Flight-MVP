using System;
using System.Collections;
using System.Collections.Generic;
using Pelumi.Juicer;
using TMPro;
using UISystem;
using UnityEngine;
using UnityEngine.UI;
using NotImplementedException = System.NotImplementedException;
using Random = UnityEngine.Random;

namespace HeroesFlight.System.UI.DIce
{
    public class DiceMenu :BaseMenu<DiceMenu>
    {
        [SerializeField] private CanvasGroup thisCG;
        [SerializeField] private TextMeshProUGUI rollText;
        [Header("Buttons")]
        [SerializeField] private Button rollButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button optionalROllButton;
        [SerializeField] private Button infoButton;

        private WaitForSeconds rollPeriod;
        private Coroutine rollRoutine;

        private Action onRollAction;

        public void ShowDiceMenu(Action OnRoll)
        {
            onRollAction = OnRoll;
            Open();
        }
        public void RollDiceUi(int resultValue,Color endColor, Action OnComplete)
        {
            if(rollRoutine!=null)
              StopCoroutine(rollRoutine);

            rollRoutine= StartCoroutine(RollDiceRoutine(resultValue, endColor, OnComplete));
        }
        
        
        public override void ResetMenu() { }

        public override void OnCreated()
        {
            rollPeriod = new WaitForSeconds(0.1f);
            rollButton.onClick.AddListener(() =>
            {
                onRollAction?.Invoke();
            });
        }

        public override void OnOpened()
        {
            ToggleCanvasGroup(thisCG, true);
        }

        public override void OnClosed()
        {
            ToggleCanvasGroup(thisCG, false);
            onRollAction = null;
            rollText.color=Color.white;
        }
        
        
        void ToggleCanvasGroup(CanvasGroup cg, bool isEnabled)
        {
            if (isEnabled)
            {
                cg.alpha = 1;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
            else
            {
                cg.alpha = 0;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }

        IEnumerator RollDiceRoutine(int endValue,Color endColor, Action OnComplete)
        {
            backButton.interactable = false;
            rollButton.interactable = false;
          //  optionalROllButton.interactable = false;
          //  infoButton.interactable = false;
            var totallRolls = 15;
            while (totallRolls > 0)
            {
                yield return rollPeriod;
                totallRolls--;
                rollText.text = Random.Range(0, 13).ToString();
            }
            
            
            yield return new WaitForSeconds(.5f);
            rollText.text = endValue.ToString();
            rollText.color = endColor;
            rollText.transform.JuicyScale(3, .5f).Start();
            yield return new WaitForSeconds(.55f);
            rollText.transform.JuicyScale(1, .5f).Start();
            yield return new WaitForSeconds(.55f);
            backButton.interactable = true;
            rollButton.interactable = true;
           // optionalROllButton.interactable = true;
          //  infoButton.interactable = true;
            OnComplete?.Invoke();
        }
    }
}