using Pelumi.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pelumi.ObjectPool
{
    public class Particle : MonoBehaviour
    {
        ParticleSystem parSystem;

        public ParticleSystem GetParticleSystem => parSystem;

        private void OnParticleSystemStopped()
        {
            ObjectPoolManager.ReleaseObject(this);
        }

        private void Awake()
        {
            parSystem = GetComponent<ParticleSystem>();
            var main = parSystem.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        public void Play()
        {
            parSystem.Play();
        }
    }
}

