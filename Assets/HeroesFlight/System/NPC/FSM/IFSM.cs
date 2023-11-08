using System;

namespace HeroesFlightProject.System.NPC.State
{
    public interface IFSM
    {
        void SetState(Type newState);
    }
}
