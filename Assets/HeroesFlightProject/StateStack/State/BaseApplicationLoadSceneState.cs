using HeroesFlight.Core.StateStack.Enum;
using StansAssets.SceneManagement;

namespace HeroesFlight.StateStack.State
{
    public abstract class BaseApplicationLoadSceneState : BaseApplicationState
    {
        protected readonly SceneActionsQueue m_SceneActionsQueue;
        protected readonly ISceneLoadService m_SceneLoadSystem;

        protected BaseApplicationLoadSceneState()
        {
            m_SceneLoadSystem = new SceneLoadService();
            m_SceneActionsQueue = new SceneActionsQueue(m_SceneLoadSystem);
        }

        public override void ChangeState(StackChangeEvent<ApplicationState> evt, IProgressReporter progressReporter) { }
    }
}