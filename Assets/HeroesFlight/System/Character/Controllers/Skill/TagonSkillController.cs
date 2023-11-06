using HeroesFlightProject.System.Gameplay.Controllers;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.Common.Animation;
using HeroesFlight.System.Character.Controllers.Skill;
using HeroesFlightProject.System.Combat.Controllers;
using UnityEngine;
using UnityEngine.UI;
using Plugins.Audio_System;

public class TagonSkillController : SkillAnimationEventHandler, IActiveAbilityInterface
{
    [Header("Skill One")]
    [SerializeField] private TimedAbility skillOne;
    [SerializeField] private SkillOrb skillOrbPrefab;
    [SerializeField] private float lifeStealPercentage = 0.1f;

    public TimedAbility PassiveAbilityOne => skillOne;

    private SkillOrb skillOrb;
    private CharacterStatController characterStatController;
    IAttackControllerInterface attackControllerInterface;

    protected override void Awake()
    {
        characterStatController = GetComponent<CharacterStatController>();
        attackControllerInterface = GetComponent<IAttackControllerInterface>();

        skillOne.OnActivated = OnActivateSkillOne;
        skillOne.OnDeactivated = OnDeactivateSkillOne;
        base.Awake();
    }

    private void Start()
    {
        skillOne.Init(this);
    }

    public void OnActivateSkillOne()
    {
        AudioManager.PlaySoundEffect("TagonSkillActivation", SoundEffectCategory.Hero);

        if (skillOrb == null)
        {
            skillOrb = Instantiate(skillOrbPrefab, transform.position, Quaternion.identity);
            skillOrb.SetTarget(transform);
        }

        skillOrb.Activate();
        attackControllerInterface.OnHitTarget += AttackControllerInterface_OnHitTarget;
    }

    public void OnDeactivateSkillOne()
    {
        skillOrb.Deactivate();
        attackControllerInterface.OnHitTarget -= AttackControllerInterface_OnHitTarget;
    }

    private void AttackControllerInterface_OnHitTarget()
    {
        ApplyLifeSteal(lifeStealPercentage);
    }

    protected void ApplyLifeSteal(float percentage)
    {
        float healthInc = StatCalc.GetPercentage(characterStatController.CurrentMaxHealth, percentage);
        characterStatController.ModifyHealth(healthInc, true);
    }

    protected override void HandleAnimationEvent(AnimationEventInterface animationEvent)
    {
        if (!skillOne.IsActive)
            return;
        
        base.HandleAnimationEvent(animationEvent);
    }
}
