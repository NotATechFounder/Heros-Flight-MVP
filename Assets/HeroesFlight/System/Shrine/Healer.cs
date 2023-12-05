using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    [SerializeField] private float healPercentage = 10f;
    private CharacterStatController characterStatController;

    public void Initialize(CharacterStatController characterStatController)
    {
        this.characterStatController = characterStatController;
    }

    public void Heal()
    {
        float healthToHeal = StatCalc.GetPercentage (characterStatController.GetStatModel.GetCurrentStatValue(StatType.MaxHealth), healPercentage);
        characterStatController.ModifyHealth(healthToHeal, true);
        Debug.Log("Healed");
    }
}
