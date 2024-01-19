using System;
using HeroesFlight.Core.Application;
using HeroesFlight.System.Character;
using HeroesFlight.System.Combat;
using HeroesFlight.System.Dice;
using HeroesFlight.System.Environment;
using HeroesFlight.System.Gameplay;
using HeroesFlight.System.Input;
using HeroesFlight.System.Inventory;
using HeroesFlight.System.NPC;
using HeroesFlight.System.ShrineSystem;
using HeroesFlight.System.Stats;
using HeroesFlight.System.Stats.Handlers;
using HeroesFlight.System.Tutorial;
using HeroesFlight.System.UI;
using StansAssets.Foundation.Patterns;
using UnityEngine;


namespace HeroesFlight.Core.Bootstrapper
{
    public class MonoBootstrapper : MonoBehaviour, IBootstrapper
    {
        IApplication m_Application;
        ServiceLocator m_ServiceLocator;

        void Awake()
        {
            m_Application = new HeroesFlightApplication();
            UnityEngine.Application.targetFrameRate = 60;
            m_Application.Start(this);
        }

        public ServiceLocator ResolveServices()
        {
            m_ServiceLocator = new ServiceLocator();

            IAuthenticationInterface authenticationSystem = new AuthenticationSystem();
            DataSystemInterface dataSystem = new DataSystem();
            IUISystem uiSystem = new UiSystem(dataSystem);
            InputSystemInterface inputSystem = new InputSystem();
            EnvironmentSystemInterface environmentSystem = new EnvironmentSystem();
            CombatSystemInterface combatSystem = new CombatSystem(environmentSystem,uiSystem);
            CharacterSystemInterface characterSystem = new CharacterSystem(inputSystem);
            NpcSystemInterface npcSystem = new NpcSystem();
            ProgressionSystemInterface progressionSystem = new ProgressionSystem(dataSystem);
            DiceSystemInterface diceSystem = new DiceSystem(uiSystem, dataSystem);
            TraitSystemInterface traitSystem = new TraitsSystem(dataSystem, uiSystem,diceSystem);
            InventorySystemInterface inventorySystem = new InventorySystem(dataSystem,uiSystem);
            RewardSystemInterface rewardSystem = new RewardSystem(dataSystem, inventorySystem, uiSystem);
            IShopSystemInterface shopSystem = new ShopSystem (uiSystem, rewardSystem, inventorySystem, dataSystem);
            ShrineSystemInterface shrineSystem = new ShrineSystem();
            IAchievementSystemInterface achievementSystem = new AchievementSystem(uiSystem,rewardSystem, inventorySystem, combatSystem, environmentSystem, dataSystem,shrineSystem);
            GamePlaySystemInterface gamePlaySystem =
                new GamePlaySystem(dataSystem, characterSystem, npcSystem, environmentSystem, combatSystem,uiSystem,progressionSystem,traitSystem, inventorySystem,achievementSystem,shrineSystem);
            ITutorialInterface tutorialInterface = 
                new TutorialSystem(dataSystem, characterSystem, npcSystem, environmentSystem, combatSystem, uiSystem, progressionSystem, traitSystem, inventorySystem,rewardSystem,shrineSystem);


            m_ServiceLocator.Register(authenticationSystem);
            m_ServiceLocator.Register(dataSystem);
            m_ServiceLocator.Register(uiSystem);
            m_ServiceLocator.Register(gamePlaySystem);
            m_ServiceLocator.Register(inputSystem);
            m_ServiceLocator.Register(characterSystem);
            m_ServiceLocator.Register(npcSystem);
            m_ServiceLocator.Register(environmentSystem);
            m_ServiceLocator.Register(combatSystem);
            m_ServiceLocator.Register(progressionSystem);
            m_ServiceLocator.Register(traitSystem);
            m_ServiceLocator.Register(achievementSystem);
            m_ServiceLocator.Register(diceSystem);
            m_ServiceLocator.Register(inventorySystem);
            m_ServiceLocator.Register(rewardSystem);
            m_ServiceLocator.Register(shopSystem);
            m_ServiceLocator.Register(tutorialInterface);
            m_ServiceLocator.Register(shrineSystem);
            return m_ServiceLocator;
        }

        private void OnDestroy()
        {
            m_ServiceLocator.Get<DataSystemInterface>().RequestDataSave();
        }

        private void OnApplicationQuit()
        {
            m_ServiceLocator.Get<DataSystemInterface>().RequestDataSave();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                m_ServiceLocator.Get<DataSystemInterface>().RequestDataSave();
            }
        }
    }
}