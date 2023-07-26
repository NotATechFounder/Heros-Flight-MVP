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
            rectTransform.sizeDelta = new Vector2(attackRange*2, attackRange*2);
            canvasTransform = visualsCanvas.transform;
        }

        public void SetPosition(Vector2 position)
        {
            canvasTransform.position = position;
        }
    }
}