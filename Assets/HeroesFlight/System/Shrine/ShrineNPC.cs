using System;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.System.ShrineSystem;
using UnityEngine;

public class ShrineNPC : InteractiveNPC
{
    [SerializeField] private ShrineNPCType shrineNPCType;
    [SerializeField] private GameObject lockIcon;

    [Header("Do not modify")] [SerializeField]
    string fillPhaseProperty = "_FillPhase";

    [SerializeField] string fillColorProperty = "_FillColor";
    MaterialPropertyBlock mpb;
    MeshRenderer meshRenderer;
    private SpriteRenderer spriteRenderer;
    private ShrineNPCFee shrineNPCFee;
    public ShrineNPCType GetShrineNPCType() => shrineNPCType;

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Initialize(ShrineNPCFee shrineNPCFee, Action OnInteract)
    {
        this.shrineNPCFee = shrineNPCFee;
        this.OnInteract += OnInteract;
        shrineNPCFee.OnInteracted = InteractionComplected;
        SetupView();
    }

    private void SetupView()
    {
        lockIcon.SetActive(!shrineNPCFee.Unlocked);
        if (meshRenderer != null)
        {
            var fillPhase = Shader.PropertyToID(fillPhaseProperty);
            var fillColor = Shader.PropertyToID(fillColorProperty);

            mpb.SetFloat(fillPhase, shrineNPCFee.Unlocked ? 0 : 1);
            mpb.SetColor(fillColor, shrineNPCFee.Unlocked ? Color.white : Color.black);
            meshRenderer.SetPropertyBlock(mpb);
        }
        else
        {
            spriteRenderer.color = shrineNPCFee.Unlocked ? Color.white : Color.black;
        }
    }

    protected override void Interact()
    {
        if (!shrineNPCFee.Unlocked) return;
        base.Interact();
    }
}