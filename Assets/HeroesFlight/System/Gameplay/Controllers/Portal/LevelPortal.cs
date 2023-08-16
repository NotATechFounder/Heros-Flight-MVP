
using System;
using UnityEngine;

public class LevelPortal : MonoBehaviour
{
    public event Action OnPlayerEntered;

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
            OnPlayerEntered?.Invoke();
    }
}
