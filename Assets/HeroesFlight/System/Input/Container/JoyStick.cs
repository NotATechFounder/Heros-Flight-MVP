using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour,IPointerDownHandler
{
    [SerializeField] private RectTransform joyStick;

    public void OnPointerDown(PointerEventData eventData)
    {
        joyStick.position = eventData.position;
    }
}
