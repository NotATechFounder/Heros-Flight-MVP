
using System.Collections.Generic;
using HeroesFlight.Common;
using HeroesFlight.Common.Animation;
using UnityEngine;

namespace HeroesFlight.System.Character.Controllers.Skill
{
    public class SkillAnimationEventHandler : MonoBehaviour
    {
        [Header("Skill effects related to animations")]
        [SerializeField] List<SkillAnimationData> aniamtionData;
        CharacterAnimationControllerInterface animator;
        SkillAnimationDataHandle aniamtionsHandler;

        protected virtual void Awake()
        {
            aniamtionsHandler = new SkillAnimationDataHandle(aniamtionData);
            animator = GetComponent<CharacterAnimationController>();
            animator.OnAnimationEvent += HandleAnimationEvent;
        }

        protected virtual void HandleAnimationEvent(AnimationEventInterface animationEvent)
        {
            aniamtionsHandler.HandleAnimationEvent(animationEvent);
        }
    }
}