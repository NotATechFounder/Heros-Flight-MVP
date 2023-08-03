using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBooster : MonoBehaviour
{
    [SerializeField] private CharacterSO characterSO;
    [SerializeField] private CharacterStatController characterStatController;
    [SerializeField] private BoosterManager boosterManager;
    [SerializeField] private BoosterSO boosterSO;

    private void Start()
    {
        characterStatController.Initialize(characterSO.GetPlayerStatData);
        boosterManager.Initialise(characterStatController);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            boosterManager.ActivateBooster(boosterSO);
        }
    }
}
