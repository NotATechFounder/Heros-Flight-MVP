using System;
using UnityEngine;

namespace UISystem
{
    public abstract class BaseMenu<T> : Menu where T : BaseMenu<T>
    {
        private static BaseMenu<T> Instance;

        public event Action OnMenuOpened;
        public event Action OnMenuClosed;

        private AdvanceButton[] gameButtons;

        protected override void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            else
            {
                base.Awake();
                gameButtons = GetComponentsInChildren<AdvanceButton>(true);
            }
        }

        public T Init()
        {
            Instance = this;
            OnCreated();
            gameObject.SetActive(false);
            return (T)Convert.ChangeType(Instance, typeof(T));
        }

        public T GetInstance(bool enable = true)
        {
            gameObject.SetActive(enable);
            return (T)Convert.ChangeType(Instance, typeof(T));
        }

        public void Open()
        {
            OpenMenu();
            OnOpened();
            OnMenuOpened?.Invoke();
        }

        public void Close()
        {
            OnClosed();
            OnMenuClosed?.Invoke();
        }

        public abstract void OnCreated();

        public abstract void OnOpened();

        public abstract void OnClosed();

        public override void OnMenuDestroyed()
        {
            Instance = null;
        }

        public void SetButtonVibility(GameButtonType gameButtonType, GameButtonVisiblity gameButtonVisiblity)
        {
            foreach (AdvanceButton button in gameButtons)
            {
                if (button.ButtonType == gameButtonType)
                {
                    button.SetVisibility(gameButtonVisiblity);
                    return;
                }
            }
        }

        public void SetButtonVibilityOnly(GameButtonType gameButtonType, GameButtonVisiblity gameButtonVisiblity)
        {
            foreach (AdvanceButton button in gameButtons)
            {
                if (button.ButtonType == gameButtonType)
                {
                    button.SetVisibility(gameButtonVisiblity);
                }
                else
                {
                    button.SetVisibility(gameButtonVisiblity == GameButtonVisiblity.Visible ? GameButtonVisiblity.Hidden : GameButtonVisiblity.Visible);
                }
            }
        }

        public void SetAllButtonVibility(GameButtonVisiblity gameButtonVisiblity)
        {
            foreach (AdvanceButton button in gameButtons)
            {
                button.SetVisibility(gameButtonVisiblity);
            }
        }

        public AdvanceButton GetButton(GameButtonType gameButtonType)
        {
            foreach (AdvanceButton button in gameButtons)
            {
                if (button.ButtonType == gameButtonType)
                {
                    return button;
                }
            }

            return null;
        }
    }
}
