using HeroesFlight.System.Character;
using HeroesFlightProject.System.Combat.Controllers;
using HeroesFlightProject.System.Gameplay.Controllers;
using Plugins.Audio_System;
using UnityEngine;

public class OrbOfLightning : RegularActiveAbility
{
    [SerializeField] private CustomAnimationCurve damagePercentageCurve;
    [SerializeField] private float currentDamagePercentage = 50;
    [SerializeField] private SkillOrb skillOrb;
    private float baseDamage;
    AutoShooter autoShooter;

    public void Initialize(int level, CharacterStatController characterStatController)
    {
        this.currentLevel = level;
        skillOrb.SetTarget(characterStatController.transform);
        if (skillOrb.TryGetComponent(out AutoShooter autoShooter))
        {
            this.autoShooter = autoShooter;
        }
        baseDamage = characterStatController.CurrentMagicDamage;
    }

    public override void OnActivated()
    {
        GetEffectParticleByLevel().gameObject.SetActive(true);

        currentDamagePercentage = damagePercentageCurve.GetCurrentValueFloat(currentLevel);
        int currentDamage = (int)StatCalc.GetPercentage(baseDamage, currentDamagePercentage);
        autoShooter.SetDamage(currentDamage);

        AudioManager.PlaySoundEffect("StormSkillActivation", SoundEffectCategory.Hero);
        skillOrb.Activate();
    }

    public override void OnDeactivated()
    {
        skillOrb.Deactivate();
        GetEffectParticleByLevel().gameObject.SetActive(false);
    }

    public override void OnCoolDownEnded()
    {

    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

    private void OnDrawGizmosSelected()
    {
        if (damagePercentageCurve.curveType != CurveType.Custom)
        {
            damagePercentageCurve.UpdateCurve();
        }
    }
}
