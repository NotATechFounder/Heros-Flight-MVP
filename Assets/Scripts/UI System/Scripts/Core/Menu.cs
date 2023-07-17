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

        [SerializeField] protected int _viewPriority = 0;
        [SerializeField] protected Close_Behaviour _closeBehaviour = Close_Behaviour.Disable;

        [Header("To Make Private")]
        protected Canvas[] _canvas;
        protected Status status = Status.Closed;
        protected Visibility _visibility = Visibility.Visible;
        protected CanvasGroup _canvasGroup;

        public Close_Behaviour CloseBehaviour => _closeBehaviour;

        private void OnValidate()
        {
            InitializedAllCanvas();
        }

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            InitializedAllCanvas();
        }

        public void InitializedAllCanvas()
        {
            if (_canvas == null) return;

            _canvas = GetComponentsInChildren<Canvas>();

            foreach (Canvas canvas in _canvas)
            {
                if (canvas != null)
                    canvas.sortingOrder = _viewPriority;
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
            switch (_closeBehaviour)
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
            _visibility = state;
            switch (_visibility)
            {
                case Visibility.Visible:
                    _canvasGroup.alpha = 1;
                    _canvasGroup.blocksRaycasts = true;
                    _canvasGroup.interactable = true;
                    break;
                case Visibility.Invisible:
                    _canvasGroup.alpha = 0;
                    _canvasGroup.blocksRaycasts = false;
                    _canvasGroup.interactable = false;
                    break;
            }
        }

        public abstract void OnMenuDestroyed();
    }
}

