using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Menu : MonoBehaviour
    {
        public enum Close_Behaviour 
        {   
            Disable,
            Destory
        }

        public enum Status 
        {
            Opened, 
            Closed 
        }

        [SerializeField] protected int viewPriority = 0;
        [SerializeField] protected Close_Behaviour closeBehaviour = Close_Behaviour.Disable;

        [Header("To Make Private")]
        protected Canvas[] canvas;
        protected Status status = Status.Closed;
        protected Visibility visibility = Visibility.Visible;
        protected CanvasGroup canvasGroup;

        public Close_Behaviour CloseBehaviour => closeBehaviour;
        public Status MenuStatus => status;

        private void OnValidate()
        {
            InitializedAllCanvas();
        }

        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            InitializedAllCanvas();
        }

        public void InitializedAllCanvas()
        {
            canvas = GetComponentsInChildren<Canvas>();

            if (canvas == null) return;

            foreach (Canvas canvas in canvas)
            {
                if (canvas != null)
                    canvas.sortingOrder = viewPriority;
            }
        }

        protected void OpenMenu()
        {
            status = Status.Opened;
            gameObject.SetActive(true);
        }

        protected void CloseMenu()
        {
            status = Status.Closed;
            switch (closeBehaviour)
            {
                case Close_Behaviour.Disable:
                    gameObject.SetActive(false);
                    break;
                case Close_Behaviour.Destory:
                    OnMenuDestroyed();
                    Destroy(gameObject);
                    break;
            }
        }

        public void SetVisibility(Visibility state)
        {
            visibility = state;
            switch (visibility)
            {
                case Visibility.Visible:
                    canvasGroup.alpha = 1;
                    canvasGroup.blocksRaycasts = true;
                    canvasGroup.interactable = true;
                    break;
                case Visibility.Invisible:
                    canvasGroup.alpha = 0;
                    canvasGroup.blocksRaycasts = false;
                    canvasGroup.interactable = false;
                    break;
            }
        }

        public abstract void OnMenuDestroyed();

        public abstract void ResetMenu();
    }
}

