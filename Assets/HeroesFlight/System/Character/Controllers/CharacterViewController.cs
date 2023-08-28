using HeroesFlight.Common;
using HeroesFlight.System.Character.Enum;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using UnityEngine;


namespace HeroesFlight.System.Character
{
    public class CharacterViewController : MonoBehaviour, ICharacterViewController
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
        [SpineSkin][SerializeField] string fullSkin;
        SkeletonAnimation m_SkeletonAnimation;

        // This "naked body" skin will likely change only once upon character creation,
        // so we store this combined set of non-equipment Skins for later re-use.
        Skin m_CharacterSkin;
        Skin m_BaseSkin;

        public Material m_RuntimeMaterial;
        public Texture2D m_RuntimeAtlas;

        void Init()
        {
            m_SkeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
            Skeleton skeleton = m_SkeletonAnimation.Skeleton;
            SkeletonData skeletonData = skeleton.Data;
            foreach (var VARIABLE in skeletonData.Skins)
            {
                Debug.Log(VARIABLE.Name);
            }
            // UpdateCharacterSkin(false);
            // UpdateCombinedSkin();
        }

       

        void UpdateCombinedSkin()
        {
            Skeleton skeleton = m_SkeletonAnimation.Skeleton;
            Skin resultCombinedSkin = new Skin("character-combined");

            resultCombinedSkin.AddSkin(m_CharacterSkin);
            AddEquipmentSkinsTo(resultCombinedSkin);

            skeleton.SetSkin(resultCombinedSkin);
            skeleton.SetSlotsToSetupPose();

            // OptimizeSkin();
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

        void UpdateCharacterSkin(bool isCustomizable)
        {
            Skeleton skeleton = m_SkeletonAnimation.Skeleton;
            SkeletonData skeletonData = skeleton.Data;
            m_CharacterSkin = new Skin("character-base");

            if (isCustomizable)
            {
                m_CharacterSkin.AddSkin(skeletonData.FindSkin(m_BaseSkinReference));
                m_CharacterSkin.AddSkin(skeletonData.FindSkin(m_LipsSkinReference));
                m_CharacterSkin.AddSkin(skeletonData.FindSkin(m_EyeLashesSkinReference));
                m_CharacterSkin.AddSkin(skeletonData.FindSkin(m_EyesSkinReference));
                m_CharacterSkin.AddSkin(skeletonData.FindSkin(m_HairSkinReference));
                m_CharacterSkin.AddSkin(skeletonData.FindSkin(m_LipsSkinReference));
                m_CharacterSkin.AddSkin(skeletonData.FindSkin(m_WingsSkinReference));
            }
            else
            {
               // m_CharacterSkin.AddSkin(skeletonData.FindSkin(fullSkin));
            }
           
        }

        void OptimizeSkin()
        {
            // Create a repacked skin.
            Skin previousSkin = m_SkeletonAnimation.Skeleton.Skin;

            Debug.Log(previousSkin == null);

            // Note: materials and textures returned by GetRepackedSkin() behave like 'new Texture2D()' and need to be destroyed
            if (m_RuntimeMaterial)
                Destroy(m_RuntimeMaterial);
            if (m_RuntimeAtlas)
                Destroy(m_RuntimeAtlas);
            Skin repackedSkin = previousSkin.GetRepackedSkin("Repacked skin",
                m_SkeletonAnimation.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial, out m_RuntimeMaterial,
                out m_RuntimeAtlas);
            previousSkin.Clear();

            // Use the repacked skin.
            m_SkeletonAnimation.Skeleton.Skin = repackedSkin;
            m_SkeletonAnimation.Skeleton.SetSlotsToSetupPose();
            m_SkeletonAnimation.AnimationState.Apply(m_SkeletonAnimation.Skeleton);

            // `GetRepackedSkin()` and each call to `GetRemappedClone()` with parameter `premultiplyAlpha` set to `true`
            // cache necessarily created Texture copies which can be cleared by calling AtlasUtilities.ClearCache().
            // You can optionally clear the textures cache after multiple repack operations.
            // Just be aware that while this cleanup frees up memory, it is also a costly operation
            // and will likely cause a spike in the framerate.
            AtlasUtilities.ClearCache();
            Resources.UnloadUnusedAssets();
        }


        public void SetupView(AppearanceData data)
        {
            m_BaseSkinReference = data.BaseSkin;
            m_EyeLashesSkinReference = data.EyesSkinReference;
            m_HairSkinReference = data.HairSkinReference;
            m_EyesSkinReference = data.EyesSkinReference;
            m_LipsSkinReference = data.LipsSkinReference;
            m_ClothesSkinReference = data.ClothesSkinReference;
            m_PantsSkinReference = data.PantsSkinReference;
            m_WingsSkinReference = data.WingsSkinReference;
            m_HatSkinReference = data.HatSkinReference;
            m_WeaponSkinReference = data.WeaponSkinReference;
            fullSkin = data.FullSkin;

            Init();
        }

        public void Equip(string itemSkin, ItemVisualType itemType)
        {
        }
    }
}