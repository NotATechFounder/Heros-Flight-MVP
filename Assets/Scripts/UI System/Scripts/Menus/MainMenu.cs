using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class MainMenu : BaseMenu<MainMenu>
    {
        public event Action OnPlayButtonPressed;
        public event Action OnSettingsButtonPressed;

        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;

        public override void OnCreated()
        {
            _playButton.onClick.AddListener(()=>
            {
                OnPlayButtonPressed?.Invoke();
            });

            _settingsButton.onClick.AddListener(() =>
            {
                OnSettingsButtonPressed?.Invoke();
            });
        }

        public override void OnOpened()
        {

        }

        public override void OnClosed()
        {
            CloseMenu();
        }

        public override void ResetMenu()
        {

        }
    }
}

