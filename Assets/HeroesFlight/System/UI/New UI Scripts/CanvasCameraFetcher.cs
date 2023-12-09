using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCameraFetcher : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;

    private Camera _mainCamera;
    private void Awake()
    {
       _mainCamera = FindAnyObjectByType<Camera>();
        _canvas.worldCamera = _mainCamera;
    }
}
