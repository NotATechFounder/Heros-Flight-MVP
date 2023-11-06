using HeroesFlightProject.System.Combat.Controllers;
using Plugins.Audio_System;
using UnityEngine;

public class OrbOfLightning : PassiveActiveAbility
{
    [SerializeField] private int damage;
    [SerializeField] private SkillOrb skillOrbPrefab;

    private SkillOrb skillOrb;
    private CharacterStatController characterStatController;

    private void Start()
    {
        characterStatController = GetComponent<CharacterStatController>();
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
                autoShooter.SetDamage((int)characterStatController.CurrentMagicDamage);
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
