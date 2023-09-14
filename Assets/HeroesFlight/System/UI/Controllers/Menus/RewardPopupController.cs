using UnityEngine;

namespace UISystem
{
    public class RewardPopupController : BaseMenu<RewardPopupController>
    {
        [SerializeField] AdvanceButton okButton;

        protected override void Awake()
        {
            base.Awake();
            okButton.onClick.AddListener(() =>
            {
                CloseMenu();
            });
        }

        public override void ResetMenu()
        {
        }

        public override void OnCreated()
        {
            
        }

        public override void OnOpened()
        {
           
        }

        public override void OnClosed()
        {
            
        }
    }
}