using FinnTeichler.StateMachineSystem;
using SpiritGarden.Creature.Interaction.Data;

namespace SpiritGarden.Creature.Behaviour
{
    public class CB_Habitual : CreatureBehaviour
    {
        private CBS_InteractCharacter stateInteractCharacter;

        public override void Start()
        {
            base.Start();

            stateInteractCharacter = new CBS_InteractCharacter(creatureReference, interactionDuration);

            CreatureInfoHub info = creatureReference.InfoHub;


            At(stateIdle, stateInteractCharacter,
                new FuncPredicate(() => info.IsInteractingSpecific(InteractionData.InteractionMethod.CHARACTER)));

            At(stateInteractCharacter, stateIdle,
                new FuncPredicate(() => !info.IsInteracting));

            At(stateIdle, stateInteractDropItem, 
                new FuncPredicate(() => info.IsInteractingSpecific(InteractionData.InteractionMethod.DROP_ITEM)));

            At(stateInteractDropItem, stateIdle, 
                new FuncPredicate(() => !info.IsInteracting));

            At(stateInteractCharacter, stateInteractDropItem,
                new FuncPredicate(() => info.IsInteractingSpecific(InteractionData.InteractionMethod.DROP_ITEM)));

            stateMachine.SetState(stateIdle);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
    }
}