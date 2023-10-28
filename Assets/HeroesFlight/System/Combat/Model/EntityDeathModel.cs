using HeroesFlight.System.Combat.Enum;
using UnityEngine;

namespace HeroesFlight.System.Combat.Model
{
    public class EntityDeathModel
    {
        public EntityDeathModel(Vector3 position,CombatEntityType entityType)
        {
            Position = position;
            EntityType = entityType;
        }
        public Vector3 Position { get; }
        public CombatEntityType EntityType { get; }
    }
}