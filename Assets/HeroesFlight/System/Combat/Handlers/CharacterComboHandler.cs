using System;
using System.Collections;
using StansAssets.Foundation.Async;
using UnityEngine;

namespace HeroesFlight.System.Combat.Handlers
{
    public class CharacterComboHandler
    {
        public event Action<int> OnComboUpdated;
        float timeSinceLastStrike;
        float timeToResetCombo = 3f;
        int characterComboNumber;

        Coroutine ComboRoutine;

        IEnumerator CheckTimeSinceLastStrike()
        {
            while (true)
            {
                timeSinceLastStrike -= Time.deltaTime;
                if (timeSinceLastStrike <= 0)
                {
                    if (characterComboNumber != 0)
                    {
                        characterComboNumber = 0;
                        OnComboUpdated?.Invoke(characterComboNumber);
                    }
                }

                yield return null;
            }
        }

        public void RegisterCharacterHit()
        {
            timeSinceLastStrike = timeToResetCombo;
            characterComboNumber++;
            OnComboUpdated?.Invoke(characterComboNumber);
        }

        public void StartComboCheck()
        {
            ComboRoutine = CoroutineUtility.Start(CheckTimeSinceLastStrike());
        }

        public void ResetComboCheck()
        {
            if (ComboRoutine != null)
                CoroutineUtility.Stop(ComboRoutine);
            characterComboNumber = 0;
            OnComboUpdated?.Invoke(characterComboNumber);
        }
    }
}