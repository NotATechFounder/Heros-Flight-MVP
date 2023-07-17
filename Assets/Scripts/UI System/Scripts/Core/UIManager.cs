using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem
{
    public enum Visibility
    {
        Visible,
        Invisible
    }

    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private List<Menu> _menuPrefabList;

        private List<Menu> _spawnedMenuList = new List<Menu>();
        private CanvasGroup _canvasGroup;
        private Visibility _visibility = Visibility.Visible;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public static void CreateNewInstance<T>()
        {
            Menu menu = Instance._menuPrefabList.Find(x => x.GetType() == typeof(T));
            if (menu != null) Instance.InitializedMenu(menu);
            else Debug.LogError("Menu not found");
        }
        private void InitializedMenu(Menu menu)
        {
            Menu newMenu = Instantiate(menu, transform);
            _spawnedMenuList.Add(newMenu);
        }

        public static void OnMenuOpened(Menu menu)
        {
            Instance._spawnedMenuList.Remove(menu);
        }

        public static void OnMenuClosed(Menu menu)
        {
            Instance._spawnedMenuList.Remove(menu);
        }

        public static void OnMenuDestroyed(Menu menu)
        {
            Instance._spawnedMenuList.Remove(menu);
        }

        public static void ToggleVisibility(Visibility state)
        {
            Instance._visibility = state;
            Instance._canvasGroup.alpha = state == Visibility.Visible ? 1 : 0;
            Instance._canvasGroup.blocksRaycasts = state == Visibility.Visible;
            Instance._canvasGroup.interactable = state == Visibility.Visible;
        }

        public static T OpenMenu<T>() where T : BaseMenu<T>
        {
            BaseMenu<T> menu = Instance._spawnedMenuList.Find(x => x.GetType() == typeof(T)) as BaseMenu<T>;
            if (menu == null)
            {
                CreateNewInstance<T>();
                menu = Instance._spawnedMenuList.Find(x => x.GetType() == typeof(T)) as BaseMenu<T>;
            }
            menu.Open();
            return menu.GetInstance();
        }

        public static T GetMenu<T>() where T : BaseMenu<T>
        {
            BaseMenu<T> menu = Instance._spawnedMenuList.Find(x => x.GetType() == typeof(T)) as BaseMenu<T>;
            if (menu == null)
            {
                Debug.LogError("Menu not found");
                return null;
            }
            return menu.GetInstance();
        }

        public static void CloseMenu<T>() where T : BaseMenu<T>
        {
            BaseMenu<T> menu = Instance._spawnedMenuList.Find(x => x.GetType() == typeof(T)) as BaseMenu<T>;
            if (menu != null) menu.Close();
        }
    }
}

