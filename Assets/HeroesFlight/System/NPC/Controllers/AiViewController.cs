using System;
using System.Collections;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers
{
    public class AiViewController : MonoBehaviour
    {
        MeshRenderer mesh;
        Color mainMaterialColor;
        MaterialPropertyBlock propertyBlock;

        public void Init()
        {
            mesh = GetComponentInChildren<MeshRenderer>();
            mainMaterialColor = new Color(255, 255, 255, 0);
            propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor("_Color", mainMaterialColor);
            mesh.SetPropertyBlock(propertyBlock);
           
        }

        public void StartFadeIn(float duration,Action onComplete)
        {
            StartCoroutine(FadeInRoutine(duration,onComplete));
        }

        IEnumerator FadeInRoutine(float duration, Action onComplete)
        {
            var currentDuration = duration;
            var step = 1 / duration;
            while (currentDuration>0)
            {
                currentDuration -= Time.deltaTime;
                mainMaterialColor.a += step;
                propertyBlock.SetColor("_Color", mainMaterialColor);
                mesh.SetPropertyBlock(propertyBlock);
                yield return new WaitForEndOfFrame();
            }
            onComplete.Invoke();
        }
    }
}