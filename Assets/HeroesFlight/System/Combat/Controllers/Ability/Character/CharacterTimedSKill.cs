using System;
using System.Collections;
using UnityEngine;



[Serializable]
public class CharacterTimedSKill
{
    public Action OnSkillActivated;
    public Action<float> OnSkillRuntime;
    public Action OnSkillDeactivated;
    public Action<float> OnSkillCoolDown;
    public Action OnSkillReady;
    public bool IsActive { get; private set; }
    public float SkillDuration => skillDuration;

    [SerializeField] private float skillDuration;
    [SerializeField] private float coolDownTime;
    [SerializeField] private MonoBehaviour owner;

    private bool canUseAbility = true;
    private float currentTime = 0;

    public void Init (MonoBehaviour targetOwner)
    {
        owner = targetOwner;
        OnSkillCoolDown?.Invoke(0);
    }

    public virtual void ActivateAbility()
    {
        if (!canUseAbility)
            return;
        OnSkillActivated?.Invoke();
        owner.StartCoroutine(Runtime());
    }

    private IEnumerator Runtime()
    {
        IsActive = true;
        canUseAbility = false;
        currentTime = skillDuration;
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            OnSkillRuntime?.Invoke(currentTime / skillDuration);
            yield return null;
        }
        IsActive = false;
        DeactivateAbility();
    }

    public virtual void DeactivateAbility()
    {
        OnSkillDeactivated?.Invoke();
        owner.StartCoroutine(CoolDown());
    }

    private IEnumerator CoolDown()
    {
        currentTime = coolDownTime;
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            OnSkillCoolDown?.Invoke(currentTime / coolDownTime);
            yield return null;
        }
        canUseAbility = true;
        OnSkillReady?.Invoke();
    }
}