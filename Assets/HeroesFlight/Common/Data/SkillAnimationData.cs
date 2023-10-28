using System;
using UnityEngine;

namespace HeroesFlight.Common
{
    [Serializable]
    public class SkillAnimationData : AnimationData
    {
        [SerializeField] Transform targetTransform;

        public Transform TargetTransform => targetTransform;
    }
}