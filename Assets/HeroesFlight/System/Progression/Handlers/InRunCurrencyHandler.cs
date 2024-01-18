using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeroesFlight.System.Stats.Handlers
{
    public class InRunCurrencyHandler
    {
        public event Action<string, int> OnCurrencyUpdated; 
        private Dictionary<string, int> currencyCache = new();


        public int GetCurrencyAmount(string key)
        {
            if (currencyCache.TryGetValue(key, out var value))
            {
                return value;
            }

            return 0;
        }

        public void AddCurrency(string key, int amount,Action OnComplete=null)
        {
            if (currencyCache.TryGetValue(key, out var value))
            {
                currencyCache[key]+= amount;
            }
            else
            {
                currencyCache.Add(key, amount);
            }
            OnComplete?.Invoke();
        }

      

         public void ResetValues()
         {
             currencyCache.Clear();
         }

         public void ResetValue(string experience)
         {
             if (currencyCache.TryGetValue(experience, out var amount))
             {
                 amount = 0;
             }
         }
    }
}