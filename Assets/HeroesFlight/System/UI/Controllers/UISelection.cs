using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pelumi.Juicer;

public class UISelection : MonoBehaviour
{
    public Action<RectTransform> OnViewChanged;

    public enum SwipeDirection
    {
        Left,
        Right
    }

    [SerializeField] private RectTransform parent;

    [SerializeField] private CharacterUISlot leftSlot;
    [SerializeField] private CharacterUISlot midSlot;
    [SerializeField] private CharacterUISlot rightSlot;

    [SerializeField] private RectTransform[] targets;

    [SerializeField] private List<RectTransform> targetsNotInView;

    private RectTransform currentTarget => midSlot.characterUI;

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

    public void Initialize(RectTransform[] rectTransforms = null)
    {
        if (rectTransforms != null)
        {
            targets = rectTransforms;
        }

        for (int i = 0; i < targets.Length; i++)
        {
            if(i == 0)
            {
                midSlot.SetCharacterUI(targets[i]);
                OnViewChanged?.Invoke(targets[i]);
            }

            if(i == 1)
            {
                targets[i].localScale = Vector3.one * 0.8f;
                rightSlot.SetCharacterUI(targets[i]);
            }

            if (i > 1 && i == targets.Length - 1)
            {
                targets[i].localScale = Vector3.one * 0.8f;
                leftSlot.SetCharacterUI(targets[i]);
            }

            if (i >= 2 && i != targets.Length - 1)
            {
                targets[i].gameObject.SetActive(false);
                targetsNotInView.Add(targets[i]);
            }
        }
    }

    public void MoveTarget(SwipeDirection swipeDirection)
    {
        switch (swipeDirection)
        {
            case SwipeDirection.Left:

                if (leftSlot.isOccupied)
                {
                    if(targetsNotInView.Count == 0 && !rightSlot.isOccupied)
                    {
                        return;
                    }

                    RectTransform rectTransform = leftSlot.ReadyToMove();
                    rectTransform.SetParent(parent);
                    rectTransform.gameObject.SetActive(false);
                    targetsNotInView.Add(rectTransform);
                }

                if (midSlot.isOccupied)
                {
                    MoveTargetToLeft(midSlot.ReadyToMove());
                }

                if (rightSlot.isOccupied)
                {
                    MoveTargetToMid(rightSlot.ReadyToMove());
                }

                if (targetsNotInView.Count > 0)
                {
                    MoveTargetToRight(targetsNotInView[0]);
                    targetsNotInView[0].gameObject.SetActive(true);
                    targetsNotInView.RemoveAt(0);
                }

                break;
            case SwipeDirection.Right:

                if (rightSlot.isOccupied)
                {
                    if (targetsNotInView.Count == 0 && !leftSlot.isOccupied)
                    {
                        return;
                    }

                    RectTransform rectTransform = rightSlot.ReadyToMove();
                    rectTransform.SetParent(parent);
                    rectTransform.gameObject.SetActive(false);
                    targetsNotInView.Add(rectTransform);
                }

                if (leftSlot.isOccupied)
                {
                    MoveTargetToMid(leftSlot.ReadyToMove());
                }

                if (midSlot.isOccupied)
                {
                    MoveTargetToRight(midSlot.ReadyToMove());
                }

                if (targetsNotInView.Count > 0)
                {
                    MoveTargetToLeft(targetsNotInView[0]);
                    targetsNotInView[0].gameObject.SetActive(true);
                    targetsNotInView.RemoveAt(0);
                }

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

    private void MoveTarget(RectTransform target, CharacterUISlot to)
    {
        target.SetParent(parent);
        target.JuicyMove(to.rectTransform.position, 0.5f)
            .SetOnCompleted(() => OnReachTarget(target, to))
            .Start();
    }

    private void OnReachTarget(RectTransform target, CharacterUISlot to)
    {
        to.SetCharacterUI(target);

        if(target == currentTarget)
        {
            target.JuicyScale(Vector3.one, 0.1f).Start();
            OnViewChanged?.Invoke(target);
        }
        else
        {
            target.JuicyScale(Vector3.one * 0.8f, 0.1f).Start();
        }
    }
}

[Serializable]
public class CharacterUISlot
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
