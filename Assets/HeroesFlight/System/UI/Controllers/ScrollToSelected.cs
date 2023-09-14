using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


[RequireComponent(typeof(ScrollRect))]
public class ScrollToSelected : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Transform content;
    [SerializeField] private List<RectTransform> galleries;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    private ScrollRect _scrollRect;
    private Vector2 targetPos;
    private Vector2 initialPos;
    private int index;

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
        leftButton.onClick.AddListener(ScrollLeft);
        rightButton.onClick.AddListener(ScrollRight);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ScrollLeft();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            ScrollRight();
        }
    }

    public void ScrollLeft()
    {
        if(galleries.Count > 1)
        StartCoroutine(Move(-1));
    }

    public void ScrollRight()
    {
        if (galleries.Count > 1)
            StartCoroutine(Move(1));
    }

    //public void AddGallery(GalleryBrowseButton galleryBrowseButton)
    //{
    //    galleries.Add(galleryBrowseButton.GetComponent<RectTransform>());
    //}

    public void Clear()
    {
        galleries.Clear();
    }

    private IEnumerator Move(int direction)
    {
        int newIndex = index + direction;

        if (newIndex < 0)
        {
            newIndex = 0; // Stop at the first index
        }
        else if (newIndex >= galleries.Count)
        {
            newIndex = galleries.Count - 1; // Stop at the last index
        }

        if (newIndex != index) // Only update if the index changes
        {
            index = newIndex;

            targetPos = _scrollRect.GetSnapToPositionToBringChildIntoView(galleries[index]);
            initialPos = _scrollRect.content.localPosition;
            float lerpTime = 0f;

            while (lerpTime < moveSpeed)
            {
                lerpTime += Time.deltaTime;

                float t = Mathf.Clamp01(lerpTime / moveSpeed);
                _scrollRect.content.localPosition = Vector2.Lerp(initialPos, targetPos, t);

                yield return null;
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


}


public static class UIUtility
{
    public static Vector2 GetSnapToPositionToBringChildIntoView(this ScrollRect scrollRect, RectTransform child)
    {
        Canvas.ForceUpdateCanvases();

        Vector2 viewportLocalPosition = scrollRect.viewport.localPosition;
        Vector2 childLocalPosition = child.localPosition;

        Vector2 result = new Vector2(
                       0 - (viewportLocalPosition.x + childLocalPosition.x),
                                  0 - (viewportLocalPosition.y + childLocalPosition.y));

        scrollRect.content.localPosition = result;
        return result;
    }
}