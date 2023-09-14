using System;
using Spine.Unity;
using UnityEngine;

namespace HeroesFlight.System.UI.Data
{
    [Serializable]
    public class CharacterUiViewData
    {
        [SerializeField] CharacterSO characterData;
        [SerializeField] SkeletonDataAsset dataAsset;

        public CharacterSO CharacterData => characterData;
        public SkeletonDataAsset Skeleton => dataAsset;
    }
}