using Pelumi.Juicer;
using System;
using UnityEngine;

public class WarningLine : MonoBehaviour
{
    public enum WarningMoveType
    {
        Stationary,
        Moving
    }

    public enum WarningVisualType
    {
        LineRenderer,
        SpriteRenderer
    }

    [Header("WarningLine")]
    [SerializeField] private WarningMoveType warningLineType;
    [SerializeField] private WarningVisualType warningVisualType;
    [SerializeField] private float length;
    [SerializeField] private Transform visual;
    [SerializeField] private SpriteRenderer visualSpriteRenderer;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask detectLayer;
    [SerializeField] Color startColor;
    [SerializeField] Color endColor;

    private Vector2 emitterEnd;

    JuicerRuntimeCore<float> warmUpEffect;
    JuicerRuntime colorEffect;
    JuicerRuntime triggerEffect;
    private float distance;
    private Action onCompleted;

    private void Start()
    {
        lineRenderer.enabled = false;
        visualSpriteRenderer.enabled = false;

        switch (warningVisualType)
        {
            case WarningVisualType.LineRenderer:

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

                break;
            case WarningVisualType.SpriteRenderer:

                warmUpEffect = visual.JuicyScaleX(1f, 1);
                warmUpEffect.SetOnComplected(() =>
                {
                    triggerEffect.Start();
                });

                colorEffect = visualSpriteRenderer.material.JuicyColour(endColor, 1f);

                triggerEffect = lineRenderer.JuicyWidth(0, 1);
                triggerEffect.SetOnComplected(() =>
                {
                    visualSpriteRenderer.enabled = false;
                    onCompleted?.Invoke();
                });

                break;
            default: break;
        }

        GetEmitterPoints();
    }

    public void Trigger(Action OnFinishedEvent = null, float duration = 1f, float width = 0.5f)
    {
        if (warningLineType == WarningMoveType.Moving)
            GetEmitterPoints();

        onCompleted = OnFinishedEvent;

        switch (warningVisualType)
        {
            case WarningVisualType.LineRenderer:
                LineRendererTrigger(duration, width);
                break;
            case WarningVisualType.SpriteRenderer:
                SpriteRendererTrigger(duration, width);
                break;
            default:
                break;
        }
    }

    private void LineRendererTrigger(float duration, float width)
    {
        // get 10% of the duration
        float triggerDuration = duration * 0.1f;
        float warmUpDuration = duration - triggerDuration;
        
        colorEffect.ChangeDuration(warmUpDuration);
        warmUpEffect.ChangeDuration(warmUpDuration);
        warmUpEffect.ChangeDestination(width);
        triggerEffect.ChangeDuration(triggerDuration);
        lineRenderer.enabled = true;
        colorEffect.Start(() => lineRenderer.material.color = startColor);
        warmUpEffect.Start(() => lineRenderer.widthMultiplier = 0);
    }

    private void SpriteRendererTrigger(float duration, float width)
    {        
        // get 10% of the duration
        float triggerDuration = duration * 0.1f;
        float warmUpDuration = duration - triggerDuration;

        visual.transform.localScale = new Vector3(width, length, 1);
        colorEffect.ChangeDuration(warmUpDuration);
        warmUpEffect.ChangeDuration(warmUpDuration);
        warmUpEffect.ChangeDestination(width);
        triggerEffect.ChangeDuration(triggerDuration);
        visualSpriteRenderer.enabled = true;
        colorEffect.Start(() => visualSpriteRenderer.material.color = startColor);
        warmUpEffect.Start(() => visual.transform.localScale = new Vector3(0, distance, 1));
    }

    public void GetEmitterPoints()
    {
        lineRenderer.SetPosition(0, transform.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, length, detectLayer);

        switch (warningVisualType)
        {
            case WarningVisualType.LineRenderer:
                emitterEnd = hit.collider != null ? hit.point : transform.position + (-transform.up * length);
                lineRenderer.SetPosition(1, emitterEnd);
                break;
            case WarningVisualType.SpriteRenderer:
                distance = hit.collider != null ? Vector2.Distance(transform.position, hit.point) : length;
                visual.transform.localScale = new Vector3(visual.transform.localScale.x, distance, 1);
                break;
            default:    break;
        }
    }

    public Vector2 GetFowardDirection => -transform.up;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -transform.up * length);
    }
}
