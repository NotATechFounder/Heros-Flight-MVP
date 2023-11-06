using HeroesFlight.System.Character;
using HeroesFlightProject.System.Combat.Controllers;
using HeroesFlightProject.System.Gameplay.Controllers;
using Plugins.Audio_System;
using UnityEngine;

public class OrbOfLightning : PassiveActiveAbility
{
    [SerializeField] private float damageMultiplier = 1;
    [SerializeField] private SkillOrb skillOrbPrefab;

    private SkillOrb skillOrb;
    private CharacterStatController characterStatController;

    public void Initialize(int level, CharacterStatController characterStatController)
    {
        this.level = level;
        this.characterStatController = characterStatController;
    }

    public override void OnActivated()
    {
        AudioManager.PlaySoundEffect("StormSkillActivation", SoundEffectCategory.Hero);

        if (skillOrb == null)
        {
            skillOrb = Instantiate(skillOrbPrefab, transform.position, Quaternion.identity);
            skillOrb.SetTarget(transform);
            if (skillOrb.TryGetComponent(out AutoShooter autoShooter))
            {
                autoShooter.SetDamage((int)(characterStatController.CurrentMagicDamage * damageMultiplier));
            }
        }

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
}
