using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UISwipeArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum SwipeDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public Action<SwipeDirection> OnSwipe;

    [SerializeField] private float swipeDistanceThreshold = 50f;
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;

    public void OnPointerDown(PointerEventData eventData)
    {
        touchStartPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        touchEndPos = eventData.position;
        CheckSwipe();
    }

    private void CheckSwipe()
    {
        float swipeDistance = Vector2.Distance(touchStartPos, touchEndPos);

        if (swipeDistance > swipeDistanceThreshold)
        {
            // Calculate the direction of the swipe
            Vector2 swipeDirection = touchEndPos - touchStartPos;

            if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
            {
                if (swipeDirection.x > 0)
                {
                    OnSwipe?.Invoke(SwipeDirection.Right);
                }
                else
                {
                    OnSwipe?.Invoke(SwipeDirection.Left);
                }
            }
            else
            {
                if (swipeDirection.y > 0)
                {
                    OnSwipe?.Invoke(SwipeDirection.Up);
                }
                else
                {
                    OnSwipe?.Invoke(SwipeDirection.Down);
                }
            }
        }
    }
}
