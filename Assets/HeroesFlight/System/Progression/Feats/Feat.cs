using UnityEngine;

namespace HeroesFlight.System.Stats.Feats
{
    public class Feat : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private Sprite image;
        [SerializeField] private string description;
        [SerializeField] private FeatEffect effect;
        [SerializeField] private string blockingFeatId;
    }
}