using Pelumi.Juicer;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UISystem
{
    public class AngelPermanetCardMenu : BaseMenu<AngelPermanetCardMenu>
    {
        [Header("Permanet Cards")]
        [SerializeField] private PermanetCardUI[] permanetCards;
        [SerializeField] private CardEffectUI cardEffectUI;
        [SerializeField] private TextMeshProUGUI increaseText;
        [SerializeField] private float visibileTime;

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;
        WaitForSeconds waitForSeconds;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);
            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnComplected(CloseMenu);

            waitForSeconds = new WaitForSeconds(visibileTime);
        }

        public override void OnOpened()
        {
          openEffectBG.Start(() => canvasGroup.alpha = 0);
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
        }

        public override void ResetMenu()
        {
           
        }

        public void AcivateCardPermanetEffect(AngelCard angelCard)
        {
            Open();

            foreach (PermanetCardUI permanetCardUI in permanetCards)
            {
                if (permanetCardUI.IsCardSet && permanetCardUI.AngelCard.angelCardSO == angelCard.angelCardSO)
                {
                    increaseText.text = $"+{angelCard.GetValueDifference()}";
                    cardEffectUI.MoveTo(permanetCardUI.transform, () =>
                    {
                        permanetCardUI.SetCard(angelCard,true);
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
                        permanetCardUI.SetCard(angelCard,false);
                        StartCoroutine(Finished());
                    });
                    break;
                }
            }
        }

        public IEnumerator Finished()
        {
            yield return waitForSeconds;
            Close();
        }
    }
}
