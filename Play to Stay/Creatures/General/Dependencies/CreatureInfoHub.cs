using FinnTeichler.Event.Bus;
using SpiritGarden.Character;
using SpiritGarden.Creature.Interaction;
using SpiritGarden.Creature.Interaction.Data;
using UnityEngine;

namespace SpiritGarden.Creature
{
    public class CreatureInfoHub : MonoBehaviour
    {
        [HideInInspector] public CreatureReference creatureReference;

        public virtual void OnEnable()
        {
            SubsribeEventsFocus();
            SubsribeEventsInteraction();
        }

        public virtual void OnDisable()
        {
            UnsubsribeEventsFocus();
            UnsubsribeEventsInteraction();
        }

        #region isFocused
        public bool IsFocused { get { return isFocused; } }
        private bool isFocused;
        private EventBinding<EngageCreatureCharacterState.OnCreatureFocusEnter> onFocusEnterBinding;
        private EventBinding<EngageCreatureCharacterState.OnCreatureFocusExit> onFocusExitBinding;

        private void SubsribeEventsFocus()
        {
            onFocusEnterBinding = new EventBinding<EngageCreatureCharacterState.OnCreatureFocusEnter>(HandleOnFocusEnter);
            EventBus<EngageCreatureCharacterState.OnCreatureFocusEnter>.Register(onFocusEnterBinding);

            onFocusExitBinding = new EventBinding<EngageCreatureCharacterState.OnCreatureFocusExit>(HandleOnFocusExit);
            EventBus<EngageCreatureCharacterState.OnCreatureFocusExit>.Register(onFocusExitBinding);
        }

        private void UnsubsribeEventsFocus()
        {
            EventBus<EngageCreatureCharacterState.OnCreatureFocusEnter>.Deregister(onFocusEnterBinding);
            EventBus<EngageCreatureCharacterState.OnCreatureFocusExit>.Deregister(onFocusExitBinding);
        }

        private void HandleOnFocusEnter(EngageCreatureCharacterState.OnCreatureFocusEnter onCreatureFocusEnter)
        {
            if (creatureReference.MainGameObject == onCreatureFocusEnter.creatureObject)
                isFocused = true;
        }

        private void HandleOnFocusExit(EngageCreatureCharacterState.OnCreatureFocusExit onCreatureFocusExit)
        {
            if (creatureReference.MainGameObject == onCreatureFocusExit.creatureObject)
                isFocused = false;
        }
        #endregion isFocused

        #region isInteracting
        public bool IsInteractingSpecific(InteractionData.InteractionMethod requestedMethod)
        {
            if (isInteracting)
            {
                switch (creatureReference.Friendship.CurrentInteraction.data.Method)
                {
                    case InteractionData.InteractionMethod.ANY:
                        return isInteracting;

                    case InteractionData.InteractionMethod.CHARACTER:
                        if (requestedMethod.Equals(InteractionData.InteractionMethod.CHARACTER))
                            return true;
                        else
                            return false;
                    case InteractionData.InteractionMethod.DROP_ITEM:
                        if (requestedMethod.Equals(InteractionData.InteractionMethod.DROP_ITEM))
                            return true;
                        else
                            return false;
                }
                return false;
            }
            else return false;
        }

        public bool IsInteracting { get { return isInteracting; } }
        private bool isInteracting;
        private EventBinding<CreatureFriendship.OnInteractionEnter> onInteracionEnterBinding;
        private EventBinding<CreatureFriendship.OnInteractionComplete> onInteractionCompleteBinding;
        private EventBinding<CreatureFriendship.OnInteractionAbort> onInteractionAbortBinding;

        private void SubsribeEventsInteraction()
        {
            onInteracionEnterBinding = new EventBinding<CreatureFriendship.OnInteractionEnter>(HandleOnInteractionEnter);
            EventBus<CreatureFriendship.OnInteractionEnter>.Register(onInteracionEnterBinding);

            onInteractionCompleteBinding = new EventBinding<CreatureFriendship.OnInteractionComplete>(HandleOnInteractionComplete);
            EventBus<CreatureFriendship.OnInteractionComplete>.Register(onInteractionCompleteBinding);

            onInteractionAbortBinding = new EventBinding<CreatureFriendship.OnInteractionAbort>(HandleOnInteractionAbort);
            EventBus<CreatureFriendship.OnInteractionAbort>.Register(onInteractionAbortBinding);
        }

        private void UnsubsribeEventsInteraction()
        {
            EventBus<CreatureFriendship.OnInteractionEnter>.Deregister(onInteracionEnterBinding);
            EventBus<CreatureFriendship.OnInteractionComplete>.Deregister(onInteractionCompleteBinding);
            EventBus<CreatureFriendship.OnInteractionAbort>.Deregister(onInteractionAbortBinding);
        }

        private void HandleOnInteractionEnter(CreatureFriendship.OnInteractionEnter onInteractionEnter)
        {
            if (creatureReference.MainGameObject == onInteractionEnter.creatureObject)
            {
                isInteracting = true;
            }
        }

        private void HandleOnInteractionComplete(CreatureFriendship.OnInteractionComplete onInteractionComplete)
        {
            if (creatureReference.MainGameObject == onInteractionComplete.creatureObject)
            {
                isInteracting = false;
            }
        }

        private void HandleOnInteractionAbort(CreatureFriendship.OnInteractionAbort onInteractionAbort)
        {
            if (creatureReference.MainGameObject == onInteractionAbort.creatureObject)
            {
                isInteracting = false;
            }
        }

        #endregion isInteracting


        #region isInteractingCharacter
        public bool IsInteractingCharacter()
        {
            return creatureReference.Friendship.CurrentInteraction.data.IsDropInteraction == false;
        }
        #endregion isInteractingCharacter


        #region isInteractingDrop
        public bool IsInteractingDropItem()
        {
            return creatureReference.Friendship.CurrentInteraction.data.IsDropInteraction;
        }
        #endregion isInteractingDrop


        #region evaluatedInteractionMoreThan
        public bool IsInteractingWithEvaluationGreaterEqualsThan(int comparison)
        {
            if (creatureReference.Friendship.CurrentInteraction.valueChange >= comparison)
                return true;
            else
                return false;
        }
        #endregion evaluatedInteractionMoreThan


        #region evaluatedInteractionLessThan
        public bool IsInteractingWithEvaluationLessThan(int comparison)
        {
            if (creatureReference.Friendship.CurrentInteraction.valueChange < comparison)
                return true;
            else
                return false;
        }
        #endregion evaluatedInteractionLessThan

        public bool PreviousInteractionValueGreaterEqualsThan(int comparison)
        {
            if (creatureReference.Friendship.PreviousInteraction.valueChange >= comparison)
                return true;
            else
                return false;
        }

        public bool PreviousInteractionValueLessThan(int comparison)
        {
            if (creatureReference.Friendship.PreviousInteraction.valueChange < comparison)
                return true;
            else
                return false;
        }


        public float DistanceToCharacter()
        {
            return (creatureReference.MainTransform.position - creatureReference.RefGame.RefCharacter.ModelTransform.position).magnitude;
        }

        public bool DistanceToCharacterGreaterEqualsThan(float comparison)
        {
            if(DistanceToCharacter() >= comparison) return true;
            else return false;
        }

        public bool DistanceToCharacterLessThan(float comparison)
        {
            if (DistanceToCharacter() < comparison) return true;
            else return false;
        }

        public float DistanceBetweenPlayerAndDropItem()
        {
            if (IsInteractingSpecific(InteractionData.InteractionMethod.DROP_ITEM))
            {
                if (creatureReference.Friendship.CurrentInteraction.data.DroppedItem)
                    return (creatureReference.RefGame.RefCharacter.MainTransform.position - creatureReference.Friendship.CurrentInteraction.data.DroppedItem.transform.position).magnitude;
                else
                {
                    Debug.LogError($"Requested distance between Dropped Item and {creatureReference.MainGameObject.name}. This is impossible, because CurrentInteraction is not a DropInteraction. ");
                    return 0f;
                }
            }
            return 0f;
        }

        public bool DistanceBetweenPlayerAndDropItemGreaterEqualsThan(float comparison)
        {
            if (DistanceBetweenPlayerAndDropItem() >= comparison) return true;
            else return false;
        }

        public bool DistanceBetweenPlayerAndDropItemLessThan(float comparison)
        {
            if (DistanceBetweenPlayerAndDropItem() < comparison) return true;
            else return false;
        }

        public bool AgentReachedDestination()
        {
            if (!creatureReference.Agent.pathPending)
            {
                if (creatureReference.Agent.remainingDistance <= creatureReference.Agent.stoppingDistance)
                {
                    if (!creatureReference.Agent.hasPath || creatureReference.Agent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}