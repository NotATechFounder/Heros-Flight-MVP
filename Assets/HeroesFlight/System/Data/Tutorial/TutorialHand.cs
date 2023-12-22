using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pelumi.Juicer;

public class TutorialHand : MonoBehaviour
{
    [SerializeField] private Vector3 maxScale = new Vector3(1.2f, 1.2f, 1.2f);
    [SerializeField] private float moveDistance = 10f;
    [SerializeField] private Transform hand;
    private JuicerRuntime handScaleEffect;
    private JuicerRuntime handMoveEffect;

    private void Awake()
    {
        handScaleEffect = hand.JuicyScale(maxScale, 0.5f).SetLoop(-1);
        handMoveEffect = hand.JuicyLocalMove(new Vector3(hand.transform.localPosition.x, hand.transform.localPosition.y + moveDistance, hand.transform.localPosition.z), 0.5f).SetLoop(-1);
    }

    public void ShowHand(RectTransform parent)
    {
        transform.position = parent.position;
        transform.SetParent(parent);
        hand.gameObject.SetActive(true);
        handScaleEffect.Start();
        handMoveEffect.Start();
    }

    public void HideHand()
    {
        hand.gameObject.SetActive(false);
        handScaleEffect.Stop();
        handMoveEffect.Stop();
    }
}
