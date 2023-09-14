using System;

namespace HeroesFlightProject.System.NPC.Enum
{
    [Serializable]
    public enum BossState
    {
        Idle,
        UsingAbility,
        Damaged,
        Dead
    }
}