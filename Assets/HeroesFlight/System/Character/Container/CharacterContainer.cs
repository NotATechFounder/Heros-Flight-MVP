using System.Collections.Generic;
using HeroesFlight.System.Character.Enum;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace HeroesFlight.System.Character.Container
{
    public class CharacterContainer : MonoBehaviour
    {
        [SerializeField] Vector2 spawnPoint;
        [SerializeField] List<CharacterSimpleController> characterPrefabs = new();
        CharacterControllerInterface currentCharacter;

        public CharacterControllerInterface CreateCharacter(CharacterType targetCharacterType)
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

        public void ResetCharacter()
        {
            currentCharacter.CharacterTransform.GetComponent<Rigidbody2D>().MovePosition(spawnPoint);
        }
    }
}