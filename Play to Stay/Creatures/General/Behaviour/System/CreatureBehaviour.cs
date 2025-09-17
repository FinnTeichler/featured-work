using UnityEngine;
using FinnTeichler.StateMachineSystem;
using FinnTeichler.Event.Bus;
using SpiritGarden.Character;
using SpiritGarden.Creature.Interaction.Data;

namespace SpiritGarden.Creature.Behaviour
{
    public class CreatureBehaviour : MonoBehaviour
    {
        public CreatureBehaviourID BehaviourID { get { return behaviourID; } }
        [SerializeField] private CreatureBehaviourID behaviourID;
        [Space(10)][SerializeField] protected float interactionDuration = 1f;


        [HideInInspector] public bool debugMode;
        [HideInInspector] public CreatureReference creatureReference;


        protected StateMachine stateMachine;
        protected void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        protected void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);
        public IState stateCurrent { get { return stateMachine.Current; } }
        protected CBS_Idle stateIdle;
        protected CBS_InteractDropItem stateInteractDropItem;


        public virtual void Start()
        {
            stateMachine = new StateMachine();
            stateMachine.debugMode = debugMode;

            stateIdle = new CBS_Idle(creatureReference);
            stateInteractDropItem = new CBS_InteractDropItem(creatureReference, interactionDuration);
        }

        public virtual void Update()
        {
            stateMachine.Update();
        }

        public virtual void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }
    }
}