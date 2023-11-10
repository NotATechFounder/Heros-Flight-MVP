using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineUITester : MonoBehaviour
{
    [SerializeField] private ActiveAbilityRerollerNPCMenu activeAbilityRerollerNPCMenu;
    [SerializeField] private ActiveAbilityManager activeAbilityManager;
    [SerializeField] private PassiveAbilityRerollerNPCMenu passiveAbilityRerollerNPCMenu;

    private void Start()
    {
        activeAbilityRerollerNPCMenu.GetEqquipedActiveAbilityTypes += activeAbilityManager.GetEqqipedActiveAbilities;
        activeAbilityRerollerNPCMenu.GetRandomActiveAbilityTypes += activeAbilityManager.GetRandomMultipleActiveAbility;
        activeAbilityRerollerNPCMenu.GetActiveAbilityVisualData += activeAbilityManager.GetActiveAbilityVisualData;
        activeAbilityRerollerNPCMenu.OnActiveAbilitySwapped += activeAbilityManager.SwapActiveAbility;

        passiveAbilityRerollerNPCMenu.GetEqquipedPassiveAbilityTypes += activeAbilityManager.GetEqquipedPassiveAbilities;
        passiveAbilityRerollerNPCMenu.GetRandomPassiveAbilityTypes += activeAbilityManager.GetRandomMultiplePassiveAbility;
        passiveAbilityRerollerNPCMenu.GetPassiveAbilityVisualData += activeAbilityManager.GetPassiveAbilityVisualData;
        passiveAbilityRerollerNPCMenu.OnPassiveAbilitySwapped += activeAbilityManager.SwapPassiveAbility;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
          //  activeAbilityRerollerNPCMenu.LoadAbilities();
            passiveAbilityRerollerNPCMenu.LoadAbilities();
        }
    }
}
