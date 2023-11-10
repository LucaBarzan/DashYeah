using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DashYeah.Components
{
    public class StateMachine
    {
        public int CurrentState
        {
            get => currentState;
            set
            {
                if (value == currentState)
                    return;

                exitStates[currentState]?.Invoke();
                currentState = value;
                enterStates[currentState]?.Invoke();
            }
        }

        private int currentState;

        Dictionary<int, System.Action> enterStates = new Dictionary<int, System.Action>();
        Dictionary<int, System.Action> updateStates = new Dictionary<int, System.Action>();
        Dictionary<int, System.Action> exitStates = new Dictionary<int, System.Action>();

        public void AddEnterState(int state, System.Action callback)
        {
            if(!enterStates.ContainsKey(state))
            {
                enterStates.Add(state, callback);
            }
        }

        public void AddUpdateState(int state, System.Action callback)
        {
            if (!updateStates.ContainsKey(state))
            {
                updateStates.Add(state, callback);
            }
        }

        public void AddExitState(int state, System.Action callback)
        {
            if (!exitStates.ContainsKey(state))
            {
                exitStates.Add(state, callback);
            }
        }

        public void UpdateState()
        {
            updateStates[currentState]?.Invoke();
        }
    }
}