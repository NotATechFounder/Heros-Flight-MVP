using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlight.System.Environment.Controllers
{
    public class RotateObjectController : MonoBehaviour
    {
        [SerializeField] private Transform rotateTarget;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private Vector2Int rotationDelay;

        private Coroutine rotationRoutine; 
        
        private void Start()
        {
            rotationRoutine = StartCoroutine(RotateTarget());
            
        }

        private void OnDestroy()
        {
            if(rotationRoutine!=null)
                StopCoroutine(rotationRoutine);
        }

        IEnumerator RotateTarget()
        {
            yield return new WaitForSeconds(Random.Range(rotationDelay.x, rotationDelay.y));
    
            while(true)
            {
                rotateTarget.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);  
                yield return null;
            }
        }

    }
}