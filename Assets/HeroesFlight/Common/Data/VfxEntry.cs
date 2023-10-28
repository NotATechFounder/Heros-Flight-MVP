using System;
using UnityEngine;

namespace HeroesFlight.Common
{
    [Serializable]
    public class VfxEntry : EntryBase
    {
        [SerializeField] ParticleSystem particleSystem;
        public ParticleSystem Particle => particleSystem;
    }
}