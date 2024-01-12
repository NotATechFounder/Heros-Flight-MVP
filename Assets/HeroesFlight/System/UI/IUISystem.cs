using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.UI.Enum;
using HeroesFlight.System.UI.Model;
using UnityEngine;

namespace HeroesFlight.System.UI
{
    public interface IUISystem : SystemInterface
    {
        event Action OnReturnToMainMenuRequest;
        event Action OnRestartLvlRequest;
        event Action<ReviveRequestModel> OnReviveCharacterRequest;
        event Action OnSpecialButtonClicked;
        event Action<int> OnPassiveAbilityButtonClicked;
        public UIEventHandler UiEventHandler { get; }
        void ShowDamageText(float damage, Transform target,
            bool isCritical, bool targetIsPlayer, bool isHeal = false);

        void DisplayStartInfoMessage(float duration);
        void UpdateGameTimeUI(float timeLeft);
        void UpdateEnemiesCounter(int enemiesLeft);
        void UpdateUltimateButton(float value);
        void UpdateSpecialEnemyHealthBar(float value);
        void ToggleSpecialEnemyHealthBar(bool isEnabled);
        void UpdateComboUI(int count);
        void ShowSpecialEnemyWarning(EncounterType encounterType);
        void UpdateRuinShardUi(int amount);
        void ShowPopupAtPosition(string info, Vector2 pos, Color color);
        void AssignGameEvents();
    }
}