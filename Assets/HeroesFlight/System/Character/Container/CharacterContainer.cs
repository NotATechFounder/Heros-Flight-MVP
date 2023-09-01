using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Character.Enum;
using UnityEngine;

namespace HeroesFlight.System.Character.Container
{
    public class CharacterContainer : MonoBehaviour
    {
        [SerializeField] List<CharacterSimpleController> characterPrefabs = new();
        CharacterControllerInterface currentCharacter;

        public CharacterControllerInterface CreateCharacter(CharacterType targetCharacterType, Vector2 position)
        {
            CharacterSimpleController characterPrefab = null;
            foreach (var controller in characterPrefabs)
            {
                if (controller.CharacterSO.CharacterType == targetCharacterType)
                {
                    characterPrefab = controller;
                    break;
                }
            }

            if (characterPrefab == null)
            {
                Debug.LogError("Character prefab is not set");
                return null;
            }

            currentCharacter = Instantiate(characterPrefab, position, Quaternion.identity);
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

        public void ResetCharacter(Vector2 position)
        {
            currentCharacter.CharacterTransform.GetComponent<Rigidbody2D>().MovePosition(position);
        }
    }
}