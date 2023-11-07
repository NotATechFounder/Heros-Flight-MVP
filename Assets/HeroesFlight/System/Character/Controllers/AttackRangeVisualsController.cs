using UnityEngine;


namespace HeroesFlight.System.Character
{
    public class AttackRangeVisualsController : MonoBehaviour
    {
        Canvas visualsCanvas;
        RectTransform rectTransform;
        Transform canvasTransform;
        float realSizeX;
        Vector2 canvasSize;

        public void Init(float damageZoneSizeX)
        {
            visualsCanvas = GetComponentInChildren<Canvas>();
            rectTransform = visualsCanvas.GetComponent<RectTransform>();

            //rectTransform.sizeDelta = new Vector2(attackRange*1.3f, attackRange*1.3f);
            canvasSize = rectTransform.sizeDelta;
            canvasTransform = visualsCanvas.transform;
            realSizeX = damageZoneSizeX;
        }

        public void SetFacing(bool IsFacingLeft)
        {
            if (IsFacingLeft)
            {
                canvasTransform.localScale = new Vector3(3, 3, 3);
            }
            else
            {
                canvasTransform.localScale = new Vector3(-3, 3, 3);
            }
        }

        public void SetPosition(Vector2 position)
        {
            canvasTransform.localPosition = CalculateVisualFinalPosition(position);
        }

        public void DisableVisuals(bool isDisabled) => visualsCanvas.enabled = !isDisabled;

        Vector2 CalculateVisualFinalPosition(Vector2 currentPosition)
        {
            var position = currentPosition;
            if (currentPosition.x < 0)
            {
                position.x = currentPosition.x - realSizeX / 2 + canvasSize.x / 2;
            }
            else
            {
                position.x = currentPosition.x + realSizeX / 2 - canvasSize.x / 2;
            }

            return position;
        }
    }
}