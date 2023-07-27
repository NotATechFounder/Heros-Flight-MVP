using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem;
using UnityEngine;

namespace UISystem
{
    public class AngelGambitMenu : BaseMenu<AngelGambitMenu>
    {
        [Header("Cards")]
        [SerializeField] private AngelCardUI buffCard;
        [SerializeField] private AngelCardUI debuffCard;
        [SerializeField] private AngelCardUI blankCard;
        [SerializeField] private AngelCardProperties selectedCardProperties;
        [SerializeField] private List<AngelCardSO> angelCardSOList;

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        JuicerRuntime buffCardEffect;
        JuicerRuntime debuffCardEffect;

        private void Start()
        {
            OnCreated();
            Open();

            GenerateRandomCards();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnClosed();
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                OnOpened();
            }
        }

        public override void OnCreated()
        {
            canvasGroup.alpha = 0;

            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);
            openEffectBG.SetOnStart(() => canvasGroup.alpha = 0);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f).SetDelay(.15f);
            //closeEffectBG.SetOnComplected(CloseMenu);

            buffCardEffect = buffCard.transform.JuicyScale(Vector3.one * 1.2f, .5f).SetEase(Ease.EaseOutSine).SetLoop(-1);
            debuffCardEffect = debuffCard.transform.JuicyScale(Vector3.one * 1.2f, .5f).SetEase(Ease.EaseOutSine).SetLoop(-1);
        }

        public override void OnOpened()
        {
            openEffectBG.Start();
            buffCardEffect.Start();
            debuffCardEffect.Start();
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
            buffCardEffect.Pause();
            debuffCardEffect.Pause();
            ResetMenu();
        }

        public override void ResetMenu()
        {
            buffCard.transform.localScale = Vector3.one;
            debuffCard.transform.localScale = Vector3.one;
        }

        public void GenerateRandomCards()
        {
            int buffCardIndex = UnityEngine.Random.Range(0, angelCardSOList.Count);
            AngelCardSO angelCardSO = angelCardSOList[buffCardIndex];
            buffCard.Init(angelCardSO);
        }    
    }
}

