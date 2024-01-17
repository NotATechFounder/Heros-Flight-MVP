using System;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.System.Shrine;
using HeroesFlight.System.ShrineSystem;
using UnityEngine;

public class ShrineNPC : InteractiveNPC
{
    [SerializeField] private ShrineNPCType shrineNPCType;
    [SerializeField] private GameObject lockIcon;

    [Header("Chat region")] [SerializeField]
    private Trigger2DObserver chatObserver;

    [SerializeField] private Canvas chatCanvas;
    [SerializeField] private float conversationDuration = 3f;

    [Header("Do not modify")] [SerializeField]
    string fillPhaseProperty = "_FillPhase";

    [SerializeField] string fillColorProperty = "_FillColor";
    MaterialPropertyBlock mpb;
    MeshRenderer meshRenderer;
    private SpriteRenderer spriteRenderer;
    private ShrineNPCFee shrineNPCFee;
    public ShrineNPCType GetShrineNPCType() => shrineNPCType;
    private DialogueHandler dialogueHandler;

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        dialogueHandler = new DialogueHandler(chatCanvas, this, conversationDuration);
        chatCanvas.enabled = false;
    }

    public void Initialize(ShrineNPCFee shrineNPCFee, Action OnInteract)
    {
        this.shrineNPCFee = shrineNPCFee;
        this.OnInteract += OnInteract;
        shrineNPCFee.OnInteracted = InteractionComplected;
        chatObserver.OnEnter += TryTriggerConversation;
        SetupView();
    }

    private void TryTriggerConversation(Collider2D obj)
    {
        if (!shrineNPCFee.Unlocked) return;
        
        dialogueHandler.TryTriggerConversation();
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