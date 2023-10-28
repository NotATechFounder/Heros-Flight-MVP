using System;
using UnityEngine;

namespace HeroesFlight.Common
{
    [Serializable]
    public class SoundEntry : EntryBase
    {
        [SerializeField] AudioClip audio;
        public AudioClip Audio => audio;
    }
}