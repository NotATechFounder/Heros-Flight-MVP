using HeroesFlight.Common.Enum;

namespace HeroesFlight.System
{
    public interface StateDependantSystemInterface : SystemInterface
    {
        GameStateType CurrentState { get; }
        void SetCurrentState(GameStateType newState);
    }
}