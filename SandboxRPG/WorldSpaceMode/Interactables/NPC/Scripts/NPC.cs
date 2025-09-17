using System.Collections.Generic;
using System.Linq;
using UBGKO.PartyControls;
using UnityEngine;

namespace UBGKO.Interactables
{
    public class NPC : MonoBehaviour, IInteractable
    {
        [Header("Base NPC Settings")]

        [SerializeField] protected InteractableID id;
        [SerializeField] protected string interactableName;

        [Header("Base NPC Interactions")]
        [SerializeField] protected InteractionType inspectInteraction;
        [SerializeField] protected InteractionType talkInteraction;
        [SerializeField] protected InteractionType killInteraction;

        [Header("Interaction Goals")]
        [SerializeField] protected List<InteractionGoal> interactionGoals = new();

        public InteractableID ID => id;

        public string GetInteractableName() => interactableName;
        public virtual Vector3 GetInteractionPoint() => transform.position + transform.forward * 1.5f;
        public virtual Vector3 GetInteractionFacingPoint() => transform.position;

        public virtual List<InteractionType> GetAvailableInteractions()
        {
            return availableInteractions;
        }
        protected List<InteractionType> availableInteractions = new();

        protected virtual void Start()
        {
            availableInteractions.Add(inspectInteraction);
            availableInteractions.Add(talkInteraction);
            availableInteractions.Add(killInteraction);
        }

        public virtual void Interact(InteractionType type)
        {
            if (type == inspectInteraction)
            {
                Debug.Log($"It's {interactableName}.");
            }
            else if (type == talkInteraction)
            {
                // filter out any nulls
                var validGoals = interactionGoals.Where(g => g != null).ToList();

                if (validGoals.Count > 0)
                {
                    int randomIndex = Random.Range(0, validGoals.Count);
                    var selectedGoal = validGoals[randomIndex];
                    InteractionEvents.RaiseGoalRequest(selectedGoal);
                }
                else
                {
                    Debug.Log($"{interactableName} has no valid goals to offer.");
                }
            }
            else if (type == killInteraction)
            {
                Debug.Log($"You killed {interactableName}.");
                Destroy(gameObject);
            }
        }
    }
}