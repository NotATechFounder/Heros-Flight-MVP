using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers
{
    public class AiViewController : MonoBehaviour
    {
        [SerializeField] string fillPhaseProperty = "_FillPhase";
        MeshRenderer mesh;
        Color mainMaterialColor;
        MaterialPropertyBlock propertyBlock;

        public void Init()
        {
            Debug.Log("INIT?");
            mesh = GetComponentInChildren<MeshRenderer>();
            mainMaterialColor = new Color(255, 255, 255, 0);
            propertyBlock = new MaterialPropertyBlock();
            mesh.GetPropertyBlock(propertyBlock);
       

        }

        public void StartFadeIn(float duration,Action onComplete)
        {
            StartCoroutine(FadeInRoutine(duration,onComplete));
        }

        IEnumerator FadeInRoutine(float duration, Action onComplete)
        {
            var currentDuration = duration;
            var step = 0.01f;
            var fillPhase = Shader.PropertyToID(fillPhaseProperty);
            propertyBlock.SetFloat(fillPhase, 1f);
            float currentFill = 1;
            while ( currentDuration>0)
            {
                currentDuration -= Time.deltaTime;
                currentFill -= Time.deltaTime;
                currentFill = Mathf.Clamp(currentFill, 0, 1);
                propertyBlock.SetFloat(fillPhase, currentFill);
                mesh.SetPropertyBlock(propertyBlock);
                yield return null;
            }
            onComplete.Invoke();
        }
    }
}