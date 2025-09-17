using UnityEngine;
using FinnTeichler.StateMachineSystem;
using SpiritGarden.Creature.Interaction.Data;
using Opsive.UltimateInventorySystem.Core;
using FinnTeichler.TimeSystem;

namespace SpiritGarden.Creature.Behaviour
{
    public class CB_Infatuated : CreatureBehaviour
    {
        [Space(5)]
        [Header("Follow Options")]
        [SerializeField] private bool doesFollow;
        [SerializeField] private TimeValue timerFollowMax;
        [SerializeField] private int followThreshold = 100;
        [SerializeField] private float followMaxDistance = 10f;
        [SerializeField] private float followDuration = 10f;

        [Space(5)][Header("Request Options")]
        [SerializeField] private bool doesRequest;
        [SerializeField] private TimeValue timerRequestMax;
        [SerializeField] private int requestInteractionRetries = 3;

        [Space(5)][Header("Orbit Options")]
        [SerializeField] private bool doesOrbit;
        [SerializeField] private TimeValue timerOrbitMax;
        [SerializeField] private float timeIntervalChangePos = 5f;
        [SerializeField] private float radius = 10f;
        [SerializeField] private float orbitMaxDistance = 10f;
        [SerializeField] private float orbitDuration = 10f;

        [Space(5)][Header("Gift Options")]
        [SerializeField] private bool doesGift;
        [SerializeField] private TimeValue timerGiftMax;
        [SerializeField] private ItemDefinition[] randomItemList;

        private CBS_InteractCharacter stateInteractCharacter;
        private CBS_FollowCharacter stateFollowCharacter;
        private CBS_RequestInteraction stateRequestInteraction;
        private CBS_OrbitCharacter stateOrbitCharacter;
        private CBS_GiftItem stateGiftItem;

        private float timerRequest = 0f;
        private float timerGift = 0f;
        private float timerOrbit = 0f;
        private float timerFollow = 0f;

        private bool readyToRequest = false;
        private bool readyToGift = false;
        private bool readyToOrbit = false;
        private bool readyToFollow = false;

        public override void Start()
        {
            base.Start();

            stateInteractCharacter = new CBS_InteractCharacter(creatureReference, interactionDuration);
            stateFollowCharacter = new CBS_FollowCharacter(creatureReference, followDuration);
            stateOrbitCharacter = new CBS_OrbitCharacter(creatureReference, timeIntervalChangePos, radius, orbitDuration);
            stateRequestInteraction = new CBS_RequestInteraction(creatureReference, requestInteractionRetries);
            stateGiftItem = new CBS_GiftItem(creatureReference, randomItemList);

            CreatureInfoHub info = creatureReference.InfoHub;

            At(stateIdle, stateInteractCharacter,
                new FuncPredicate(() => info.IsInteractingSpecific(InteractionData.InteractionMethod.CHARACTER)));

            At(stateInteractCharacter, stateIdle,
                new FuncPredicate(() => !info.IsInteracting &&
                                        info.PreviousInteractionValueLessThan(followThreshold)));

            At(stateInteractCharacter, stateFollowCharacter,
                new FuncPredicate(() => !info.IsInteracting &&
                                        info.PreviousInteractionValueGreaterEqualsThan(followThreshold)));

            At(stateIdle, stateInteractDropItem,
                new FuncPredicate(() => info.IsInteractingSpecific(InteractionData.InteractionMethod.DROP_ITEM)));

            At(stateInteractDropItem, stateIdle,
                new FuncPredicate(() => !info.IsInteracting));

            At(stateInteractCharacter, stateInteractDropItem,
                new FuncPredicate(() => info.IsInteractingSpecific(InteractionData.InteractionMethod.DROP_ITEM)));



            At(stateIdle, stateRequestInteraction,
                new FuncPredicate(() => ReadyToRequest()));

            At(stateRequestInteraction, stateIdle,
                new FuncPredicate(() => stateRequestInteraction.IsDone()));


            At(stateIdle, stateGiftItem,
                new FuncPredicate(() => ReadyToGift()));

            At(stateGiftItem, stateIdle,
                new FuncPredicate(() => stateGiftItem.IsDone));


            At(stateIdle, stateOrbitCharacter,
                new FuncPredicate(() => ReadyToOrbit()));

            At(stateOrbitCharacter, stateIdle,
                new FuncPredicate(() => info.DistanceToCharacterGreaterEqualsThan(orbitMaxDistance) ||
                                        !stateOrbitCharacter.IsWithinDuration()));


            At(stateIdle, stateFollowCharacter,
                new FuncPredicate(() => ReadyToFollow()));

            At(stateFollowCharacter, stateIdle,
                new FuncPredicate(() => info.DistanceToCharacterGreaterEqualsThan(followMaxDistance) ||
                                        !stateFollowCharacter.IsWithinDuration()));



            At(stateRequestInteraction, stateInteractCharacter,
                new FuncPredicate(() => info.IsInteractingSpecific(InteractionData.InteractionMethod.CHARACTER)));

            At(stateRequestInteraction, stateInteractDropItem,
                new FuncPredicate(() => info.IsInteractingSpecific(InteractionData.InteractionMethod.DROP_ITEM)));


            At(stateOrbitCharacter, stateInteractCharacter,
                new FuncPredicate(() => info.IsInteractingSpecific(InteractionData.InteractionMethod.CHARACTER)));

            At(stateOrbitCharacter, stateInteractDropItem,
                new FuncPredicate(() => info.IsInteractingSpecific(InteractionData.InteractionMethod.DROP_ITEM)));


            At(stateFollowCharacter, stateInteractCharacter,
                new FuncPredicate(() => info.IsInteractingSpecific(InteractionData.InteractionMethod.CHARACTER)));

            At(stateFollowCharacter, stateInteractDropItem,
                new FuncPredicate(() => info.IsInteractingSpecific(InteractionData.InteractionMethod.DROP_ITEM)));

            stateMachine.SetState(stateFollowCharacter);
        }

        public override void Update()
        {
            base.Update();

            if (stateMachine.Current.Equals(stateIdle))
            {
                timerRequest += Time.deltaTime;
                timerOrbit += Time.deltaTime;
                timerGift += Time.deltaTime;
                timerFollow += Time.deltaTime;
            }


            if (doesRequest && timerRequest > timerRequestMax.GetValueIn(TimeTickSystem.TimeMeasurement.RealSecond))
            {
                timerRequest = 0f;
                readyToRequest = true;
            }

            if (doesOrbit && timerOrbit > timerOrbitMax.GetValueIn(TimeTickSystem.TimeMeasurement.RealSecond))
            {
                timerOrbit = 0f;
                readyToOrbit = true;
            }

            if (doesGift && timerGift > timerGiftMax.GetValueIn(TimeTickSystem.TimeMeasurement.RealSecond))
            {
                timerGift = 0f;
                readyToGift = true;
            }

            if (doesFollow && timerFollow > timerFollowMax.GetValueIn(TimeTickSystem.TimeMeasurement.RealSecond))
            {
                timerFollow = 0f;
                readyToFollow = true;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        private bool ReadyToRequest()
        {
            if (readyToRequest)
            {
                readyToRequest = false;
                return true;
            }
            else return false;
        }

        private bool ReadyToGift()
        {
            if (readyToGift)
            {
                readyToGift = false;
                return true;
            }
            else return false;
        }

        private bool ReadyToOrbit()
        {
            if (readyToOrbit)
            {
                readyToOrbit = false;
                return true;
            }
            else return false;
        }

        private bool ReadyToFollow()
        {
            if (readyToFollow)
            {
                readyToFollow = false;
                return true;
            }
            else return false;
        }
    }
}