using HeroesFlight.System.Character;
using UnityEngine;

public class HermesEffect : MonoBehaviour
{
    [SerializeField] private Transform visual;  

    private CharacterControllerInterface characterController;
    public void SetUp(CharacterControllerInterface characterControllerInterface)
    {
        characterController = characterControllerInterface;
        characterController.OnFaceDirectionChange += Flip;
    }

    public void Flip(bool facingLeft)
    {
        visual.localScale = new Vector3(facingLeft ? 1 : -1, 1, 1);
    }

    private void OnDestroy()
    {
        characterController.OnFaceDirectionChange -= Flip;
    }
}
