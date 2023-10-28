
using TMPro;
using UnityEngine;

namespace HeroesFlight.System.UI.Container
{
    public class UiContainer : MonoBehaviour
    {
        [SerializeField] TMP_SpriteAsset normalDamageAsset;
        [SerializeField] TMP_SpriteAsset criticalDamageAsset;

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
    }
}