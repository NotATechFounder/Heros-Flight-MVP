using System;

namespace HeroesFlight.System.Dice
{
    public interface DiceSystemInterface : SystemInterface
    {
        void  RollDice(int min, int max, Action<int> onComplete );
    }
}