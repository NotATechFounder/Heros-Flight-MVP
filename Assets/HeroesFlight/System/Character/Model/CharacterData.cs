using System;
using HeroesFlight.Common;
using UnityEngine;

namespace HeroesFlight.System.Character.Model
{
    [Serializable]
    public class CharacterData
    {
        [SerializeField] AppearanceModel appearenceModel;
        [SerializeField] CombatModel combatModel;

        public CombatModel CombatModel => combatModel;
        public AppearanceModel AppearenceModel => appearenceModel;
    }
}