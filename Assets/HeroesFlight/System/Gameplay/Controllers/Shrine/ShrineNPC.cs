using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineNPC : InteractiveNPC
{
    [SerializeField] private ShrineNPCType shrineNPCType;
    [SerializeField] private ShrineNPCFee shrineNPCFee;

    public Action OnPurchaseSuccessful;
    public ShrineNPCType GetShrineNPCType ()=> shrineNPCType;
    public ShrineNPCFee GetShrineNPCFee => shrineNPCFee;

    public void Unlock()
    {
        shrineNPCFee.Unlock();
    }

    protected override void OnEnter2D(Collider2D d)
    {
        if (!shrineNPCFee.Unlocked) return;
        base.OnEnter2D(d);
    }

    protected override void Interact()
    {
        base.Interact();
    }

    public void OnPurchased()
    {
        hasInteracted = true;
        OnPurchaseSuccessful?.Invoke();
    }

    public void ResetInteractivity()
    {
        hasInteracted = false;
    }
}
