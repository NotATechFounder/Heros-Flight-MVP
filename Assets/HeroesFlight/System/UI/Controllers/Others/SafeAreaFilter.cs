using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFilter : MonoBehaviour
{
    private void Awake()
    {
        RectTransform recTransform = GetComponent<RectTransform>();
        Rect safeArea = Screen.safeArea;
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = anchorMin + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        recTransform.anchorMin = anchorMin;
        recTransform.anchorMax = anchorMax;
    }
}
