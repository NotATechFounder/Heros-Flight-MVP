using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Pelumi.Juicer;
using System;

public class PuzzlePiece : MonoBehaviour,IPointerClickHandler
{
    public delegate void PuzzlePieceClicked(PuzzlePiece puzzlePiece);

    public event PuzzlePieceClicked OnPuzzlePieceClicked;

    [SerializeField] private Image _image;
    [SerializeField] private int _rotationIndex;

    private readonly int[] rotationAngles = { 0, -90, -180, +90 };

    private bool _inMotion;

    JuicerRuntime juicerRuntime;

    public bool IsCorrectRotation
    {
        get
        {
            return _rotationIndex == 0;
        }
    }

    private void Start()
    {
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        juicerRuntime = transform.JuicyScale(Vector3.one, 0.2f);
        juicerRuntime.Start();
    }

    public void ShuffleRotation()
    {
        _rotationIndex = UnityEngine.Random.Range(0, rotationAngles.Length);
        float rotationAngle = rotationAngles[_rotationIndex];
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, rotationAngle);
        _image.transform.rotation = targetRotation;
    }

    public void SetSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_inMotion)
        {
            return;
        }

        _rotationIndex = (_rotationIndex + 1) % rotationAngles.Length;
        float rotationAngle = rotationAngles[_rotationIndex];

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, rotationAngle);
        _image.transform.JuicyLocalRotateQuaternion(targetRotation, .25f)
            .SetEase(Ease.Spring)
            .SetOnStart(() => _inMotion =  true)
            .SetOnComplected(() => _inMotion = false)
            .Start();

        OnPuzzlePieceClicked?.Invoke(this);
    }
}
