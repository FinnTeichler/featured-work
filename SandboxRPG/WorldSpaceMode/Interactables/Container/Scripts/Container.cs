using System.Collections.Generic;
using UnityEngine;
using UBGKO.PartyControls;

namespace UBGKO.Interactables
{
    public class Container : MonoBehaviour, IInteractable
    {
        [SerializeField] private InteractableID id;
        [SerializeField] private string interactableName;
        [SerializeField] private InteractionType inspectInteraction;
        [SerializeField] private InteractionType openInteraction;

        public InteractableID ID => id;

        public string GetInteractableName() => interactableName;
        public Vector3 GetInteractionPoint() => transform.position + transform.forward * 1.5f;
        public Vector3 GetInteractionFacingPoint() => transform.position;

        public List<InteractionType> GetAvailableInteractions()
        {
            return new List<InteractionType> { inspectInteraction, openInteraction };
        }

        public void Interact(InteractionType type)
        {
            if (type == inspectInteraction)
            {
                Debug.Log($"It's a {interactableName}.");
            }
            else if (type == openInteraction)
            {
                Debug.Log($"You opened the {interactableName}.");
            }
        }
    }
}