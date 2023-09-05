using Pelumi.Juicer;
using System;
using UnityEngine;

public class WarningLine : MonoBehaviour
{
    public enum WarningLineType
    {
        Stationary,
        Moving
    }

    [Header("WarningLine")]
    [SerializeField] private WarningLineType warningLineType;
    [SerializeField] private float length;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask detectLayer;
    [SerializeField] Color startColor;
    [SerializeField] Color endColor;

    private Vector2 emitterEnd;

    JuicerRuntimeCore<float> warmUpEffect;
    JuicerRuntime colorEffect;
    JuicerRuntime triggerEffect;

    private Action onCompleted;

    private void Start()
    {
        lineRenderer.enabled = false;

        warmUpEffect = lineRenderer.JuicyWidth(1f, 1);
        warmUpEffect.SetOnComplected(() =>
        {
            triggerEffect.Start();
        });

        colorEffect = lineRenderer.material.JuicyColour(endColor, 1f);

        triggerEffect = lineRenderer.JuicyWidth(0, 1);
        triggerEffect.SetOnComplected(() =>
        {
            lineRenderer.enabled = false;
            onCompleted?.Invoke();
        });

        GetEmitterPoints();
    }

    public void Trigger(Action OnFinishedEvent = null, float duration = 1f, float width = 0.5f)
    {
        if (warningLineType == WarningLineType.Moving)
            GetEmitterPoints();

        onCompleted = OnFinishedEvent;
        colorEffect.ChangeDuration(duration);
        warmUpEffect.ChangeDuration(duration);
        warmUpEffect.ChangeDestination(width);
        triggerEffect.ChangeDuration(.15f);
        lineRenderer.enabled = true;
        colorEffect.Start(() => lineRenderer.material.color = startColor);
        warmUpEffect.Start(()=> lineRenderer.widthMultiplier = 0);
    }

    public void GetEmitterPoints()
    {
        lineRenderer.SetPosition(0, transform.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, length, detectLayer);
        emitterEnd = hit.collider != null ? hit.point : transform.position + (-transform.up * length);
        lineRenderer.SetPosition(1, emitterEnd);
    }

    public Vector2 GetFowardDirection => -transform.up;

    public void OtherEffects()
    {
        // lineRenderer.startWidth = startWidth;
        // lineRenderer.endWidth = endWidth;
        // lineRenderer.widthMultiplier = widthMultiplier;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -transform.up * length);

        //if (lineRenderer.positionCount <= 1)
        //{
        //    lineRenderer.positionCount = 2;
        //    lineRenderer.SetPositions(new Vector3[2]);
        //}
        //lineRenderer.SetPosition(1, transform.position + (-transform.up * length));
    }

    private void OnDisable()
    {
        warmUpEffect.Stop();
        colorEffect.Stop();
        triggerEffect.Stop();
    }
}
