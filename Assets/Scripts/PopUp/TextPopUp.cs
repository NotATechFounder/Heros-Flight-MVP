using Pelumi.Juicer;
using Pelumi.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextPopUp : MonoBehaviour
{
    [Header("TextPopUp")]
    [SerializeField] private TextMeshPro textMeshPro;
    [SerializeField] private float scaleDuration;
    [SerializeField] private float moveDuration;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float moveLenght;
    [SerializeField] private JuicerRuntime scaleEffect;
    [SerializeField] private JuicerRuntime moveEffect;
    [SerializeField] private JuicerRuntime fadeEffect;

    private void Awake()
    {
        scaleEffect = transform.JuicyScale(Vector3.one, scaleDuration);
        scaleEffect.SetOnComplected(() =>
        {
            moveEffect.Start();
            fadeEffect.Start();
        });

        fadeEffect = textMeshPro.JuicyAlpha(0, fadeDuration).SetDelay(0.1f);
        fadeEffect.SetOnComplected(() =>
        {
            ObjectPoolManager.ReleaseObject(this);
        });
    }

    public void Init(string text, Color color, Vector3 pos)
    {
        textMeshPro.text = text;
        textMeshPro.color = color;
        transform.position = pos;
        Effect();
    }

    public void Effect()
    {
        transform.localScale = Vector3.zero;
        textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 1);
        moveEffect = transform.JuicyMove(transform.position + Vector3.up * moveLenght, moveDuration);
        scaleEffect.Start();
    }
}
