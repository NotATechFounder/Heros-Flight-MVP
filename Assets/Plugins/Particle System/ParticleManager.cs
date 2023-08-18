using Pelumi.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;
    [SerializeField] private ParticleBank particleBank;

    private void Awake()
    {
        instance = this;
    }

    // private void Update()
    // {
    //     Vector2 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         Spawn("GreenBlood", mousepos);
    //     }
    // }

    public Particle Spawn(string key, Vector3 position, Quaternion rotation)
    {
        Particle particlePrefab = particleBank.GetAsset(key);
        Particle instance = ObjectPoolManager.SpawnObject(particlePrefab, position,rotation);
        return instance;
    }

    public Particle Spawn(string key, Vector3 position)
    {
        return Spawn(key, position, Quaternion.identity);
    }

    public Particle Spawn(string key, Transform targetTransform)
    {
        Particle particlePrefab = particleBank.GetAsset(key);
        Particle instance = ObjectPoolManager.SpawnObject(particlePrefab, targetTransform);
        return instance;
    }
}
