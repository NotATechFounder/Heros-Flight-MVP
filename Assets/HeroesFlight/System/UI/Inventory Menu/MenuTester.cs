using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;

public class MenuTester : MonoBehaviour
{
    [SerializeField] private StatPoints statPoints;
    [SerializeField] private StatPointsMenu statPointsMenu;

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            statPointsMenu.GetCurrentSpLevel += statPoints.GetSp;
            statPointsMenu.OnAddSpClicked += statPoints.TryAddSp;
            statPointsMenu.OnRemoveSpClicked += statPoints.TrytRemoveSp;
            statPointsMenu.GetAvailabletSp += statPoints.GetAvailableSp;
            statPointsMenu.OnCompletePressed += statPoints.Confirm;

            statPointsMenu.CacheStatUI();
            statPointsMenu.InitStatUI();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            statPointsMenu.OnCreated();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
    }
}
