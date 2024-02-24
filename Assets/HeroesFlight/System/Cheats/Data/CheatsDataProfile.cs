using UnityEngine;

namespace HeroesFlight.System.Cheats.Data
{
    [CreateAssetMenu(fileName = "CheatsDataProfile", menuName = "Cheats/CheatsDataProfile")]
    public class CheatsDataProfile : ScriptableObject
    {
        [SerializeField] private bool enableCheats;

        public bool EnableCheats => enableCheats;
    }
}