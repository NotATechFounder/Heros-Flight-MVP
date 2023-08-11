using Pelumi.Juicer;
using StansAssets.Foundation.Async;
using System.Collections;
using System.ComponentModel;
using TMPro;
using UnityEngine;

namespace UISystem
{
    public class AngelPermanetCardMenu : BaseMenu<AngelPermanetCardMenu>
    {
        [SerializeField] private Transform container;

        [Header("Permanet Cards")]
        [SerializeField] private PermanetCardUI[] permanetCards;
        [SerializeField] private CardEffectUI cardEffectUI;
        [SerializeField] private TextMeshProUGUI increaseText;
        [SerializeField] private float visibileTime;

        JuicerRuntime openEffectBG;
        JuicerRuntime openEffectContainer;

        JuicerRuntime closeEffectBG;
        JuicerRuntime closeEffectContainer;

        WaitForSeconds waitForSeconds;

        public override void OnCreated()
        {
            container.localScale = Vector3.zero;

            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            openEffectContainer = container.JuicyScale(Vector3.one, 0.15f) .SetEase(Ease.EaseInQuart).SetDelay(0.15f);

            closeEffectContainer = container.JuicyScale(Vector3.zero, 0.15f).SetEase(Ease.EaseInQuart);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f).SetDelay(0.15f);
            closeEffectBG.SetOnComplected(CloseMenu);

            waitForSeconds = new WaitForSeconds(visibileTime);
        }

        public override void OnOpened()
        {
            openEffectBG.Start();
            openEffectContainer.Start();
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
            closeEffectContainer.Start();
        }

        public override void ResetMenu()
        {
            foreach(PermanetCardUI permanetCardUI in permanetCards)
            {
                permanetCardUI.ResetCard();
            }
        }

        public void AcivateCardPermanetEffect(AngelCard angelCard)
        {
            Open();

            CoroutineUtility.WaitForSeconds(.5f, () =>
            {
                foreach (PermanetCardUI permanetCardUI in permanetCards)
                {
                    if (permanetCardUI.IsCardSet && permanetCardUI.AngelCard.angelCardSO == angelCard.angelCardSO)
                    {
                        increaseText.text = $"+{angelCard.GetValueDifference()}";
                        cardEffectUI.MoveTo(permanetCardUI.transform, () =>
                        {
                            permanetCardUI.SetCard(angelCard, true);
                            StartCoroutine(Finished());
                        });
                        return;
                    }
                }

                foreach (PermanetCardUI permanetCardUI in permanetCards)
                {
                    if (!permanetCardUI.IsCardSet)
                    {
                        increaseText.text = "";
                        cardEffectUI.MoveTo(permanetCardUI.transform, () =>
                        {
                            permanetCardUI.SetCard(angelCard, false);
                            StartCoroutine(Finished());
                        });
                        break;
                    }
                }
            });      
        }

        public IEnumerator Finished()
        {
            yield return waitForSeconds;
            Close();
        }
    }
}
