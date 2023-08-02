using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterContainer : MonoBehaviour
{
    [SerializeField] private Boost activeBoost;
    private float currentDuration;

    public Boost ActiveBoost => activeBoost;

    public void SetActiveBoost(Boost booster)
    {
        if(activeBoost != null && activeBoost.boosterSO.BoosterEffectType == booster.boosterSO.BoosterEffectType)
        {
            currentDuration = activeBoost.boosterSO.BoosterDuration;
            return;
        }
        activeBoost = booster;
        ApplyBoost();
    }

    public void ApplyBoost()
    {
        activeBoost.OnStart?.Invoke();
        CountDown();
    }

    public void CountDown()
    {
        currentDuration = activeBoost.boosterSO.BoosterDuration;
        StartCoroutine(CountDownRoutine());
    }

    IEnumerator CountDownRoutine()
    {
        while (currentDuration > 0)
        {
            currentDuration -= Time.deltaTime;
            yield return null;
        }
        RemoveBoost();
    }

    public void RemoveBoost()
    {
        activeBoost.OnEnd?.Invoke();
        activeBoost = null;
    }
}
