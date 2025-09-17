using UnityEngine;
using FinnTeichler.StateMachineSystem;
using SpiritGarden.Creature.Interaction.Data;

namespace SpiritGarden.Creature.Behaviour
{
    public class CB_Fearful : CreatureBehaviour
    {
        [Space(5)][Header("Evade Options")][SerializeField] private float evadeRange = 10f;
        [SerializeField] private float returnIdleRange = 20f;
        [SerializeField] private float moveSpeedEvade = 6f;

        private CBS_EvadeCharacter stateEvadeCharacter;

        private CreatureInfoHub info;

        public override void Start()
        {
            base.Start();

            stateEvadeCharacter = new CBS_EvadeCharacter(creatureReference, moveSpeedEvade);


            info = creatureReference.InfoHub;

            Any(stateEvadeCharacter, 
                new FuncPredicate(() => info.DistanceToCharacterLessThan(evadeRange)));

            At(stateEvadeCharacter, stateIdle, 
                new FuncPredicate(() => info.DistanceToCharacterGreaterEqualsThan(returnIdleRange)));

            At(stateIdle, stateInteractDropItem, 
                new FuncPredicate(() => info.IsInteractingSpecific(InteractionData.InteractionMethod.DROP_ITEM) &&
                                        info.DistanceBetweenPlayerAndDropItemGreaterEqualsThan(returnIdleRange)));

            At(stateInteractDropItem, stateIdle, 
                new FuncPredicate(() => !info.IsInteractingSpecific(InteractionData.InteractionMethod.DROP_ITEM)));


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