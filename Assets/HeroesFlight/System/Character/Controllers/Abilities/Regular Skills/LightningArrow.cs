using HeroesFlight.System.Character;
using HeroesFlightProject.System.Gameplay.Controllers;
using Pelumi.ObjectPool;
using Plugins.Audio_System;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningArrow : RegularActiveAbility
{
    [Header("LineDamage")]
    [SerializeField] private float firstAttackDelay = 0.1f;
    [SerializeField] private float lineDamageDelay = 0.25f;
    [SerializeField] private int linesOfDamage = 2;
    [SerializeField] private int linesOfDamagePerIncrease = 1;

    [Header("Projectile")]
    [SerializeField] private int numberOfProjectile = 10;
    [SerializeField] private int spacing = 2;

    [Header("Damage")]
    [SerializeField] private CustomAnimationCurve damagePercentageCurve;
    [SerializeField] private ProjectileControllerBase projectileController;
    [SerializeField] Transform projectileSpawnPoint;

    [Header("Animation and Viusal Settings")]
    [SerializeField] private Transform visual;
    [SerializeField] SkeletonAnimation skeletonAnimation;
    [SerializeField] public const string attackAnimation1Name = "animation";

    private int baseDamage;
    private float currentDamagePercentage;
    private int currentDamage;
    private int currentlinesOfDamage;
    private CharacterSimpleController characterControllerInterface;

    private ProjectileControllerBase currentProjectile;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnActivated();
        }
    }

    public override void OnActivated()
    {
        skeletonAnimation.gameObject.SetActive(true);
        GetEffectParticleByLevel().Play();

        currentDamagePercentage = damagePercentageCurve.GetCurrentValueFloat(currentLevel);
        currentDamage = (int)StatCalc.GetPercentage(baseDamage, currentDamagePercentage);

        currentlinesOfDamage = GetMajorValueByLevel(linesOfDamage, linesOfDamagePerIncrease);

        skeletonAnimation.AnimationState. SetAnimation(0, attackAnimation1Name, false);
    }

    public override void OnDeactivated()
    {

    }

    public override void OnCoolDownEnded()
    {

    }

    private void AnimationState_Event(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == "Attack")
        {
            FireUpProjectile();
        }
    }

    private void AnimationState_Complete(TrackEntry trackEntry)
    {
        switch (trackEntry.Animation.Name)
        {
            case attackAnimation1Name:
                // play no animation
                skeletonAnimation.AnimationState.SetEmptyAnimation(0, 0);
                skeletonAnimation.gameObject.SetActive(false);
                break;
            default: break;
        }
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
        skeletonAnimation.AnimationState.Complete += AnimationState_Complete;
        skeletonAnimation.AnimationState.Event += AnimationState_Event;
        skeletonAnimation.gameObject.SetActive(false);
    }

    private void Flip(bool facingLeft)
    {
        transform.localScale = new Vector3(facingLeft ? 1 : -1, 1, 1);
    }

    private void FireUpProjectile()
    {
        currentProjectile = Instantiate(projectileController, projectileSpawnPoint.position, Quaternion.identity);
        currentProjectile.SetupProjectile(currentDamage,Vector2.up);
        currentProjectile.OnHit += HandleOnHit;
        currentProjectile.OnDeactivate += HandleArrowDisable;

        StartCoroutine(FireRainProjectile());
    }

    private IEnumerator FireRainProjectile()
    {
        yield return new WaitForSeconds(1.5f);
        Transform centre = currentProjectile.transform;
        for (int i = 0; i < numberOfProjectile; i++)
        {
            Vector2 spawnPos = i % 2 == 0 ? centre.position + Vector3.left * spacing * (i / 2) : centre.position + Vector3.right * spacing * (i / 2);
            ProjectileControllerBase  Projectile = Instantiate(projectileController, spawnPos, Quaternion.identity);
            Projectile.SetupProjectile(currentDamage, Vector2.down);
            Projectile.OnHit += HandleOnHit;
            Projectile.OnDeactivate += HandleArrowDisable;
            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        }
    }

    void HandleOnHit(ProjectileControllerInterface obj)
    {

    }

    void HandleArrowDisable(ProjectileControllerInterface obj)
    {
        AudioManager.PlaySoundEffect("LightningExplosion", SoundEffectCategory.Hero);
        obj.OnHit -= HandleOnHit;
        obj.OnDeactivate -= HandleArrowDisable;
        var arrow = obj as ProjectileControllerBase;
        Destroy(arrow.gameObject);
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
