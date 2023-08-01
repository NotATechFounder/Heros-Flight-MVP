using HeroesFlight.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AiAgentCombatModel", menuName = "Model/Combat/AiAgentCombat", order = 0)]
public class AiAgentCombatModel : CombatModel
{
    [SerializeField] float agroDistance;
    [SerializeField] int damage = 2;

    public int Damage => damage;
    public float AgroDistance => agroDistance;
}
