using HeroesFlight.Common.Enum;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private CharacterSO[] characters;

    public CharacterSO SelectedCharacter { get; private set; }

    private void Awake()                
    {
        LoadAllCharacterData();
    }

    public CharacterSO GetCharacterSO(CharacterType characterType)
    {
        foreach (CharacterSO characterSO in characters)
        {
            if (characterSO.CharacterType == characterType)
            {
                return characterSO;
            }
        }
        return null;
    }

    public void LoadAllCharacterData()
    {
        foreach (CharacterSO characterSO in characters)
        {
            characterSO.Load();
        }

        SelectedCharacter = GetSelectedCharacter();
    }

    public void SaveAllCharacterData()
    {
        foreach (CharacterSO characterSO in characters)
        {
            characterSO.Save();
        }
    }

    public void SetSelectedCharacter(CharacterSO characterSO)
    {
        SelectedCharacter = characterSO;
    }

    public void UnlockCharacter(CharacterType characterType)
    {
        CharacterSO characterSO = GetCharacterSO(characterType);
        characterSO.Unlock();
    }

    public void ToggleCharacterSelected(CharacterType characterType, bool selected)
    {
        CharacterSO characterSO = GetCharacterSO(characterType);
        characterSO.ToggleSelected(selected);
        if (selected)
        {
            SetSelectedCharacter(characterSO);
        }
    }

    public void ToggleCharacterSelected(CharacterSO characterSO, bool selected)
    {
        characterSO.ToggleSelected(selected);
        if (selected)
        {
            SetSelectedCharacter(characterSO);
        }
    }

    public CharacterSO GetSelectedCharacter()
    {
        foreach (CharacterSO characterSO in characters)
        {
            if (characterSO.CharacterData.isSelected)
            {
                return characterSO;
            }
        }
        return null;
    }
}
