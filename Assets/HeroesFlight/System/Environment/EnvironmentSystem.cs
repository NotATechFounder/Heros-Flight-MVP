using System;
using StansAssets.Foundation.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.Environment
{
    public class EnvironmentSystem : EnvironmentSystemInterface
    {
        public void Init(Scene scene = default, Action onComplete = null)
        {
            ParticleManager = scene.GetComponent<ParticleManager>();
            Debug.Log(ParticleManager == null);
        }

        public ParticleManager ParticleManager { get; private set; }

        public void Reset()
        {
            ParticleManager = null;
        }
    }
}