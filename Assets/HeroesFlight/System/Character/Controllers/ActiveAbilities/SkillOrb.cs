using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillOrb : MonoBehaviour
{
    [Header("Orbit")]
    [SerializeField] float moveSpeed = 2.0f;
    [SerializeField] float distance = 2.0f;

    [Header("Bounce")]
    [SerializeField] float bounceAmplitude = 0.1f; // Adjust this value as needed
    [SerializeField] float bounceFrequency = 2.0f; // Adjust this value as needed

    private float timeOffset;

    private Transform target;
    private Vector2 facingDirection;

    void Start()
    {
        // Calculate the initial offset
        facingDirection = transform.position - target.position;
        timeOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    void Update()
    {
        // Calculate the desired position for the orb
        Vector2 desiredPosition = (Vector2)target.position + facingDirection.normalized * distance;

        float yOffset = Mathf.Sin((Time.time + timeOffset) * bounceFrequency) * bounceAmplitude;

        desiredPosition.y += yOffset;

        // Smoothly move the orb towards the desired position
        transform.position = Vector2.Lerp(transform.position, desiredPosition, Time.deltaTime * moveSpeed);

        // Update the orbit offset as the target moves
        facingDirection = transform.position - target.position;

        // Calculate the angle to the target
        float angle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;

        // Rotate the orb to face the target
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void Activate()
    {
        Vector2 rangeDistanceAroundTarget = Random.insideUnitCircle * distance;
        transform.position = (Vector2)target.position + rangeDistanceAroundTarget;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
