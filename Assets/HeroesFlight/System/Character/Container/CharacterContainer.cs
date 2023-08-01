using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace HeroesFlight.System.Character.Container
{
    public class CharacterContainer : MonoBehaviour
    {
        [SerializeField] Vector2 spawnPoint;
        [SerializeField] CharacterSimpleController characterPrefab;
        CharacterControllerInterface currentCharacter;

        public CharacterControllerInterface CreateCharacter()
        {
            currentCharacter = Instantiate(characterPrefab, spawnPoint, Quaternion.identity);
            currentCharacter.Init();
            return currentCharacter;
        }

        public void SetCharacterControllerState(bool isEnabled)
        {
            currentCharacter.SetActionState(isEnabled);
        }

        public void Reset()
        {
            Destroy(currentCharacter.CharacterTransform.gameObject);
            currentCharacter = null;
        }
    }
}