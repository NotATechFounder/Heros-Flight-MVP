using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class TimedAbilityController
{
    public event Action OnActivated;
    public event Action<float> OnRuntimeActive;
    public event Action OnCoolDownStarted;
    public event Action<float> OnCoolDownActive;
    public event Action OnCoolDownEnded;

    public bool IsActive { get; private set; }
    public float Duration { get; private set; }

    private float coolDownTime;
    private MonoBehaviour owner;
    private bool canUseAbility = true;
    private float currentTime = 0;

    public bool IsValid => owner != null;

    public void Init (MonoBehaviour targetOwner, float targetDuration, float targetCoolDownTime)
    {
        if (owner != null)
            owner.StopAllCoroutines();

        owner = targetOwner;
        Duration = targetDuration;
        coolDownTime = targetCoolDownTime;
        OnCoolDownActive?.Invoke(0);
    }

    public virtual bool ActivateAbility()
    {
        if (!canUseAbility)
            return false;
        OnActivated?.Invoke();
        owner.StartCoroutine(Runtime());
        return true;
    }

    private IEnumerator Runtime()
    {
        IsActive = true;
        canUseAbility = false;
        currentTime = Duration;
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            OnRuntimeActive?.Invoke(currentTime / Duration);
            yield return null;
        }
        IsActive = false;
        StartCoolDown();
    }

    public virtual void StartCoolDown()
    {
        OnCoolDownStarted?.Invoke();
        owner.StartCoroutine(CoolDown());
    }

    private IEnumerator CoolDown()
    {
        currentTime = coolDownTime;
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            OnCoolDownActive?.Invoke(currentTime / coolDownTime);
            yield return null;
        }
        canUseAbility = true;
        OnCoolDownEnded?.Invoke();
    }

    public void Refresh()
    {
        if (owner != null)  owner.StopAllCoroutines();
        canUseAbility = true;
        OnCoolDownEnded?.Invoke();
    }
}