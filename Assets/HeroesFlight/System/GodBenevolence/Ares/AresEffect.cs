using HeroesFlight.System.Character;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using Pelumi.Juicer;
using System;
using System.Collections;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using UnityEngine;

public class AresEffect : MonoBehaviour
{
    public Action OnHitEnemy;

    [SerializeField] private float autoAttackSpeed = 2f;
    [SerializeField] private float damage = 10f;

    [Header("Clash")]
    [SerializeField] private Transform swordHolder;
    [SerializeField] private Transform swordHandle;
    [SerializeField] private OverlapChecker overlapChecker;
    [SerializeField] private ParticleSystem clashEffect;

    [Header("Clash Foward Effect")]
    [SerializeField] private Vector3 rotateFowardDest = new Vector3(0, 0, 360);
    [SerializeField] private float fowardSpeed = 1f;
    [SerializeField] private Ease fowardEase = Ease.Linear;

    [Header("Clash Backward Effect")]
    [SerializeField] private Vector3 rotateBackwardDest = new Vector3(0, 0, 180);
    [SerializeField] private float backwardSpeed = 1f;
    [SerializeField] private Ease backwardEase = Ease.Linear;

    JuicerRuntime clashFowardEffectFoward;
    JuicerRuntime clashFowardEffectBackward;
    private CharacterControllerInterface characterController;
    private float timer;

    private void Start()
    {
        overlapChecker.OnDetect += OnOverlap;

        clashFowardEffectFoward = swordHandle.JuicyLocalRotate(rotateFowardDest, fowardSpeed);
        clashFowardEffectFoward.SetEase(fowardEase);
        clashFowardEffectFoward.SetOnStart(() =>
        {
            clashEffect.Play();
        });
        clashFowardEffectFoward.AddTimeEvent(0.5f, () =>
        {
            overlapChecker.Detect();
        });
        clashFowardEffectFoward.SetOnCompleted(() =>
        {
            clashFowardEffectBackward.Start();
        });

        clashFowardEffectBackward = swordHandle.JuicyLocalRotate(rotateBackwardDest, backwardSpeed);
        clashFowardEffectBackward.SetEase(backwardEase);
    }

    public IEnumerator AutoAttack()
    {
        while (true)
        {
            timer += Time.deltaTime;
            if (timer >= autoAttackSpeed)
            {
                timer = 0;

                if(overlapChecker.TargetInRange())
                clashFowardEffectFoward.Start();
            }
            yield return null;
        }
    }

    public void SetUp(float damage, CharacterControllerInterface characterControllerInterface, Action OnHitEvent=null)
    {       
        this.damage = damage;
        OnHitEnemy = OnHitEvent;
        this.characterController = characterControllerInterface;
        characterController.OnFaceDirectionChange += Flip;
        StartCoroutine(AutoAttack());
    }

    public void Flip(bool facingLeft)
    {
        swordHolder.localScale = new Vector3(facingLeft ? 1 : -1, 1, 1);
        overlapChecker.SetDirection(swordHolder.localScale.x == 1 ? OverlapChecker.Direction.Right : OverlapChecker.Direction.Left);
    }

    private void OnOverlap(int count, Collider2D[] colliders)
    {
        for (int i = 0; i < count; i++)
        {
            if (colliders[i].TryGetComponent( out IHealthController healthController))
            {
                healthController.TryDealDamage(new HealthModificationIntentModel(damage, 
                    DamageType.Critical, AttackType.Regular,DamageCalculationType.Flat));
              OnHitEnemy?.Invoke();
            }
        }
    }

    private void OnDisable()
    {
        OnHitEnemy = null;
        StopAllCoroutines();
        characterController.OnFaceDirectionChange -= Flip;
    }
}
