using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBooster : MonoBehaviour
{
    [SerializeField] private BoosterSpawner boosterSpawner;
    [SerializeField] private CharacterSO characterSO;
    [SerializeField] private CharacterStatController characterStatController;
    [SerializeField] private BoosterManager boosterManager;
    [SerializeField] private BoosterSO boosterSO;
    [SerializeField] public string boosterName;

    private void Start()
    {
        characterStatController.Initialize(characterSO.GetPlayerStatData);
        boosterManager.Initialize(characterStatController);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            boosterSpawner.SpawnBoostLoot(boosterName, transform.position);
           // boosterManager.ActivateBooster(boosterSO);
        }
    }
}
