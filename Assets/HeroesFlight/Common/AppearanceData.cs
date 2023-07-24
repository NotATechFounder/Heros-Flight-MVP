using System;
using UnityEngine;

namespace HeroesFlight.Common
{
    [Serializable]
    public class AppearanceData
    {
        [SerializeField] string m_BaseSkinReference = "APPEARANCE/skin_color/skin_2";
        [SerializeField] string m_EyeLashesSkinReference = "APPEARANCE/eyebrows/eyebrows_004";
        [SerializeField] string m_HairSkinReference = "APPEARANCE/hair/hair_012";
        [SerializeField] string m_EyesSkinReference = "APPEARANCE/eyes/black/eyes_black_5";
        [SerializeField] string m_LipsSkinReference = "APPEARANCE/lips/lips_001";
        [SerializeField] string m_ClothesSkinReference = "OUTFIT/topwear/topwear_001_common";
        [SerializeField] string m_PantsSkinReference = "OUTFIT/bottomwear/bottomwear_001_common";
        [SerializeField] string m_WingsSkinReference = "OUTFIT/back_wings/back_wings_004_rare";
        [SerializeField] string m_HatSkinReference = "OUTFIT/hat/hat_000_null";
        [SerializeField] string m_WeaponSkinReference = "";

        public string BaseSkin => m_BaseSkinReference;
        public string EyeLashesSkinReference => m_EyeLashesSkinReference;
        public string HairSkinReference => m_HairSkinReference;
        public string EyesSkinReference => m_EyesSkinReference;
        public string LipsSkinReference => m_LipsSkinReference;
        public string ClothesSkinReference => m_ClothesSkinReference;
        public string PantsSkinReference => m_PantsSkinReference;
        public string WingsSkinReference => m_WingsSkinReference;
        public string HatSkinReference => m_HatSkinReference;
        public string WeaponSkinReference => m_WeaponSkinReference;
    }
}