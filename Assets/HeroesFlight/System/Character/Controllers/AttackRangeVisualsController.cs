using UnityEngine;


namespace HeroesFlight.System.Character
{
    public class AttackRangeVisualsController : MonoBehaviour
    {
        [SerializeField] Canvas visualsCanvas;
        RectTransform rectTransform;
        Transform canvasTransform;

        public void Init(float attackRange)
        {
            rectTransform = visualsCanvas.GetComponent<RectTransform>();
            //rectTransform.sizeDelta = new Vector2(attackRange*1.3f, attackRange*1.3f);
            canvasTransform = visualsCanvas.transform;
        }

        public void SetPosition(Vector2 position)
        {
            canvasTransform.localPosition = position;
        }

        public void DisableVisuals(bool isDisabled) => visualsCanvas.enabled = !isDisabled;
       
    
    }
}