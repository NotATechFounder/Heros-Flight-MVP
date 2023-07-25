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
        [SerializeField] private List<Menu> menuPrefabList;
        [SerializeField] private ConfirmationUISO backToMenu;
        [SerializeField] private ConfirmationUISO puzzleConfirmation;

        private List<Menu> spawnedMenuList = new List<Menu>();
        private CanvasGroup canvasGroup;
        private Visibility _visibility = Visibility.Visible;

        public void CreateNewInstance<T>()
        {
            Menu menu = menuPrefabList.Find(x => x.GetType() == typeof(T));
            if (menu != null)
            {
                menu = Instantiate(menu, transform);
                spawnedMenuList.Add(menu);
            }
            else Debug.LogError("Menu not found");
        }

        public void ToggleVisibility(Visibility state)
        {
            _visibility = state;
            canvasGroup.alpha = state == Visibility.Visible ? 1 : 0;
            canvasGroup.blocksRaycasts = state == Visibility.Visible;
            canvasGroup.interactable = state == Visibility.Visible;
        }

        public T InitMenu<T>() where T : BaseMenu<T>
        {
            BaseMenu<T> menu = spawnedMenuList.Find(x => x.GetType() == typeof(T)) as BaseMenu<T>;
            if (menu == null)
            {
                CreateNewInstance<T>();
                menu = spawnedMenuList.Find(x => x.GetType() == typeof(T)) as BaseMenu<T>;
            }
            return menu.Init();
        }

        public T OpenMenu<T>() where T : BaseMenu<T>
        {
            BaseMenu<T> menu = spawnedMenuList.Find(x => x.GetType() == typeof(T)) as BaseMenu<T>;
            if (menu == null)
            {
                CreateNewInstance<T>();
                menu = spawnedMenuList.Find(x => x.GetType() == typeof(T)) as BaseMenu<T>;
            }
            menu.Open();
            return menu.GetInstance();
        }

        public void CloseMenu<T>() where T : BaseMenu<T>
        {
            BaseMenu<T> menu = spawnedMenuList.Find(x => x.GetType() == typeof(T)) as BaseMenu<T>;
            if (menu != null)
            {
                menu.Close();
                if(menu.CloseBehaviour == Menu.Close_Behaviour.Destory)
                    spawnedMenuList.Remove(menu);
            } 
            else Debug.LogError("Menu not found");
        }
    }
}

