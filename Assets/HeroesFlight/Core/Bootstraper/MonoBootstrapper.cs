using HeroesFlight.Core.Application;
using HeroesFlight.System.Character;
using HeroesFlight.System.Gameplay;
using HeroesFlight.System.Input;
using HeroesFlight.System.UI;
using StansAssets.Foundation.Patterns;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace HeroesFlight.Core.Bootstrapper
{

   
    public class MonoBootstrapper : MonoBehaviour,IBootstrapper
    {
        IApplication m_Application;
        ServiceLocator m_ServiceLocator;

        void Awake()
        {
            m_Application = new HeroesFlightApplication();
            m_Application.Start(this);
        }

        public ServiceLocator ResolveServices()
        {
            m_ServiceLocator = new ServiceLocator();
            IInputSystem inputSystem = new InputSystem();
            ICharacterSystem characterSystem = new CharacterSystem(inputSystem);
            IUISystem uiSystem = new UiSystem();
            GamePlaySystemInterface gamePlaySystemInterface = new GamePlaySystem(uiSystem,characterSystem);

            m_ServiceLocator.Register(uiSystem);
            m_ServiceLocator.Register(gamePlaySystemInterface);
            m_ServiceLocator.Register(inputSystem);
            m_ServiceLocator.Register(characterSystem);
            return m_ServiceLocator;
        }
    }
}