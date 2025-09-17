using UnityEngine;

namespace FinnTeichler.StateMachine.Old
{
    public class StateMachineV1
    {
        private IStateV1 currentState;

        public IStateV1 CurrentState
        {
            get => currentState;
            private set { currentState = value; }
        }

        public bool debugMode;

        public void Update()
        {
            currentState.OnUpdate();
        }

        public void ChangeState(IStateV1 newState)
        {
            if (debugMode)
                Debug.Log($"{currentState} --> {newState}");

            if (currentState != null)
                currentState.OnExit();

            currentState = newState;
            currentState.OnEnter();
        }
    }
}