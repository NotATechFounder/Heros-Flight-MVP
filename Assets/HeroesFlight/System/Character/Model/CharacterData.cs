using System;
using HeroesFlight.Common;
using UnityEngine;

namespace HeroesFlight.System.Character.Model
{
    [Serializable]
    public class CharacterData
    {
        [SerializeField] AppearanceModel appearenceModel;
        [SerializeField] PlayerCombatModel combatModel;

        public PlayerCombatModel CombatModel => combatModel;
        public AppearanceModel AppearenceModel => appearenceModel;
    }
}