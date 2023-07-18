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
        [SerializeField] private List<Menu> _menuPrefabList;

        private List<Menu> _spawnedMenuList = new List<Menu>();
        private CanvasGroup _canvasGroup;
        private Visibility _visibility = Visibility.Visible;

        public void CreateNewInstance<T>()
        {
            Menu menu = _menuPrefabList.Find(x => x.GetType() == typeof(T));
            if (menu != null)
            {
                menu = Instantiate(menu, transform);
                _spawnedMenuList.Add(menu);
            }
            else Debug.LogError("Menu not found");
        }

        public void ToggleVisibility(Visibility state)
        {
            _visibility = state;
            _canvasGroup.alpha = state == Visibility.Visible ? 1 : 0;
            _canvasGroup.blocksRaycasts = state == Visibility.Visible;
            _canvasGroup.interactable = state == Visibility.Visible;
        }

        public T InitMenu<T>() where T : BaseMenu<T>
        {
            BaseMenu<T> menu = _spawnedMenuList.Find(x => x.GetType() == typeof(T)) as BaseMenu<T>;
            if (menu == null)
            {
                CreateNewInstance<T>();
                menu = _spawnedMenuList.Find(x => x.GetType() == typeof(T)) as BaseMenu<T>;
            }
            return menu.Init();
        }

        public T OpenMenu<T>() where T : BaseMenu<T>
        {
            BaseMenu<T> menu = _spawnedMenuList.Find(x => x.GetType() == typeof(T)) as BaseMenu<T>;
            if (menu == null)
            {
                CreateNewInstance<T>();
                menu = _spawnedMenuList.Find(x => x.GetType() == typeof(T)) as BaseMenu<T>;
            }
            menu.Open();
            return menu.GetInstance();
        }

        public void CloseMenu<T>() where T : BaseMenu<T>
        {
            BaseMenu<T> menu = _spawnedMenuList.Find(x => x.GetType() == typeof(T)) as BaseMenu<T>;
            if (menu != null)
            {
                menu.Close();
                if(menu.CloseBehaviour == Menu.Close_Behaviour.Destory)
                    _spawnedMenuList.Remove(menu);
            } 
            else Debug.LogError("Menu not found");
        }
    }
}

