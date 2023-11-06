using System;
using System.Collections;
using UnityEngine;



[Serializable]
public class TimedAbility
{
    public Action OnActivated;
    public Action<float> OnRuntime;
    public Action OnDeactivated;
    public Action<float> OnCoolDown;
    public Action OnReady;
    public bool IsActive { get; private set; }
    public float Duration => duration;

    [SerializeField] private float duration;
    [SerializeField] private float coolDownTime;
    [SerializeField] private MonoBehaviour owner;

    private bool canUseAbility = true;
    private float currentTime = 0;

    public void Init (MonoBehaviour targetOwner)
    {
        owner = targetOwner;
        OnCoolDown?.Invoke(0);
    }

    public virtual void ActivateAbility()
    {
        if (!canUseAbility)
            return;
        OnActivated?.Invoke();
        owner.StartCoroutine(Runtime());
    }

    private IEnumerator Runtime()
    {
        IsActive = true;
        canUseAbility = false;
        currentTime = duration;
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            OnRuntime?.Invoke(currentTime / duration);
            yield return null;
        }
        IsActive = false;
        DeactivateAbility();
    }

    public virtual void DeactivateAbility()
    {
        OnDeactivated?.Invoke();
        owner.StartCoroutine(CoolDown());
    }

    private IEnumerator CoolDown()
    {
        currentTime = coolDownTime;
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            OnCoolDown?.Invoke(currentTime / coolDownTime);
            yield return null;
        }
        canUseAbility = true;
        OnReady?.Invoke();
    }
}