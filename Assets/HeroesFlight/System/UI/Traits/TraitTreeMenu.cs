using System;
using System.Collections.Generic;
using System.Linq;
using HeroesFlight.System.FileManager.Model;
using HeroesFlight.System.UI.FeatsTree;
using HeroesFlight.System.Utility.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HeroesFlight.System.UI.Traits
{
    [Serializable]
    public class TREE_UI_DATA
    {
        public GridLayoutGroup GridLayoutRef;
        public List<TraitModel> slotsDATA = new();
        public List<TreeNodeHolder> nodesREF = new();
    }

    public class TraitTreeMenu : MonoBehaviour
    {
        [SerializeField] private CanvasGroup thisCG, requirementsCG, errorMessageCG;
        [SerializeField] private TextMeshProUGUI TreeNameText, requirementsText, errorMessageText, availablePointsText;

        [SerializeField] private Color NotUnlockableColor, MaxRankColor;

        [SerializeField] private GameObject NodeSlotPrefabActive;

        [SerializeField] private GameObject TierSlotPrefab;
        [SerializeField] private Transform TierSlotsParent;
        [SerializeField] private List<TREE_UI_DATA> treeUIData = new List<TREE_UI_DATA>();
        [SerializeField] private GameObject TreeNodeLinePrefab;

        [SerializeField] private float nodeXStartOffset = 0.35f;
        [SerializeField] private float nodeDistanceOffset = 0.95f;
        [SerializeField] private float nodeDistanceOffsetBonusPerTier = 0.25f;
        [SerializeField] private float nodeOffsetWhenAbove = 0.25f;
        [SerializeField] private float nodeDistanceOffsetWhenAbove = 0.95f;
        [SerializeField] private float nodeDistanceOffsetBonusPerTierWhenAbove = 0.25f;


        private readonly List<GameObject> curTreesTiersSlots = new List<GameObject>();
        private readonly List<GameObject> curNodeSlots = new List<GameObject>();
        private TraitTreeModel currentTree;


        private void ClearAllTiersData()
        {
            foreach (var t in curNodeSlots)
                Destroy(t);

            curNodeSlots.Clear();

            foreach (var t in curTreesTiersSlots)
                Destroy(t);

            curTreesTiersSlots.Clear();
            treeUIData.Clear();
        }

        public void InitTree(TraitTreeModel tree)
        {
            if (tree == null) return;

            currentTree = tree;
            Show();
            ClearAllTiersData();

            // TreeNameText.text = tree.entryDisplayName;

            for (var i = 0; i < tree.MaxTier; i++)
            {
                var newTierSlot = Instantiate(TierSlotPrefab, TierSlotsParent);
                var newTierData = new TREE_UI_DATA { GridLayoutRef = newTierSlot.GetComponent<GridLayoutGroup>() };
                for (var x = 0; x < tree.RowsPerTier; x++)
                {
                    newTierData.slotsDATA.Add(new TraitModel());
                    foreach (var t in tree.Data.Values.Where(t => t.Tier == i + 1).Where(t => t.Slot == x + 1))
                    {
                        newTierData.slotsDATA[x] = t;
                    }
                }

                treeUIData.Add(newTierData);
                curTreesTiersSlots.Add(newTierSlot);
            }

            foreach (var t in treeUIData)
            foreach (var t1 in t.slotsDATA)
            {
                var newAb = Instantiate(NodeSlotPrefabActive, t.GridLayoutRef.transform);
                var holder = newAb.GetComponent<TreeNodeHolder>();
                t.nodesREF.Add(holder);
                if (t1.Id != string.Empty)
                    holder.Init(t1);
                else
                    holder.InitHide();

                curNodeSlots.Add(newAb);
            }

            // availablePointsText.text =
            //     "Points: " + Character.Instance.getTreePointsAmountByPoint(tree.treePointAcceptedID);

            InitTalentTreeLines(tree);
        }


         void InitTalentTreeLines(TraitTreeModel tree)
        {
            foreach (var t in treeUIData)
                for (var x = 0; x < t.nodesREF.Count; x++)
                    if (t.nodesREF[x].used)
                        InitTalentTreeNodeLines(tree, t.slotsDATA[x], t.nodesREF[x].transform);
        }

         void InitTalentTreeNodeLines(TraitTreeModel tree, TraitModel traitModel, Transform nodeTransform)
        {
         
            var isAbilityNotNull = traitModel.Id != string.Empty;

            foreach (var t in tree.Data)
            {
               
                if (t.Value.BlockingId == string.Empty) continue;
                TreeNodeHolder otherNodeREF;
           
                if (isAbilityNotNull && t.Value.BlockingId == traitModel.Id)
                {
                    otherNodeREF = GetAbilityNodeREF(traitModel.Id);
                 
                    if (otherNodeREF != null)
                    {
                      
                        GenerateLine(t.Value, traitModel, nodeTransform, t.Value.IsFeatUnlocked);
                    }
                  
                }
            }
        }

         void GenerateLine(TraitModel traitModel, TraitModel traitModel1, Transform nodeTransform,
            bool tIsFeatUnlocked)
        {
            var otherTierSlot = getNodeTierSlotIndex(traitModel);
            var thisTierSlot = getNodeTierSlotIndex(traitModel1);
            Debug.Log($"Slot index for {traitModel1.Id} is {thisTierSlot[0]}:{thisTierSlot[1]}");
            Debug.Log($"Slot index for {traitModel.Id} is {otherTierSlot[0]}:{otherTierSlot[1]}");
            var otherAbTier = otherTierSlot[0];
            var otherAbSlot = otherTierSlot[1];
            var thisAbTier = thisTierSlot[0];
            var thisAbSlot = thisTierSlot[1];

            
            var tierDifference = GetTierDifference(otherAbTier - thisAbTier);


            var slotDifference = 0;
            var otherNodeIsLeft = false;
            if (otherAbSlot != thisAbSlot)
            {
                slotDifference = otherAbSlot - thisAbSlot;
                Debug.Log($"Slot difference {slotDifference}");
                if (slotDifference < 0)
                {
                   // slotDifference = Mathf.Abs(slotDifference);
                    otherNodeIsLeft = true;
                }
                // else
                // {
                //     slotDifference = -slotDifference;
                // }
                Debug.Log($"Slot difference {slotDifference}");
            }
            else
            {
                slotDifference = 0;
            }

            var newTreeNodeLine = Instantiate(TreeNodeLinePrefab, nodeTransform);
            var lineREF = newTreeNodeLine.GetComponent<UILineRenderer>();
            
            Debug.Log($"Tier difference {tierDifference } , slotdifference {slotDifference} and other node is left {otherNodeIsLeft} from {traitModel.Id}");
            HandleLine(tierDifference, slotDifference, lineREF, thisAbTier, otherAbTier, otherNodeIsLeft);

            lineREF.color = tIsFeatUnlocked ? MaxRankColor : NotUnlockableColor;
        }

        void HandleLine(int tierDifference, int slotDifference, UILineRenderer lineREF, int thisTier, int otherTier,
            bool isLeft)
        {
            if (slotDifference == 0)
            {
                // straight line
                lineREF.points.Clear();
                if (thisTier < otherTier)
                {
                    lineREF.points.Add(new Vector2(0, nodeXStartOffset));
                    var YOffset = nodeDistanceOffset * tierDifference;
                    YOffset += tierDifference * nodeDistanceOffsetBonusPerTier;
                    if (YOffset < 0)
                        YOffset = Mathf.Abs(YOffset);
                    else
                        YOffset = -YOffset;
                    lineREF.points.Add(new Vector2(0, YOffset));
                }
                else
                {
                    var y = nodeXStartOffset;
                    y += nodeOffsetWhenAbove;
                    y = -y;
                    lineREF.points.Add(new Vector2(y, 0));

                    var YOffset = nodeDistanceOffsetWhenAbove * tierDifference;
                    YOffset += tierDifference * nodeDistanceOffsetBonusPerTierWhenAbove;
                    if (YOffset < 0)
                        YOffset = Mathf.Abs(YOffset);
                    else
                        YOffset = -YOffset;
                    lineREF.points.Add(new Vector2(YOffset, 0));
                }
            }
            else
            {
                // line requires 3 points
                lineREF.points.Clear();

                if (isLeft)
                    lineREF.points.Add(new Vector2(-nodeXStartOffset, 0));
                else
                    lineREF.points.Add(new Vector2(nodeXStartOffset, 0));
                var XOffset = nodeDistanceOffset * slotDifference;
                if (XOffset < 0)
                    XOffset = Mathf.Abs(XOffset);
                else
                    XOffset = -XOffset;
                lineREF.points.Add(new Vector2(-XOffset, 0));
                var YOffset = nodeDistanceOffset * tierDifference;
                YOffset += tierDifference * nodeDistanceOffsetBonusPerTier;
                if (YOffset < 0)
                    YOffset = Mathf.Abs(YOffset);
                else
                    YOffset = -YOffset;
                lineREF.points.Add(new Vector2(-XOffset, YOffset));
            }
        }

        private int[] getNodeTierSlotIndex(TraitModel nodeDATA)
        {
            var tierSlot = new int[2];
            for (var i = 0; i < treeUIData.Count; i++)
            for (var x = 0; x < treeUIData[i].slotsDATA.Count; x++)
                if (treeUIData[i].slotsDATA[x].Id == nodeDATA.Id)
                {
                    tierSlot[0] = i;
                    tierSlot[1] = x;
                    return tierSlot;
                }

            return tierSlot;
        }

        private int GetTierDifference(int initialValue)
        {
            if (initialValue < 0)
                return Mathf.Abs(initialValue);
            return -initialValue;
        }

        private TreeNodeHolder GetAbilityNodeREF(string id)
        {
            foreach (var t in treeUIData)
                for (var x = 0; x < t.slotsDATA.Count; x++)
                    if (t.slotsDATA[x].Id == id)
                        return t.nodesREF[x];

            return null;
        }

        public  void Show()
        {
            ToggleCanvasGroup(thisCG, true);
            transform.SetAsLastSibling();
        }

        public void Hide()
        {
            transform.SetAsFirstSibling();
            ToggleCanvasGroup(thisCG, false);
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
    }
}