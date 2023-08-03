using System.Collections;
using UnityEngine;

[System.Serializable]
public class BoosterContainer
{
    [SerializeField] private Boost activeBoost;
    [SerializeField] private float currentDuration;
    [SerializeField] private int stackCount = 1;
    [SerializeField] private bool isRunning;
    private MonoBehaviour monoBehaviour;
    private Coroutine countDownRoutine;

    public Boost ActiveBoost => activeBoost;
    public float CurrentDuration => currentDuration;
    public int StackCount => stackCount;
    public bool IsRunning => isRunning;

    public void SetActiveBoost(MonoBehaviour mono, Boost booster)
    {
        isRunning = true;
        stackCount = 1;
        monoBehaviour = mono;
        activeBoost = booster;
        ApplyBoost();
    }

    public void ApplyBoost()
    {
        activeBoost.OnStart?.Invoke();
        currentDuration = activeBoost.boosterSO.BoosterDuration;
        countDownRoutine = monoBehaviour.StartCoroutine(CountDownRoutine());
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
        for (int i = 0; i < stackCount; i++)
        {
            activeBoost.OnEnd?.Invoke();
        }

        activeBoost = null;
        countDownRoutine = null;
        isRunning = false;
    }

    public void ClearBoost(bool triggerEnd)
    {
        if (triggerEnd)
        {
            for (int i = 0; i < stackCount; i++)
            {
                activeBoost.OnEnd?.Invoke();
            }
        }
        monoBehaviour.StopCoroutine(countDownRoutine);
        activeBoost = null;
        countDownRoutine = null;
        isRunning = false;
    }

    public void ResetBoostDuration()
    {
        currentDuration = activeBoost.boosterSO.BoosterDuration;
    }

    public void IncreaseStackCount()
    {
        stackCount++;
        activeBoost.OnStart?.Invoke();
    }
}
