using HeroesFlight.Common.Enum;
using HeroesFlight.System.Character;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using Pelumi.ObjectPool;
using Plugins.Audio_System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavenStab : RegularActiveAbility
{
    [Header("LineDamage")]
    [SerializeField] private float firstAttackDelay = 0.1f;
    [SerializeField] private float lineDamageDelay = 0.25f;
    [SerializeField] private int linesOfDamage = 2;
    [SerializeField] private int linesOfDamagePerIncrease = 1;

    [Header("Damage")]
    [SerializeField] private CustomAnimationCurve damagePercentageCurve;
    [SerializeField] private ProjectileControllerBase projectileController;
    [SerializeField] private Transform visual;

    private int baseDamage;
    private float currentDamagePercentage;
    private int currentDamage;
    private int currentlinesOfDamage;
    private CharacterSimpleController characterControllerInterface;

    public override void OnActivated()
    {
        GetEffectParticleByLevel().SetActive(true);

        currentDamagePercentage = damagePercentageCurve.GetCurrentValueFloat(currentLevel);
        currentDamage = (int)StatCalc.GetPercentage(baseDamage, currentDamagePercentage);

        currentlinesOfDamage = GetMajorValueByLevel(linesOfDamage, linesOfDamagePerIncrease);

        FireProjectile();
    }

    public override void OnDeactivated()
    {

    }

    public override void OnCoolDownEnded()
    {
        GetEffectParticleByLevel().SetActive(false);
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

    public void Initialize(int level, int baseDamage, CharacterSimpleController characterControllerInterface)
    {
        this.currentLevel = level;
        this.baseDamage = baseDamage;
        this.characterControllerInterface = characterControllerInterface;
        characterControllerInterface.OnFaceDirectionChange += Flip;
    }

    private void Flip(bool facingLeft)
    {
        visual.localScale = new Vector3(facingLeft ? 1 : -1, 1, 1);
    }

    private void FireProjectile()
    {
        ProjectileControllerBase bullet = ObjectPoolManager.SpawnObject(projectileController, transform.position, Quaternion.identity);
        bullet.transform.localScale = new Vector3(visual.localScale.x * bullet.transform.localScale.y, bullet.transform.localScale.y, 1);

        bullet.SetupProjectile(currentDamage, -visual.localScale.x * Vector2.right);
        bullet.SetLine(currentlinesOfDamage, 0.25f);
        bullet.OnHit += HandleOnHit;
        bullet.OnDeactivate += HandleArrowDisable;
    }

    void HandleOnHit(ProjectileControllerInterface obj)
    {

    }

    void HandleArrowDisable(ProjectileControllerInterface obj)
    {
        AudioManager.PlaySoundEffect("LightningExplosion", SoundEffectCategory.Hero);
        obj.OnHit -= HandleArrowDisable;
        obj.OnDeactivate -= HandleArrowDisable;
        var arrow = obj as ProjectileControllerBase;
        ObjectPoolManager.ReleaseObject(arrow.gameObject);
    }

    private void OnDestroy()
    {
        characterControllerInterface.OnFaceDirectionChange -= Flip;
    }

    private void OnDrawGizmosSelected()
    {
        if (damagePercentageCurve.curveType != CurveType.Custom)
        {
            damagePercentageCurve.UpdateCurve();
        }
    }
}
