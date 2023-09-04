using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnironmentHazard : MonoBehaviour
{
    [Header("Core Hazard Settings")]
    [SerializeField] protected RangeValue triggerDelay;

    protected bool isInCooldown;

    public abstract void Trigger();

    protected virtual IEnumerator ActivateCooldown()
    {
        isInCooldown = true;
        yield return new WaitForSeconds(triggerDelay.GetRandomValue());
        isInCooldown = false;
    }
}
