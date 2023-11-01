using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.State
{
    public class FSMachine : IFSM
    {
        protected Dictionary<Type, FSMState> m_StatesLookup = new();
        protected FSMState m_CurrentState;

        public void AddStates(List<FSMState> states)
        {
            foreach (var state in states)
            {
                Debug.Log($"Adding state of type {state.GetType()}");
                m_StatesLookup.Add(state.GetType(), state);
            }
        }

       
        public void Process() => m_CurrentState.Process();

        public void SetState(Type newState)
        {
            if (m_CurrentState != null && m_CurrentState.GetType() == newState)
                return;
           
            Debug.Log($"Setting state to {newState}");
            if (m_StatesLookup.TryGetValue(newState, out var state))
            {
                //m_CurrentState?.Exit();
                m_CurrentState = state;
                m_CurrentState.Enter();
            }
        }

        public FSMState CurrentState => m_CurrentState;
    }
}