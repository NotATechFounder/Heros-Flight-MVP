
using System;
using UnityEngine;

public class LevelPortal : MonoBehaviour
{
    [SerializeField] private ParticleSystem impactVfx;
    public event Action OnPlayerEntered;

    public void Enable(Vector2 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            impactVfx.Play();
            OnPlayerEntered?.Invoke();
        }
    }
}
