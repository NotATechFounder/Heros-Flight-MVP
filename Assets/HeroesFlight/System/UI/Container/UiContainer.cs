using System;
using HeroesFlight.System.Gameplay.Enum;
using TMPro;
using UnityEngine;

namespace HeroesFlight.System.UI.Container
{
    public class UiContainer : MonoBehaviour
    {
        [SerializeField] TMP_SpriteAsset normalDamageAsset;
        [SerializeField] TMP_SpriteAsset criticalDamageAsset;

        public TMP_SpriteAsset GetDamageTextSprite(DamageType damageType)
        {
            switch (damageType)
            {
                case DamageType.NoneCritical:
                    return normalDamageAsset;
                    break;
                case DamageType.Critical:
                    return criticalDamageAsset;
                    break;
                default:
                    return null;
            }
        }
    }
}