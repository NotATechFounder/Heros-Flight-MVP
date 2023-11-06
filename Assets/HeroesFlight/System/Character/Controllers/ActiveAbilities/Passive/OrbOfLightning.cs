using HeroesFlight.System.Character;
using HeroesFlightProject.System.Combat.Controllers;
using HeroesFlightProject.System.Gameplay.Controllers;
using Plugins.Audio_System;
using UnityEngine;

public class OrbOfLightning : PassiveActiveAbility
{
    [SerializeField] private float damageMultiplier = 1;
    [SerializeField] private SkillOrb skillOrb;

    public void Initialize(int level, int baseDamage)
    {
        this.currentLevel = level;

        skillOrb.SetTarget(transform);
        if (skillOrb.TryGetComponent(out AutoShooter autoShooter))
        {
            autoShooter.SetDamage((int)(baseDamage * damageMultiplier));
        }
    }

    public override void OnActivated()
    {
        AudioManager.PlaySoundEffect("StormSkillActivation", SoundEffectCategory.Hero);
        skillOrb.Activate();
    }

    public override void OnCoolDownStarted()
    {
        skillOrb.Deactivate();
    }

    public override void OnCoolDownEnded()
    {

    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

    public override void LevelUpIncreaseEffect()
    {

    }
}
