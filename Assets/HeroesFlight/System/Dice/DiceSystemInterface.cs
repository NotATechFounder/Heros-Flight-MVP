using System;

namespace HeroesFlight.System.Dice
{
    public interface DiceSystemInterface : SystemInterface
    {
        void  RollDice(Action<int> onComplete );
    }
}