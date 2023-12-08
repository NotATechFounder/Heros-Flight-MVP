using System;
using System.Collections;
using UnityEngine;

namespace HeroesFlight.Utilities
{
    public class DragonHeadMover : MonoBehaviour
    {
        [SerializeField] private Transform dragonTransform;
        [SerializeField] private float speed;
        [SerializeField] private float duration;
        [SerializeField] Vector3 moveDirection;

        private Vector2 initialPos;

        private void Awake()
        {
            initialPos = dragonTransform.localPosition;
        }

        private void OnParticleSystemStopped()
        {
            StartCoroutine(MoveDragonHead());
        }

        IEnumerator MoveDragonHead()
        {
            dragonTransform.gameObject.SetActive(true);
            var currentDuration = duration;
            while (currentDuration>0)
            {
                currentDuration -= Time.deltaTime;
                dragonTransform.position += moveDirection * (speed * Time.deltaTime);
                yield return null;
            }
            
            dragonTransform.gameObject.SetActive(false);
            dragonTransform.localPosition = initialPos;
        }
    }
}