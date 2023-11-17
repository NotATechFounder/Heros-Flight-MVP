using System;

namespace HeroesFlight.System.Stats
{
    public interface ProgressionSystemInterface : SystemInterface
    {
        public BoosterManager BoosterManager { get; }
        void AddCurrency(string key, int amount, Action OnComplete = null);
        int GetCurrency(string key);
        void SaveRunResults();
        void ResetCurrency(string experience);
        void CollectRunCurrency();
        
    }
}