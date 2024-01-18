using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KageBunshin : RegularActiveAbility
{
    [SerializeField] private CustomAnimationCurve damagePercentageCurve;
    [SerializeField] private OverlapChecker overlapChecker;
    [SerializeField] Kage kagepPrefab;

    public const string Lvl_1 = "LV_1";
    public const string Lvl_2 = "LV_2";
    public const string Lvl_3 = "LV_3";

    private int baseDamage;
    private int currentDamage;

    private void Start()
    {
        Initialize(1, 10);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnActivated();
        }
    }

    public override void OnActivated()
    {
        currentDamage = (int)StatCalc.GetPercentage(baseDamage, damagePercentageCurve.GetCurrentValueFloat(currentLevel));
        overlapChecker.DetectOverlap();
    }

    public override void OnDeactivated()
    {

    }

    public override void OnCoolDownEnded()
    {

    }
     
    public override void LevelUp()
    {
        base.LevelUp();
    }


    public void Initialize(int level, int baseDamage)
    {
        this.currentLevel = level;
        this.baseDamage = baseDamage;
        overlapChecker.OnDetect += OnOverlap;
    }

    private void OnOverlap(int count, Collider2D[] colliders)
    {
        for (int i = 0; i < count; i++)
        {
            Kage kage = Instantiate(kagepPrefab, colliders[i].transform.position, Quaternion.identity);
            kage.transform.SetParent(colliders[i].transform);
            kage.Init(currentDamage, GetAnimatonID());
        }
    }

    public string GetAnimatonID()
    {
        int normalisedLevel = GetNormalisedLevel();
        switch (normalisedLevel)
        {
            case 0: return Lvl_2;
            case 1: return Lvl_3;
            case 2: return Lvl_3;
            default: break;
        }
        return Lvl_2;
    }

    private void OnDrawGizmosSelected()
    {
        if (damagePercentageCurve.curveType != CurveType.Custom)
        {
            damagePercentageCurve.UpdateCurve();
        }
    }
}
