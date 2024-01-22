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
        characterStatController.ModifyHealth(healPercentage, true);
        Debug.Log("Healed");
    }
}
