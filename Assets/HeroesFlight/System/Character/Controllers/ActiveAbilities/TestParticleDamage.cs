using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestParticleDamage : MonoBehaviour
{
    [SerializeField]   ParticleSystem ps;

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle Collision" + other.name);
    }

    private void OnParticleTrigger()
    {
        Debug.Log("Particle Trigger");
    }
}
