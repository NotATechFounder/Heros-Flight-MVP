using System;
using System.Collections;
using System.Collections.Generic;
using Pelumi.Juicer;
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
        [SerializeField] private Button rollButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button optionalROllButton;
        [SerializeField] private Button infoButton;
        [SerializeField] private Button closeInfoButton;
      

        private WaitForSeconds rollPeriod;
        private Coroutine rollRoutine;

        private Action onRollAction;

        public void ShowDiceMenu(int initialValue,Action OnRoll)
        {
            rollText.text = initialValue.ToString();
            onRollAction = OnRoll;
            ToggleCanvasGroup(rollCG, true);
            Open();
        }

        public void RollDiceUi(int resultValue, Color endColor, Action OnComplete)
        {
            if (rollRoutine != null)
                StopCoroutine(rollRoutine);

            rollRoutine = StartCoroutine(RollDiceRoutine(resultValue, endColor, OnComplete));
        }

        public void ShowDiceInfo(string info)
        {
            ToggleCanvasGroup(infoCG, true);
            if (!info.Equals(string.Empty)) infoText.text = info;
            Open();
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

        IEnumerator RollDiceRoutine(int endValue, Color endColor, Action OnComplete)
        {
            backButton.interactable = false;
            rollButton.interactable = false;
            //  optionalROllButton.interactable = false;
            infoButton.interactable = false;
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
             infoButton.interactable = true;
            OnComplete?.Invoke();
        }
    }
}