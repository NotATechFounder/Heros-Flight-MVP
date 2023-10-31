using System;
using System.Collections.Generic;

namespace HeroesFlightProject.System.NPC.State
{
    public class FSMState
    {
        protected enum StatePhase
        {
            Enter,
            Update,
            Exit
        }

        protected FSMState(IFSM stateMachine)
        {
            m_StateMachine = stateMachine;
            GeneratePhaseReactionMap();
        }

      
        protected StatePhase m_CurrentPhase;
       
        protected IFSM m_StateMachine;

        Dictionary<StatePhase, Action> m_StateProcessMap = new ();
       
      
         void GeneratePhaseReactionMap()
        {
            m_StateProcessMap.Add(StatePhase.Enter, Enter);
            m_StateProcessMap.Add(StatePhase.Update, Update);
            m_StateProcessMap.Add(StatePhase.Exit, () =>
            {
              
            });
        }
        public virtual void Enter() { m_CurrentPhase = StatePhase.Update; }

        protected virtual void Update() { m_CurrentPhase = StatePhase.Update; }

        public virtual void Exit() { m_CurrentPhase = StatePhase.Exit;}

        public void Process()
        {
            if (m_StateProcessMap.TryGetValue(m_CurrentPhase, out var response))
            {
                response.Invoke();
            }
        }

    }
}
