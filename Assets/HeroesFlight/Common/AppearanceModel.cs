using UnityEngine;

namespace HeroesFlight.Common
{
    [CreateAssetMenu(fileName = "AppearanceModel", menuName = "Model/Appearance", order = 0)]
    public class AppearanceModel : ScriptableObject
    {
        [SerializeField] AppearanceData data;
        public AppearanceData Data => data;
    }
}