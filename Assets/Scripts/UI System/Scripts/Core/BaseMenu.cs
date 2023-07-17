using System;
using UnityEngine;

namespace UISystem
{
    public abstract class BaseMenu<T> : Menu where T : BaseMenu<T>
    {
        private static BaseMenu<T> Instance;

        protected override void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            else
            {
                base.Awake();
                Instance = this;
                OnCreated();
            }
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
        }

        public void Close()
        {
            OnClosed();
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
