using System;
using FinnTeichler.StateMachineSystem;

namespace SpiritGarden.Creature.Behaviour
{
    [Serializable]
    public class CBStateBase : IState
    {
        protected readonly CreatureReference creatureReference;


        protected CBStateBase(CreatureReference creatureReference)
        {
            this.creatureReference = creatureReference;
        }

        public virtual void OnEnter()
        {
            // noop
        }

        public virtual void Update()
        {
            // noop
        }

        public virtual void FixedUpdate()
        {
            // noop
        }

        public virtual void OnExit()
        {
            // noop
        }
    }
}