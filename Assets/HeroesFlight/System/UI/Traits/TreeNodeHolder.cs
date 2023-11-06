using HeroesFlight.System.FileManager.Model;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HeroesFlight.System.UI.FeatsTree
{
    public class
        TreeNodeHolder : MonoBehaviour, IPointerClickHandler 
    {
        [SerializeField] private Image border, rankBorder, costBorder;
        [SerializeField] private Image icon, background;
        [SerializeField] private TextMeshProUGUI curRankText, costText;
        [SerializeField] private CanvasGroup thisCG, costCG;
        [SerializeField] private Color UnlockedColor, NotUnlockedColor, NotUnlockableColor, MaxRankColor;

        [SerializeField] private Sprite UnlockedImage,
            NotUnlockedImage,
            NotUnlockableImage,
            MaxRankImage;
           

        private TraitModel currentModel;

        public bool used;

        public void Init(TraitModel model)
        {
            used = true;

            EnableAllElements();
            var unlockCost = 0;
            var rank = -1;
            var maxRank = -1;
            bool isKnown = false;


            icon.sprite = model.Visual;
            background.sprite = null;
            rank = model.IsFeatUnlocked ? 1 : 0;
            isKnown = model.IsFeatUnlocked;
            unlockCost = model.GoldCost;
           

            HandleBorders(model.IsFeatUnlocked,model.IsFeatUnlockable);
            HandleRank(model.IsFeatUnlocked, unlockCost,
                model.IsFeatUnlockable, rank);
          //  SetCurRankText(displayRank + " / " + maxRank);
        }

         void HandleRank(bool maxRank, int cost, bool canBeUnlocked, int rank)
        {
            if (maxRank)
            {
                ToggleCanvasGroup(costCG,false);
                border.color = MaxRankColor;
                border.sprite =  MaxRankImage;
                rankBorder.color = MaxRankColor;
            }
            else
            {
                costText.text = cost.ToString();
                if (canBeUnlocked)
                {
                    costBorder.color = NotUnlockedColor;
                    costText.color = NotUnlockedColor;
                }
                else
                {
                    costBorder.color = NotUnlockableColor;
                    costText.color = NotUnlockableColor;
                }
            }
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

         void HandleBorders( bool known,bool isFeatUnlockable)
        {
            if (known)
            {
                border.color = UnlockedColor;
                rankBorder.color = UnlockedColor;
                border.sprite =  UnlockedImage;
            }
            else
            {
                if (isFeatUnlockable)
                {
                    border.color = NotUnlockedColor;
                    rankBorder.color = NotUnlockedColor;
                    border.sprite =  NotUnlockedImage;
                }
                else
                {
                    border.color = NotUnlockableColor;
                    rankBorder.color = NotUnlockableColor;
                    border.sprite =  NotUnlockableImage;
                }
               
            }
        }

      

        void EnableAllElements()
        {
            ToggleCanvasGroup(costCG,true);
           
        }

        private void SetCurRankText(string text)
        {
            curRankText.text = text;
        }

        public void InitHide()
        {
           used = false;
           ToggleCanvasGroup(thisCG,false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // if (eventData.button == PointerEventData.InputButton.Right)
            // {
            //     RankDown();
            // }
            // else
            // {
            //     RankUp();
            // }
        }

   
    }
}