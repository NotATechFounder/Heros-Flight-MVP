using System;

namespace HeroesFlight.System.Dice
{
    public interface DiceSystemInterface : SystemInterface
    {
        int DiceCost { get; }
        void InjectUiConnection();
        void  RollDice(Action<int> onComplete );
    }
}