using System;
using System.Collections.Generic;
using System.Linq;
using HeroesFlight.Common.Feat;
using HeroesFlight.System.FileManager.Enum;
using HeroesFlight.System.FileManager.Model;
using HeroesFlight.System.UI.FeatsTree;
using HeroesFlight.System.Utility.UI;
using TMPro;
using UISystem;
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

    public class TraitTreeMenu : BaseMenu<TraitTreeMenu>
    {
        [SerializeField] private CanvasGroup thisCG, requirementsCG, errorMessageCG;
        [SerializeField] private TextMeshProUGUI TreeNameText, requirementsText, errorMessageText, availablePointsText;
        [SerializeField] private Color NotUnlockableColor, MaxRankColor;
        [SerializeField] private GameObject NodeSlotPrefabActive;
        [SerializeField] private GameObject TierSlotPrefab;
        [SerializeField] private Transform TierSlotsParent;
        [SerializeField] private GameObject TreeNodeLinePrefab;
        [Header("Hooks")] [SerializeField] private Button exitButton;
        [SerializeField] private Button errorButton;
        [SerializeField] private TraitPopup popup;
        [SerializeField] private ScrollRect scroll;
        [SerializeField] private ClickNotifier notifier;
        [SerializeField] private Button diceInfoButton;
        private float nodeStartOffset = 0.35f;
        private float nodeDistanceOffsetX = 0.95f;
        private float nodeDistanceOffsetY = 0.95f;
        private float nodeDistanceOffsetBonusPerTier = 0.25f;


        private List<TREE_UI_DATA> treeUIData = new();
        private readonly List<GameObject> curTreesTiersSlots = new();
        private readonly List<GameObject> curNodeSlots = new();
        private TraitTreeModel currentTree;
        private GridLayoutGroup tierLayout;
        private GridLayoutGroup contentLayout;
        private TraitModel selectedModel;
        public event Action<TraitModificationEventModel> OnTraitModificationRequest;
        public event Action DiceInfoRequest;


        protected override void Awake()
        {
            base.Awake();
            scroll.onValueChanged.AddListener(HandleScroll);
            notifier.OnClicked += () =>
            {
                Debug.Log("Clicked");
                if (popup.State != TraitPopupState.Disabled)
                    popup.HidePopup();
            };
            popup.OnTraitModificationRequest += TransferTraitModificationRequest;
            popup.OnClose += () =>
            {
                selectedModel = null;
            };
            errorButton.onClick.AddListener(() => { ToggleCanvasGroup(errorMessageCG, false); });
        }

        private void TransferTraitModificationRequest(TraitModificationEventModel request)
        {
            OnTraitModificationRequest?.Invoke(request);
        }


        public void UpdateTreeView(TraitTreeModel tree)
        {
           // OnCreated();
            if (tree == null) return;

            currentTree = tree;
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
                {
                    holder.Init(t1);
                    holder.OnCLicked += HandleNodeClicked;
                }
                else
                    holder.InitHide();

                curNodeSlots.Add(newAb);
            }

            InitTalentTreeLines(tree);
            if (selectedModel != null)
            {
                Debug.Log("Popup was open before update");
                Debug.Log(tree.Data[selectedModel.Id].State);
                popup.ShowPopup(Vector2.zero, TraitPopupState.Enabled,
                    tree.Data[selectedModel.Id]);
            }
        }

        public void ShowErrorMessage(string error)
        {
            errorMessageText.text = error;
            ToggleCanvasGroup(errorMessageCG, true);
        }

        public override void ResetMenu()
        {
        }

        public override void OnCreated()
        {
            contentLayout = TierSlotsParent.GetComponent<GridLayoutGroup>();
            tierLayout = TierSlotPrefab.GetComponent<GridLayoutGroup>();
            // nodeDistanceOffsetBonusPerTier = contentLayout.spacing.y / 100;
            nodeStartOffset = tierLayout.cellSize.x/ 200 ;
            nodeDistanceOffsetX = tierLayout.spacing.x / 100;
            nodeDistanceOffsetY = tierLayout.spacing.y / 100;
            nodeDistanceOffsetBonusPerTier = contentLayout.spacing.y/ 50;
            exitButton.onClick.AddListener(Close);
            diceInfoButton.onClick.AddListener(() =>
            {
                DiceInfoRequest?.Invoke();
            });
        }

        public override void OnOpened()
        {
            Show();
            notifier.enabled = true;
        }

        public override void OnClosed()
        {
            selectedModel = null;
            notifier.enabled = false;
            Hide();
        }

        void ClearAllTiersData()
        {
            foreach (var t in curNodeSlots)
                Destroy(t);

            curNodeSlots.Clear();

            foreach (var t in curTreesTiersSlots)
                Destroy(t);

            curTreesTiersSlots.Clear();
            treeUIData.Clear();
        }

        void HandleNodeClicked(TreeNodeClickedEvent eventData)
        {
            selectedModel = eventData.TargetModel;
            popup.ShowPopup(eventData.Position, TraitPopupState.Enabled, eventData.TargetModel);
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
                        GenerateLine(t.Value, traitModel, nodeTransform, t.Value.State == TraitModelState.Unlocked);
                    }
                }
            }
        }

        void GenerateLine(TraitModel traitModel, TraitModel traitModel1, Transform nodeTransform,
            bool tIsFeatUnlocked)
        {
            var otherTierSlot = GetNodeTierSlotIndex(traitModel);
            var thisTierSlot = GetNodeTierSlotIndex(traitModel1);

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

                if (slotDifference < 0)
                {
                    otherNodeIsLeft = true;
                }
            }
            else
            {
                slotDifference = 0;
            }

            var newTreeNodeLine = Instantiate(TreeNodeLinePrefab, nodeTransform);
            var lineREF = newTreeNodeLine.GetComponent<UILineRenderer>();

            //Debug.Log(   $"Tier difference {tierDifference} , slotdifference {slotDifference} and other node is left {otherNodeIsLeft} from {traitModel.Id}");
            HandleLine(tierDifference, slotDifference, lineREF, thisAbTier, otherAbTier, otherNodeIsLeft);

            lineREF.color = tIsFeatUnlocked ? MaxRankColor : NotUnlockableColor;
        }

        void HandleLine(int tierDifference, int slotDifference, UILineRenderer lineREF, int thisTier, int otherTier,
            bool isLeft)
        {
            if (slotDifference == 0)
            {
                // straight line up
                lineREF.points.Clear();
                lineREF.points.Add(new Vector2(0, nodeStartOffset));
                var yOffset = nodeDistanceOffsetY * tierDifference;
                if (tierDifference < -1)
                {
                    yOffset += tierDifference * nodeDistanceOffsetBonusPerTier + nodeStartOffset * (tierDifference + 1);
                }
                else
                {
                    yOffset += tierDifference * nodeDistanceOffsetBonusPerTier;
                }

                if (yOffset < 0)
                    yOffset = Mathf.Abs(yOffset);
                else
                    yOffset = -yOffset;
                lineREF.points.Add(new Vector2(0, yOffset));
            }
            else
            {
                lineREF.points.Clear();
                if (tierDifference == 0)
                {
                    // straight line lef or right
                    var secondXOffset = nodeDistanceOffsetX * Mathf.Abs(slotDifference);
                    if (Mathf.Abs(slotDifference) > 0)
                    {
                        secondXOffset += nodeStartOffset * 2 * Mathf.Abs(slotDifference);
                        secondXOffset -= nodeStartOffset;
                    }

                    if (isLeft)
                    {
                        lineREF.points.Add(new Vector2(-nodeStartOffset, 0));


                        lineREF.points.Add(new Vector2(-secondXOffset, 0));
                    }
                    else
                    {
                        lineREF.points.Add(new Vector2(nodeStartOffset, 0));

                        lineREF.points.Add(new Vector2(secondXOffset, 0));
                    }
                }
                else
                {
                    var secondXOffset = nodeDistanceOffsetX * Mathf.Abs(slotDifference);
                    if (Mathf.Abs(slotDifference) > 0)
                    {
                        secondXOffset += nodeStartOffset * 2 * Mathf.Abs(slotDifference);
                    }

                    //3 points 
                    if (isLeft)
                    {
                        lineREF.points.Add(new Vector2(-nodeStartOffset, 0));

                        lineREF.points.Add(new Vector2(-secondXOffset, 0));


                        var yOffset = nodeDistanceOffsetY * tierDifference;
                        if (tierDifference < -1)
                        {
                            yOffset += tierDifference * nodeDistanceOffsetBonusPerTier +
                                       nodeStartOffset * (tierDifference + 1);
                        }
                        else
                        {
                            yOffset += tierDifference * nodeDistanceOffsetBonusPerTier;
                        }

                        if (yOffset < 0)
                            yOffset = Mathf.Abs(yOffset);
                        else
                            yOffset = -yOffset;
                        lineREF.points.Add(new Vector2(-secondXOffset, yOffset));
                    }

                    else
                    {
                        lineREF.points.Add(new Vector2(nodeStartOffset, 0));

                        lineREF.points.Add(new Vector2(secondXOffset, 0));
                        var yOffset = nodeDistanceOffsetY * tierDifference;
                        if (tierDifference < -1)
                        {
                            yOffset += tierDifference * nodeDistanceOffsetBonusPerTier +
                                       nodeStartOffset * (tierDifference + 1);
                        }
                        else
                        {
                            yOffset += tierDifference * nodeDistanceOffsetBonusPerTier;
                        }

                        if (yOffset < 0)
                            yOffset = Mathf.Abs(yOffset);
                        else
                            yOffset = -yOffset;
                        lineREF.points.Add(new Vector2(secondXOffset, yOffset));
                    }
                }
            }
        }

        private int[] GetNodeTierSlotIndex(TraitModel nodeDATA)
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

        void Show()
        {
            ToggleCanvasGroup(thisCG, true);
         //   transform.SetAsLastSibling();
        }

        void Hide()
        {
            scroll.normalizedPosition = new Vector2(0, -1);
          //  transform.SetAsFirstSibling();
            popup.HidePopup();
            ToggleCanvasGroup(thisCG, false);
        }

        void ToggleCanvasGroup(CanvasGroup cg, bool isEnabled)
        {
            gameObject.SetActive(isEnabled);
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


        private void HandleScroll(Vector2 arg0)
        {
            if (popup.State != TraitPopupState.Disabled)
                popup.HidePopup();
        }
    }
}