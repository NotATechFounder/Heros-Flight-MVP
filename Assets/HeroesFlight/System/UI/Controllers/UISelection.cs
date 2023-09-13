using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pelumi.Juicer;

public class UISelection : MonoBehaviour
{
    public enum SwipeDirection
    {
        Left,
        Right
    }

    [SerializeField] private RectTransform parent;

    [SerializeField] private CharacterSlotUI leftSlot;
    [SerializeField] private CharacterSlotUI midSlot;
    [SerializeField] private CharacterSlotUI rightSlot;

    [SerializeField] private RectTransform[] targets;
    private RectTransform currentTarget => midSlot.rectTransform;
    private int currentSelection = 0;
    private bool isMoving = false;

    JuicerRuntimeCore<Vector3> juicerRuntimeCore;

    private void Start()
    {
        Initialize();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveTarget(SwipeDirection.Left);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveTarget(SwipeDirection.Right);
        }
    }

    public void Initialize()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            if(i == 0)
            {
                midSlot.SetCharacterUI(targets[i]);
            }

            if(i == 1)
            {
                rightSlot.SetCharacterUI(targets[i]);
            }

            if(i == 2)
            {
                leftSlot.SetCharacterUI(targets[i]);
            }
        }
    }

    public void MoveTarget(SwipeDirection swipeDirection)
    {
        //if (isMoving)
        //{
        //    return;
        //}

        //isMoving = true;

        switch (swipeDirection)
        {
            case SwipeDirection.Left:

                if (leftSlot.isOccupied)
                {
                    return;
                }

                if (midSlot.isOccupied)
                {
                    MoveTargetToLeft(midSlot.ReadyToMove());
                }

                if (rightSlot.isOccupied)
                {
                    MoveTargetToMid(rightSlot.ReadyToMove());
                }

                break;
            case SwipeDirection.Right:

                if (rightSlot.isOccupied)
                {
                    return;
                }

                if (leftSlot.isOccupied)
                {
                    MoveTargetToMid(leftSlot.ReadyToMove());
                }

                if (midSlot.isOccupied)
                {
                    MoveTargetToRight(midSlot.ReadyToMove());
                }

                //if (rightSlot.isOccupied)
                //{
                //    MoveTargetToMid(rightSlot.characterUI);
                //}

                break;
            default:
                break;
        }
    }

    private void MoveTargetToLeft(RectTransform target)
    {
        MoveTarget(target, leftSlot);
    }

    private void MoveTargetToMid(RectTransform target)
    {
        MoveTarget(target, midSlot);
    }

    private void MoveTargetToRight(RectTransform target)
    {
        MoveTarget(target, rightSlot);
    }

    private void MoveTarget(RectTransform target, CharacterSlotUI to)
    {
        target.SetParent(parent);
        target.JuicyMove(to.rectTransform.position, 0.5f)
            .SetOnComplected(() => to.SetCharacterUI(target))
            .Start();
    }
}

[Serializable]
public class CharacterSlotUI
{
    public RectTransform rectTransform;
    public RectTransform characterUI;
    public bool isOccupied => characterUI != null;

    public void SetCharacterUI(RectTransform characterUI)
    {
        this.characterUI = characterUI;
        characterUI.position = rectTransform.position;
        characterUI.SetParent(rectTransform);
    }

    public RectTransform ReadyToMove()
    {
        RectTransform temp = characterUI;
        characterUI = null;
        return temp;
    }

    public void ClearCharacterUI()
    {
        characterUI = null;
    }
}
