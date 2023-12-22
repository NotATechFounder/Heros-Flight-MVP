using TMPro;
using UnityEngine;


namespace HeroesFlight.System.UI.Container
{
    public class UiContainer : MonoBehaviour
    {
        [SerializeField] TMP_SpriteAsset normalDamageAsset;
        [SerializeField] TMP_SpriteAsset criticalDamageAsset;
        [SerializeField] TMP_SpriteAsset healTextAsset;

        //TODO: remove it after gettingactual heal text asset
        private void Awake()
        {
            healTextAsset.material.color = Color.green;
            ;
        }

        public TMP_SpriteAsset GetDamageTextSprite(bool isCritical)
        {
            switch (isCritical)
            {
                case false:
                    return normalDamageAsset;
                case true:
                    return criticalDamageAsset;
            }
        }

        public TMP_SpriteAsset GetDamageTextSprite(bool isCritical, bool isHeal)
        {
            if (isHeal)
            {
                return healTextAsset;
            }

            return GetDamageTextSprite(isCritical);
        }
    }
}