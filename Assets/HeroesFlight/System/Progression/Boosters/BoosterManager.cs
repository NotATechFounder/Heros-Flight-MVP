using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterManager : MonoBehaviour
{
    [SerializeField] private CharacterStatController characterStatController;
    [SerializeField] private List<BoosterContainer> boosterContainerList;
    public event Action<BoosterSO, float, Transform> OnBoosterActivated;

    public event Action<BoosterContainer> OnBoosterContainerCreated;

    public void Initialize(CharacterStatController statController)
    {
        characterStatController = statController;
    }

    public bool ActivateBooster(BoosterSO boosterSO)
    {
        Boost boost = CreateBoost(boosterSO);

        if (IsInstantBoost(boosterSO))
        {
            OnBoosterActivated?.Invoke(boosterSO, boosterSO.BoosterValue, characterStatController.transform);
            boost.OnStart?.Invoke();
            return true;
        }

        if(IsBoosterIsActive(boosterSO, out bool used))
        {
            return used;
        }

        BoosterContainer boosterContainer = boosterContainerList.Find(x => !x.IsRunning);

        if (boosterContainer == null)
        {
            boosterContainer = new BoosterContainer();
            boosterContainerList.Add(boosterContainer);
        }

        boosterContainer.SetActiveBoost(this, boost);

        OnBoosterContainerCreated?.Invoke(boosterContainer);

        OnBoosterActivated?.Invoke(boosterSO, boosterSO.BoosterValue, characterStatController.transform);

        return true;
    }

    private bool IsInstantBoost(BoosterSO boosterSO)
    {
        return boosterSO.BoosterDuration == 0;
    }

    private bool IsBoosterIsActive(BoosterSO boosterSO, out bool used)
    {
        foreach (var boosterContainer in boosterContainerList)
        {
            if (boosterContainer.IsRunning && boosterContainer.ActiveBoost.boosterSO == boosterSO)
            {
                switch (boosterContainer.ActiveBoost.boosterSO.BoosterStackType)
                {
                    case BoosterStackType.None:
                        used = false;
                        return true;
                    case BoosterStackType.Duration:
                        OnBoosterActivated?.Invoke(boosterSO, boosterSO.BoosterValue, characterStatController.transform);
                        boosterContainer.ResetBoostDuration();
                        used = true;
                        return true;
                    case BoosterStackType.Effect:
                        OnBoosterActivated?.Invoke(boosterSO, boosterSO.BoosterValue, characterStatController.transform);
                        boosterContainer.IncreaseStackCount();
                        used = true;
                        return true;
                }
            }
        }
        used = false;
        return false;
    }

    private Boost CreateBoost(BoosterSO boosterSO)
    {
        switch (boosterSO.BoosterEffectType)
        {
            case BoosterEffectType.Health:
                return HealthBoost(boosterSO);
            case BoosterEffectType.Attack:
                return AttackBoost(boosterSO);
            case BoosterEffectType.Defense:
                return DefenseBoost(boosterSO);
            case BoosterEffectType.MoveSpeed:
                return MoveSpeedBoost(boosterSO);
            case BoosterEffectType.AttackSpeed:
                return AttackSpeedBoost(boosterSO);
            default: return null;
        }
    }

    private Boost DefenseBoost(BoosterSO boosterSO)
    {
        return new Boost(boosterSO, ()=>
        {
            characterStatController.GetStatModel.ModifyCurrentStat(StatType.Defense, boosterSO.BoosterValue, StatModel.StatModificationType.Addition, StatModel.StatCalculationType.Percentage);
        }, () =>
        {
            characterStatController.GetStatModel.ModifyCurrentStat(StatType.Defense, boosterSO.BoosterValue, StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);
        });
    }

    private Boost AttackSpeedBoost(BoosterSO boosterSO)
    {
        return new Boost(boosterSO, () =>
        {
            characterStatController.GetStatModel.ModifyCurrentStat(StatType.AttackSpeed, boosterSO.BoosterValue, StatModel.StatModificationType.Addition, StatModel.StatCalculationType.Percentage);
        }, () =>
        {
            characterStatController.GetStatModel.ModifyCurrentStat(StatType.AttackSpeed, boosterSO.BoosterValue, StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);
        });
    }

    private Boost MoveSpeedBoost(BoosterSO boosterSO)
    {
        return new Boost(boosterSO, () =>
        {
            characterStatController.GetStatModel.ModifyCurrentStat(StatType.MoveSpeed, boosterSO.BoosterValue, StatModel.StatModificationType.Addition, StatModel.StatCalculationType.Percentage);
        }, () =>
        {
            characterStatController.GetStatModel.ModifyCurrentStat(StatType.MoveSpeed, boosterSO.BoosterValue, StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);
        });
    }

    private Boost HealthBoost (BoosterSO boosterSO)
    {
        return new Boost(boosterSO, () =>
        {
            if(characterStatController.GetHealthPercentage() >= 40)
            {
                characterStatController.ModifyHealth(boosterSO.BoosterValue, true);
            }
            else
            {
                characterStatController.ModifyHealth(boosterSO.BoosterValue  + 5, true);
            }

        }, null);
    }

    private Boost AttackBoost(BoosterSO boosterSO)
    {
        return new Boost(boosterSO, () =>
        {
            characterStatController.GetStatModel.ModifyCurrentStat(StatType.PhysicalDamage, boosterSO.BoosterValue, StatModel.StatModificationType.Addition, StatModel.StatCalculationType.Percentage);
            characterStatController.GetStatModel.ModifyCurrentStat(StatType.MagicDamage, boosterSO.BoosterValue, StatModel.StatModificationType.Addition, StatModel.StatCalculationType.Percentage);
        }, () =>
        {
            characterStatController.GetStatModel.ModifyCurrentStat(StatType.PhysicalDamage, boosterSO.BoosterValue, StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);
            characterStatController.GetStatModel.ModifyCurrentStat(StatType.MagicDamage, boosterSO.BoosterValue, StatModel.StatModificationType.Subtraction, StatModel.StatCalculationType.Percentage);
        });
    }
}
