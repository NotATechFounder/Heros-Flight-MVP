using System;

namespace HeroesFlight.System.Dice
{
    public interface DiceSystemInterface : SystemInterface
    {
        void InjectUiConnection();
        void  RollDice(Action<int> onComplete );
    }
}