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
            BoosterSpawner = scene.GetComponent<BoosterSpawner>();
            CurrencySpawner = scene.GetComponent<CurrencySpawner>();
        }

        public ParticleManager ParticleManager { get; private set; }

        public BoosterSpawner BoosterSpawner { get; private set; }

        public CurrencySpawner CurrencySpawner { get; private set; }

        public void Reset()
        {
            ParticleManager = null;
            BoosterSpawner = null;
            CurrencySpawner = null;
        }
    }
}