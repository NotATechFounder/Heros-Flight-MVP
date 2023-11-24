using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeroesFlight.System.NPC.Controllers
{
    public class AiViewController : MonoBehaviour
    {
        [SerializeField] string fillPhaseProperty = "_FillPhase";
        [SerializeField] string fillColorProperty = "_FillColor";
        [SerializeField] Color fadeColor;
        MeshRenderer mesh;
        MaterialPropertyBlock propertyBlock;

        public void Init()
        {
            mesh = GetComponentInChildren<MeshRenderer>();
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
            var step =Time.deltaTime / duration;
            var fillPhase = Shader.PropertyToID(fillPhaseProperty);
            var fillColor = Shader.PropertyToID(fillColorProperty);
            propertyBlock.SetColor(fillColor, fadeColor);
            propertyBlock.SetFloat(fillPhase, 1f);
            mesh.SetPropertyBlock(propertyBlock);
            float currentFill = 1;
            while ( currentDuration>0)
            {
                currentDuration -= Time.deltaTime;
                currentFill -= step;
                currentFill = Mathf.Clamp(currentFill, 0, 1);
                propertyBlock.SetFloat(fillPhase, currentFill);
                mesh.SetPropertyBlock(propertyBlock);
                yield return null;
            }
            propertyBlock.SetFloat(fillPhase, 0);
            propertyBlock.SetColor(fillColor, Color.white);
            mesh.SetPropertyBlock(propertyBlock);
            onComplete.Invoke();
        }
    }
}