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

    public Particle Spawn(string key, Vector3 position, Quaternion rotation)
    {
        Particle particlePrefab = particleBank.GetAsset(key);
        Particle instance = ObjectPoolManager.SpawnObject(particlePrefab, position,rotation,PoolType.Particle);
        return instance;
    }
    public Particle Spawn(Particle particle, Vector3 position, Quaternion rotation)
    {
        
        Particle instance = ObjectPoolManager.SpawnObject(particle, position,rotation,PoolType.Particle);
        return instance;
    }

    public Particle Spawn(string key, Vector3 position)
    {
        return Spawn(key, position, Quaternion.identity);
    }

    public Particle Spawn(Particle particle, Vector3 position)
    {
        return Spawn(particle, position, Quaternion.identity);
    }

    public Particle Spawn(string key, Transform targetTransform)
    {
        Particle particlePrefab = particleBank.GetAsset(key);
        Particle instance = ObjectPoolManager.SpawnObject(particlePrefab, targetTransform, PoolType.Particle);
        return instance;
    }
    public Particle Spawn(Particle particle, Transform targetTransform)
    {
        Particle instance = ObjectPoolManager.SpawnObject(particle, targetTransform, PoolType.Particle);
        return instance;
    }
}
