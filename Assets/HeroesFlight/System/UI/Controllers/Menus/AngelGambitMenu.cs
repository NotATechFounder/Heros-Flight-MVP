using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class AngelGambitMenu : BaseMenu<AngelGambitMenu>
    {
        public Action<AngelCardSO> OnCardSelected;
        public Func<AngelCardSO,AngelCard> CardExit;

        [SerializeField] private Transform container;

        [Header("Card Buttons")]
        [SerializeField] private AdvanceButton buffCardButton;
        [SerializeField] private AdvanceButton debuffCardButton;
        [SerializeField] private AdvanceButton blankCardButton;
        [SerializeField] private AdvanceButton continueButton;

        [Header("Card Colors")]
        [SerializeField] private Color buffCardColor;
        [SerializeField] private Color debuffCardColor;
        [SerializeField] private Color blankCardColor;

        [Header("Card Reveal")]
        [SerializeField] private GameObject cardRevealPanel;
        [SerializeField] private Transform cardToReveal;
        [SerializeField] private Image cardBg;
        [SerializeField] private TextMeshProUGUI cardNameDisplay;
        [SerializeField] private TextMeshProUGUI cardTierDisplay;
        [SerializeField] private TextMeshProUGUI cardDescriptionDisplay;
        [SerializeField] private Image cardImageDisplay;
        [SerializeField] private GameObject[] cardRevealProperties;

        [Header("Card List")]
        [SerializeField] private AngelCardSO[] angelCardSOList;

        AngelCardSO selectedCard = null;
        private float totalChance;
        private List<AngelCardSO> validAngelCardSOList;

        JuicerRuntime openEffectBG;
        JuicerRuntime openEffectContainer;
        JuicerRuntime closeEffectBG;

        JuicerRuntime buffCardEffect;
        JuicerRuntime debuffCardEffect;
        JuicerRuntime spinCardEffect;

        public override void OnCreated()
        {

            container.localScale = Vector3.zero;

            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            openEffectContainer = container.JuicyScale(Vector3.one, 0.5f)
                                            .SetEase(Ease.EaseInQuint)
                                            .SetDelay(0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f).SetDelay(.15f);
            closeEffectBG.SetOnComplected(CloseMenu);

            buffCardEffect = buffCardButton.transform.JuicyScale(Vector3.one * 1.2f, .5f).SetEase(Ease.EaseOutSine).SetLoop(-1);
            debuffCardEffect = debuffCardButton.transform.JuicyScale(Vector3.one * 1.2f, .5f).SetEase(Ease.EaseOutSine).SetLoop(-1);

            spinCardEffect = cardToReveal.transform.JuicyRotate(Vector3.up * 360, .25f).SetEase(Ease.Linear).SetLoop(5, LoopType.Incremental);
            spinCardEffect.SetOnComplected(() => ToggleCardRevealProperties(true));

            buffCardButton.onClick.AddListener(() => GenerateRandomCards(AngelCardType.Buff));

            debuffCardButton.onClick.AddListener(() => GenerateRandomCards(AngelCardType.Debuff));

            blankCardButton.onClick.AddListener(Close);

            continueButton.onClick.AddListener(()=>
            {
                ActivateNewCard();
            });

            ToggleCardRevealProperties (false);
        }

        public override void OnOpened()
        {
            cardRevealPanel.SetActive(false);
            openEffectBG.Start(() => canvasGroup.alpha = 0);
            buffCardEffect.Start();
            debuffCardEffect.Start();
            openEffectContainer.Start(() => container.localScale = Vector3.zero);
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
            buffCardButton.transform.localScale = Vector3.one;
            debuffCardButton.transform.localScale = Vector3.one;
        }

        public void GetValidCardSO(AngelCardType angelCardType)
        {
            validAngelCardSOList = new List<AngelCardSO>();

            foreach (AngelCardSO angelCardSO in angelCardSOList)
            {
                if (angelCardSO.CardType == angelCardType)
                {
                    AngelCard existingCard = CardExit?.Invoke(angelCardSO);

                    if (existingCard != null)
                    {
                        if (existingCard.tier == AngelCardTier.Six)
                        {
                            Debug.Log("All tiers are completed");
                            continue;
                        }
                    }
                    totalChance += angelCardSO.Chance;
                    validAngelCardSOList.Add(angelCardSO);
                }
            }
        }

        public void GenerateRandomCards(AngelCardType angelCardType)
        {
            totalChance = 0;
            selectedCard = null;

            GetValidCardSO(angelCardType);

            float randomChance = UnityEngine.Random.Range(0, totalChance);

            foreach (AngelCardSO angelCardSO in validAngelCardSOList)
            {
                randomChance -= angelCardSO.Chance;

                if (randomChance <= 0)
                {
                    selectedCard = angelCardSO;
                    break;
                }
            }

            if(selectedCard == null)
            {
                Debug.Log("No card was selected");
                return;
            }

            DisplayCard();
        }   
        
        public void DisplayCard()
        {
            cardRevealPanel.SetActive(true);

            AngelCard existingCard = CardExit?.Invoke(selectedCard);

            if (existingCard != null)
            {
                cardTierDisplay.text = "Tier : " + (existingCard.tier + 1).ToString();
                cardDescriptionDisplay.text = selectedCard.GetDescription(existingCard.tier + 1);
            }
            else
            {
                cardTierDisplay.text = "Tier : " + AngelCardTier.One.ToString();
                cardDescriptionDisplay.text = selectedCard.GetDescription(AngelCardTier.One);
            }

            cardNameDisplay.text = selectedCard.CardName;
            cardImageDisplay.sprite = selectedCard.CardImage;
            cardBg.color = selectedCard.CardType == AngelCardType.Buff ? buffCardColor : debuffCardColor;

            spinCardEffect.Start();
        }

        public void ActivateNewCard ()
        {
            OnCardSelected?.Invoke(selectedCard);
            ToggleCardRevealProperties(false);
            Close();
        }

        public void ToggleCardRevealProperties(bool toggle)
        {
            foreach (GameObject cardRevealProperty in cardRevealProperties)
            {
                cardRevealProperty.SetActive(toggle);
            }
        }
    }
}

