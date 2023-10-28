using HeroesFlight.Core.Application;
using HeroesFlight.System.Character;
using HeroesFlight.System.Combat;
using HeroesFlight.System.Environment;
using HeroesFlight.System.Gameplay;
using HeroesFlight.System.Input;
using HeroesFlight.System.NPC;
using HeroesFlight.System.Stats;
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

            DataSystemInterface dataSystem = new DataSystem();
            IUISystem uiSystem = new UiSystem(dataSystem);
            InputSystemInterface inputSystem = new InputSystem();
            EnvironmentSystemInterface environmentSystem = new EnvironmentSystem();
            CombatSystemInterface combatSystem = new CombatSystem(environmentSystem,uiSystem);
            CharacterSystemInterface characterSystem = new CharacterSystem(inputSystem);
            NpcSystemInterface npcSystem = new NpcSystem();
            ProgressionSystemInterface progressionSystem = new ProgressionSystem(dataSystem);
            GamePlaySystemInterface gamePlaySystem =
                new GamePlaySystem(dataSystem, characterSystem, npcSystem, environmentSystem, combatSystem,uiSystem,progressionSystem);

            m_ServiceLocator.Register(dataSystem);
            m_ServiceLocator.Register(uiSystem);
            m_ServiceLocator.Register(gamePlaySystem);
            m_ServiceLocator.Register(inputSystem);
            m_ServiceLocator.Register(characterSystem);
            m_ServiceLocator.Register(npcSystem);
            m_ServiceLocator.Register(environmentSystem);
            m_ServiceLocator.Register(combatSystem);
            m_ServiceLocator.Register(progressionSystem);
            return m_ServiceLocator;
        }
    }
}