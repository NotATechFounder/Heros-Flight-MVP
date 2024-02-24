namespace HeroesFlight.System.Cheats
{
    public interface CheatSystemInterface : SystemInterface
    {
        void AddCurrency();
        void AddItems();
        void UnlockTraits();
        void KillAllEnemies();
        void MakePlayerImmortal(bool isImmortal);
    }
}