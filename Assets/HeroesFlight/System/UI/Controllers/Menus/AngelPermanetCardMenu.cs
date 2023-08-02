using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;

namespace UISystem
{
    public class AngelPermanetCardMenu : BaseMenu<AngelPermanetCardMenu>
    {
        [Header("Permanet Cards")]
        [SerializeField] private PermanetCardUI[] permanetCards;
        [SerializeField] private CardEffectUI cardEffectUI;

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);
            openEffectBG.SetOnStart(() => canvasGroup.alpha = 0);
            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnComplected(CloseMenu);
        }

        public override void OnOpened()
        {
          openEffectBG.Start();
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
                    cardEffectUI.MoveTo(permanetCardUI.transform, () =>
                    {
                        permanetCardUI.SetCard(angelCard);
                        StartCoroutine(Finished());
                    });
                    return;
                }
            }

            foreach (PermanetCardUI permanetCardUI in permanetCards)
            {
                if (!permanetCardUI.IsCardSet)
                {
                    cardEffectUI.MoveTo(permanetCardUI.transform, () =>
                    {
                        permanetCardUI.SetCard(angelCard);
                       StartCoroutine(Finished());
                    });
                    break;
                }
            }
        }

        public IEnumerator Finished()
        {
            yield return new WaitForSeconds(3f);
            Close();
            Debug.Log("Finished");
        }
    }
}
