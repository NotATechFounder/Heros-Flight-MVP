using System;
using UnityEngine;

namespace UISystem
{
    public abstract class BaseMenu<T> : Menu where T : BaseMenu<T>
    {
        private static BaseMenu<T> Instance;

        public event Action OnMenuOpened;
        public event Action OnMenuClosed;

        protected override void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            else
            {
                base.Awake();
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
    }
}
