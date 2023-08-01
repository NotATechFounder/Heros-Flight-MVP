using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class AngelGambitMenu : BaseMenu<AngelGambitMenu>
    {
        public event Action<AngelCardSO> OnCardSelected;
        public Func<AngelCard> CardExit;

        [Header("Permanet Cards")]
        [SerializeField] private PermanetCardUI[] permanetCards;

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

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        JuicerRuntime buffCardEffect;
        JuicerRuntime debuffCardEffect;
        JuicerRuntime spinCardEffect;

        private void Start()
        {
            OnCreated();
            Open();
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

            if (Input.GetKeyDown(KeyCode.T))
            {
                spinCardEffect.Start();
            }
        }

        public override void OnCreated()
        {
            canvasGroup.alpha = 0;

            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);
            openEffectBG.SetOnStart(() => canvasGroup.alpha = 0);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f).SetDelay(.15f);
            //closeEffectBG.SetOnComplected(CloseMenu);

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
            buffCardButton.transform.localScale = Vector3.one;
            debuffCardButton.transform.localScale = Vector3.one;
        }

        public void GenerateRandomCards(AngelCardType angelCardType)
        {
            float totalChance = 0;

            foreach (AngelCardSO angelCardSO in angelCardSOList)
            {
                if (angelCardSO.CardType == angelCardType)
                {
                    totalChance += angelCardSO.Chance;
                }
            }

            float randomChance = UnityEngine.Random.Range(0, totalChance);

            foreach (AngelCardSO angelCardSO in angelCardSOList)
            {
                if (angelCardSO.CardType == angelCardType)
                {
                    randomChance -= angelCardSO.Chance;

                    if (randomChance <= 0)
                    {
                        selectedCard = angelCardSO;
                        break;
                    }
                }
            }

            DisplayCard();
        }   
        
        public void DisplayCard()
        {
            cardRevealPanel.SetActive(true);

            AcivateLastCardPermanet();

            AngelCard existingCard = CardExit?.Invoke();

            // To be removed
            existingCard = StatEffectManager.Instance.Exists(selectedCard);

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

        public void AcivateLastCardPermanet()
        {
            AngelCard angelCard = StatEffectManager.Instance.GetActiveAngelCard();
            if (angelCard == null ||angelCard.angelCardSO == null) return;

            foreach (PermanetCardUI permanetCardUI in permanetCards)
            {
                if (permanetCardUI.IsCardSet && permanetCardUI.AngelCard.angelCardSO == angelCard.angelCardSO)
                {
                    permanetCardUI.SetCard(angelCard);
                    return;
                }
            }

            foreach (PermanetCardUI permanetCardUI in permanetCards)
            {
                if (!permanetCardUI.IsCardSet)
                {
                    permanetCardUI.SetCard(StatEffectManager.Instance.GetActiveAngelCard());
                    break;
                }
            }
        }

        public void ActivateNewCard ()
        {
            cardRevealPanel.SetActive(false);
            OnCardSelected?.Invoke(selectedCard);
            ToggleCardRevealProperties(false);
            // To be removed
            StatEffectManager.Instance.ProccessCard(selectedCard);
           // Close();
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

