using System.Collections;
using UnityEngine;
using Pelumi.Juicer;

namespace HeroesFlight.System.NPC.Controllers
{
    public class FlashEffect : MonoBehaviour
    {
        [SerializeField] int DefaultFlashCount = 3;
        [SerializeField] Color flashColor = Color.white;

        [Range(1f / 120f, 1f / 15f)]
        [SerializeField] float interval = 1f / 60f;
        
        [Header("Do not modify")]
        [SerializeField] string fillPhaseProperty = "_FillPhase";
        [SerializeField] string fillColorProperty = "_FillColor";

        MaterialPropertyBlock mpb;
        MeshRenderer meshRenderer;
        WaitForSeconds wait;
        private Coroutine flashRoutine;
        int flashCount = 3;

        void Awake()
        {
            mpb = new MaterialPropertyBlock();
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            flashCount = DefaultFlashCount;
            wait = new WaitForSeconds(interval);
        }

        public void Flash () 
        {
            meshRenderer.GetPropertyBlock(mpb);
            flashRoutine=  StartCoroutine(FlashRoutine(interval));
        }

        public void Flash(float duration)
        {
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
                var fillPhase = Shader.PropertyToID(fillPhaseProperty);
                mpb.SetFloat(fillPhase, 0f);
                meshRenderer.SetPropertyBlock(mpb);
            }
            meshRenderer.GetPropertyBlock(mpb);
            flashRoutine=  StartCoroutine(FlashRoutine(duration));
        }

        IEnumerator FlashRoutine (float duration) {
            wait = new WaitForSeconds(duration);
            if (flashCount < 0) flashCount = DefaultFlashCount;
            var fillPhase = Shader.PropertyToID(fillPhaseProperty);
            var fillColor = Shader.PropertyToID(fillColorProperty);
         

            for (int i = 0; i < flashCount; i++) {
                mpb.SetColor(fillColor, flashColor);
                mpb.SetFloat(fillPhase, 1f);
                meshRenderer.SetPropertyBlock(mpb);
                yield return wait;

                mpb.SetFloat(fillPhase, 0f);
                meshRenderer.SetPropertyBlock(mpb);
                yield return wait;
            }

            yield return null;
        }
    }
}