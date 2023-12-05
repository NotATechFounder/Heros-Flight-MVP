using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMinorAnimationManager : MonoBehaviour
{
    [System.Serializable]
    public class AnimationObjectProperties
    {
        [SerializeField] public string ObjectName;

        [Header("Settings")]
        [SerializeField] public RectTransform objectRectT;
        
        [SerializeField] public bool moveInScreenLoop;
        [SerializeField] public bool inverseHLoop;
        [SerializeField] public bool inverseVLoop;
        [Space]
        [SerializeField] public bool moveHorizontal;
        [SerializeField] public bool moveVertical;

        [SerializeField] public Vector2 moveXOffset;
        [SerializeField] public Vector2 moveYOffset;

        [SerializeField] public float moveSpeed;

        [Header("Read Only")]
        [SerializeField] public Vector2 initialPos;
        [SerializeField] public Vector2 minMaxX;
        [SerializeField] public Vector2 minMaxY;
    }

    [SerializeField] private AnimationObjectProperties[] totalAOP;

    private void Start()
    {
        for (int i = 0; i < totalAOP.Length; i++)
        {
            totalAOP[i].initialPos = totalAOP[i].objectRectT.anchoredPosition;

            //totalAOP[i].minMaxX = new Vector2(totalAOP[i].initialPos.x - totalAOP[i].moveXOffset.x, totalAOP[i].initialPos.x + totalAOP[i].moveXOffset.y);
            //totalAOP[i].minMaxY = new Vector2(totalAOP[i].initialPos.y - totalAOP[i].moveYOffset.x, totalAOP[i].initialPos.y + totalAOP[i].moveYOffset.y);
        }
    }

    private void Update()
    {
        MoveAllObjects();
    }

    private void MoveAllObjects()
    {
        for (int i = 0; i < totalAOP.Length; i++)
        {
            totalAOP[i].minMaxX = new Vector2(totalAOP[i].initialPos.x - totalAOP[i].moveXOffset.x, totalAOP[i].initialPos.x + totalAOP[i].moveXOffset.y);
            totalAOP[i].minMaxY = new Vector2(totalAOP[i].initialPos.y - totalAOP[i].moveYOffset.x, totalAOP[i].initialPos.y + totalAOP[i].moveYOffset.y);
        }

        for (int i = 0; i < totalAOP.Length; i++)
        {
            if (totalAOP[i].moveHorizontal)
            {
                if (totalAOP[i].moveInScreenLoop)
                {
                    float newX = totalAOP[i].objectRectT.anchoredPosition.x + (totalAOP[i].inverseHLoop ? totalAOP[i].moveSpeed : -totalAOP[i].moveSpeed) * Time.deltaTime;

                    if (newX > totalAOP[i].minMaxX.y || newX < totalAOP[i].minMaxX.x)
                    {
                        newX = Mathf.Repeat(newX - totalAOP[i].minMaxX.x, totalAOP[i].minMaxX.y - totalAOP[i].minMaxX.x) + totalAOP[i].minMaxX.x;
                        totalAOP[i].objectRectT.anchoredPosition = new Vector2(newX, totalAOP[i].objectRectT.anchoredPosition.y);
                    }
                    else
                    {
                        totalAOP[i].objectRectT.anchoredPosition = new Vector2(newX, totalAOP[i].objectRectT.anchoredPosition.y);
                    }
                }
                else
                {
                    float targetX = totalAOP[i].minMaxX.x + Mathf.PingPong(Time.time * totalAOP[i].moveSpeed, totalAOP[i].minMaxX.y - totalAOP[i].minMaxX.x);
                    float newX = Mathf.MoveTowards(totalAOP[i].objectRectT.anchoredPosition.x, targetX, totalAOP[i].moveSpeed * Time.deltaTime);

                    totalAOP[i].objectRectT.anchoredPosition = new Vector2(newX, totalAOP[i].objectRectT.anchoredPosition.y);
                }
            }

            if (totalAOP[i].moveVertical)
            {
                if (totalAOP[i].moveInScreenLoop)
                {
                    float newY = totalAOP[i].objectRectT.anchoredPosition.y + (totalAOP[i].inverseVLoop ? totalAOP[i].moveSpeed : -totalAOP[i].moveSpeed) * Time.deltaTime;

                    if (newY > totalAOP[i].minMaxY.y || newY < totalAOP[i].minMaxY.x)
                    {
                        newY = Mathf.Repeat(newY - totalAOP[i].minMaxY.x, totalAOP[i].minMaxY.y - totalAOP[i].minMaxY.x) + totalAOP[i].minMaxY.x;
                        totalAOP[i].objectRectT.anchoredPosition = new Vector2(totalAOP[i].objectRectT.anchoredPosition.x, newY);
                    }
                    else
                    {
                        totalAOP[i].objectRectT.anchoredPosition = new Vector2(totalAOP[i].objectRectT.anchoredPosition.x, newY);
                    }
                }
                else
                {
                    float targetY = totalAOP[i].minMaxY.x + Mathf.PingPong(Time.time * totalAOP[i].moveSpeed, totalAOP[i].minMaxY.y - totalAOP[i].minMaxY.x);
                    float newY = Mathf.MoveTowards(totalAOP[i].objectRectT.anchoredPosition.y, targetY, totalAOP[i].moveSpeed * Time.deltaTime);

                    totalAOP[i].objectRectT.anchoredPosition = new Vector2(totalAOP[i].objectRectT.anchoredPosition.x, newY);
                }
            }
        }
    }
}