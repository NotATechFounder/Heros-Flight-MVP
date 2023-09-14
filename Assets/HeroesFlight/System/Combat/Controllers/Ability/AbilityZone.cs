using System;
using UnityEngine.Serialization;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    [Serializable]
    public class AbilityZone
    {
        public WarningLine ZoneVisual;
        public OverlapChecker ZoneChecker;
    }
}