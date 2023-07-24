using HeroesFlight.Common;
using UnityEngine;

namespace HeroesFlight.System.Character.Model
{
    [CreateAssetMenu(fileName = "CharacterModel", menuName = "Model/Character", order = 0)]
    public class CharacterModel : ScriptableObject
    {
        [SerializeField] CharacterData data;
        public CharacterData Data => data;
    }
}