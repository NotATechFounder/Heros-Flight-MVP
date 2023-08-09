
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pelumi.Juicer;
using Pelumi.ObjectPool;
using TMPro;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance { get; private set; }

    [SerializeField] private TextPopUp popUpPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PopUpTextAtTransfrom(Transform spawnPosition, Vector3 randomIntensity, string text, Color color, bool parent = false)
    {
        TextPopUp textPopUp = ObjectPoolManager.SpawnObject(popUpPrefab);
        if (parent) textPopUp.transform.SetParent(spawnPosition);
        SetPopUpInfo(textPopUp, spawnPosition.position, randomIntensity, text, color);
    }

    public void PopUpAtTextPosition(Vector3 spawnPosition, Vector3 randomIntensity, string text, Color color, float txtSize = 60)
    {
        TextPopUp textPopUp = ObjectPoolManager.SpawnObject(popUpPrefab);
        SetPopUpInfo(textPopUp, spawnPosition, randomIntensity, text, color, size: txtSize);
    }

    public void SetPopUpInfo(TextPopUp textPopUp, Vector3 spawnPosition, Vector3 randomIntensity, string text, Color color, float size = 60)
    {
        Vector2 finalPos = spawnPosition += new Vector3
            (
            Random.Range(-randomIntensity.x, randomIntensity.x),
                Random.Range(-randomIntensity.y, randomIntensity.y),
                Random.Range(-randomIntensity.z, randomIntensity.z)
            );
        textPopUp.Init(text, color, finalPos, size);
    }

    public void PopUpTextAtTransfrom(Transform damageModelTarget, Vector3 randomIntensity, string damageText, TMP_SpriteAsset spriteAsset,float size, bool parent = false)
    {
        TextPopUp textPopUp = ObjectPoolManager.SpawnObject(popUpPrefab);
        if (parent) textPopUp.transform.SetParent(damageModelTarget);
        SetPopUpInfo(textPopUp, damageModelTarget.position, randomIntensity, damageText, spriteAsset,size);
    }

    void SetPopUpInfo(TextPopUp textPopUp, Vector3 spawnPosition, Vector3 randomIntensity, string damageText, TMP_SpriteAsset spriteAsset,float size)
    {
        Vector2 finalPos = spawnPosition += new Vector3
        (
            Random.Range(-randomIntensity.x, randomIntensity.x),
            Random.Range(-randomIntensity.y, randomIntensity.y),
            Random.Range(-randomIntensity.z, randomIntensity.z)
        );
        textPopUp.Init(damageText, spriteAsset, finalPos,size);
    }
}
