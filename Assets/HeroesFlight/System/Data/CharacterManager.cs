using HeroesFlight.Common.Enum;
using System;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public event Action<CharacterSO> OnCharacterChanged;

    [SerializeField] private CharacterSO[] characters;

    private CurrencyManager currencyManager;

    public CharacterSO SelectedCharacter { get; private set; }

    public void Init(CurrencyManager currencyManager)
    {
        this.currencyManager = currencyManager;
        LoadAllCharacterData();
    }

    public bool TryBuyCharacter(CharacterType characterType)
    {
        CharacterSO characterSO = GetCharacterSO(characterType);
        if (currencyManager.GetCurrencyAmount (characterSO.CurrencySO.GetID()) >= characterSO.UnlockPrice)
        {
            currencyManager.ReduceCurency(characterSO.CurrencySO.GetID(), characterSO.UnlockPrice);
            UnlockCharacter(characterType);
            return true;
        }
        return false;
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

            if (characterSO.CharacterData.isSelected)
            {
                SetSelectedCharacter(characterSO);
            }
        }
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
        OnCharacterChanged?.Invoke(characterSO);
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

    public CharacterSO[] GetAllCharacterSO()
    {
        return characters;
    }
}
