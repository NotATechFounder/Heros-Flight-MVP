using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace HeroesFlight.System.Dice
{
    public class DiceSystem : DiceSystemInterface
    {
        public DiceSystem()
        {
            totalWeight = PoorThreshHold + AverageThreshHold + GreatThreshHold + PoorThreshHold+PowerFulThreshHold;
            rollCache.Add(RollType.Poor,PoorThreshHold);
            rollCache.Add(RollType.Average,AverageThreshHold);
            rollCache.Add(RollType.Great,GreatThreshHold);
            rollCache.Add(RollType.PowerFul,PowerFulThreshHold);
        }
        private const int PoorThreshHold = 15;
        private const int AverageThreshHold = 60;
        private const int GreatThreshHold = 20;
        private const int PowerFulThreshHold = 5;
        private Dictionary<RollType, int> rollCache = new();
        private int totalWeight;
        public void Init(Scene scene = default, Action onComplete = null) {}

        public void Reset() { }

        public void RollDice(int min, int max, Action<int> onComplete )
        {
            var diceRoll = Random.Range(0, totalWeight);
            var currentRollType = RollType.Poor;
            foreach (var entry in rollCache)
            {
                if (entry.Value >= diceRoll)
                {
                    currentRollType=entry.Key;
                    break;
                }

                diceRoll -= entry.Value;
            }

            int resultRoll = 0;

            switch (currentRollType)
            {
                case RollType.Poor:
                    resultRoll = Random.Range(1, 4);
                    break;
                case RollType.Average:
                    resultRoll = Random.Range(4, 8);
                    break;
                case RollType.Great:
                    resultRoll = Random.Range(8, 11);
                    break;
                case RollType.PowerFul:
                    resultRoll = Random.Range(11, 13);
                    break;
            }
            
            
            onComplete?.Invoke(resultRoll);
           
        }


        Color ConvertDiceRollToColor(RollType type)
        {
            switch (type)
            {
                case RollType.Poor:
                    return Color.red;
                case RollType.Average:
                    return Color.white;
                case RollType.Great:
                    return Color.green;
                case RollType.PowerFul:
                    return Color.yellow;
               default:
                   return Color.white;
            }
        }
    }
}