using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace HeroesFlight.Common
{
    [Serializable]
    public class AnimationData
    {
        [SerializeField] AnimationReferenceAsset animation;
        [SerializeField] List<VfxEntry> vfxEntries = new();
        [SerializeField] List<SoundEntry> soundEntries = new();

        public AnimationReferenceAsset Aniamtion => animation;
        public List<VfxEntry> VfxEntries => vfxEntries;
        public List<SoundEntry> SoundEntries => soundEntries;
    }
}