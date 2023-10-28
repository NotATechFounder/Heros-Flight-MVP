using HeroesFlight.System.UI.Data;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace UISystem
{
    public class UiSpineViewController : MonoBehaviour
    {
        [SpineSkin][SerializeField] string m_BaseSkinReference = "skin-base";
        [SpineSkin][SerializeField] string m_EyeLashesSkinReference = "eyelids/girly";
        [SpineSkin][SerializeField] string m_HairSkinReference;
        [SpineSkin][SerializeField] string m_EyesSkinReference;
        [SpineSkin][SerializeField] string m_LipsSkinReference;
        [SpineSkin][SerializeField] string m_ClothesSkinReference = "clothes/hoodie-orange";
        [SpineSkin][SerializeField] string m_PantsSkinReference = "legs/pants-jeans";
        [SpineSkin][SerializeField] string m_WingsSkinReference = "";
        [SpineSkin][SerializeField] string m_HatSkinReference = "accessories/hat-red-yellow";
        [SpineSkin][SerializeField] string m_WeaponSkinReference;
       
        SkeletonGraphic m_SkeletonAnimation;

        // This "naked body" skin will likely change only once upon character creation,
        // so we store this combined set of non-equipment Skins for later re-use.
        Skin m_CharacterSkin;
        Skin m_BaseSkin;

       
        void Awake()
        {
            m_SkeletonAnimation = GetComponent<SkeletonGraphic>();
        }
      

        void UpdateCombinedSkin()
        {
            Skeleton skeleton = m_SkeletonAnimation.Skeleton;
            Skin resultCombinedSkin = new Skin("character-combined");

            resultCombinedSkin.AddSkin(m_CharacterSkin);
            //AddEquipmentSkinsTo(resultCombinedSkin);

            skeleton.SetSkin(resultCombinedSkin);
            skeleton.SetSlotsToSetupPose();
         
        }

        void AddEquipmentSkinsTo(Skin resultCombinedSkin)
        {
            Skeleton skeleton = m_SkeletonAnimation.Skeleton;
            SkeletonData skeletonData = skeleton.Data;
            resultCombinedSkin.AddSkin(skeletonData.FindSkin(m_ClothesSkinReference));
            resultCombinedSkin.AddSkin(skeletonData.FindSkin(m_PantsSkinReference));
            if (!string.IsNullOrEmpty(m_WeaponSkinReference))
                resultCombinedSkin.AddSkin(skeletonData.FindSkin(m_WeaponSkinReference));
            if (!string.IsNullOrEmpty(m_HatSkinReference))
                resultCombinedSkin.AddSkin(skeletonData.FindSkin(m_HatSkinReference));
        }

        void UpdateCharacterSkin()
        {
            Skeleton skeleton = m_SkeletonAnimation.Skeleton;
            SkeletonData skeletonData = skeleton.Data;
            m_CharacterSkin = new Skin("character-base");

            // Note that the result Skin returned by calls to skeletonData.FindSkin()
            // could be cached once in Start() instead of searching for the same skin
            // every time. For demonstration purposes we keep it simple here.
            m_CharacterSkin.AddSkin(skeletonData.FindSkin(m_BaseSkinReference));
            // m_CharacterSkin.AddSkin(skeletonData.FindSkin(m_LipsSkinReference));
            // m_CharacterSkin.AddSkin(skeletonData.FindSkin(m_EyeLashesSkinReference));
            // m_CharacterSkin.AddSkin(skeletonData.FindSkin(m_EyesSkinReference));
            // m_CharacterSkin.AddSkin(skeletonData.FindSkin(m_HairSkinReference));
            // m_CharacterSkin.AddSkin(skeletonData.FindSkin(m_LipsSkinReference));
            // m_CharacterSkin.AddSkin(skeletonData.FindSkin(m_WingsSkinReference));
        }

       


        public void SetupView(CharacterUiViewData data)
        {
            var viewData = data.CharacterData.GetAppearanceData;
            m_SkeletonAnimation.skeletonDataAsset = data.Skeleton;
            m_SkeletonAnimation.Initialize(true);
            m_BaseSkinReference = viewData.FullSkin;
            // m_EyeLashesSkinReference = viewData.EyesSkinReference;
            // m_HairSkinReference = viewData.HairSkinReference;
            // m_EyesSkinReference = viewData.EyesSkinReference;
            // m_LipsSkinReference = viewData.LipsSkinReference;
            // m_ClothesSkinReference = viewData.ClothesSkinReference;
            // m_PantsSkinReference = viewData.PantsSkinReference;
            // m_WingsSkinReference = viewData.WingsSkinReference;
            // m_HatSkinReference = viewData.HatSkinReference;
            // m_WeaponSkinReference = viewData.WeaponSkinReference;
            UpdateCharacterSkin();
            UpdateCombinedSkin();
            m_SkeletonAnimation.AnimationState.SetAnimation(0,data.CharacterData.CharacterAnimations.IdleAniamtion.Animation,  true);
        }
    }
}