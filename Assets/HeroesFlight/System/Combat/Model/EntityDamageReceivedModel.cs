using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;

namespace HeroesFlight.System.Combat.Model
{
    public class EntityDamageReceivedModel
    {
        public EntityDamageReceivedModel(Vector3 position,HealthModificationIntentModel damageIntent,
            CombatEntityType type,float healthPercentageLeft)
        {
            Position = position;
            DamageIntentModel = damageIntent;
            EntityType = type;
            HealthPercentagePercentageLeft = healthPercentageLeft;
        }
        public Vector3 Position { get; }
        public HealthModificationIntentModel DamageIntentModel { get; }
        public CombatEntityType EntityType { get; }
        public float HealthPercentagePercentageLeft { get; }
    }
}