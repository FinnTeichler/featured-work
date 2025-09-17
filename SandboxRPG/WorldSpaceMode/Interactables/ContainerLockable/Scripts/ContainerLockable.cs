using System.Collections.Generic;
using UnityEngine;
using UBGKO.PartyControls;

namespace UBGKO.Interactables
{
    public class ContainerLockable : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool isLocked = true;
        [Space(10)]
        [SerializeField] private InteractableID id;
        [SerializeField] private string interactableName;

        [SerializeField] private InteractionType inspectInteraction;
        [SerializeField] private InteractionType openInteraction;
        [SerializeField] private InteractionType unlockInteraction;
        [SerializeField] private InteractionType lockInteraction;

        public InteractableID ID => id;

        public string GetInteractableName() => interactableName;
        public Vector3 GetInteractionPoint() => transform.position + transform.forward * 1.5f;
        public Vector3 GetInteractionFacingPoint() => transform.position;

        public List<InteractionType> GetAvailableInteractions()
        {
            List<InteractionType> interactions = new List<InteractionType>();

            interactions.Add(inspectInteraction);

            if (isLocked)
            {
                interactions.Add(unlockInteraction);
            }
            else
            {
                interactions.Add(openInteraction);
                interactions.Add(lockInteraction);
            }

            return interactions;
        }

        public void Interact(InteractionType type)
        {
            if (type == inspectInteraction)
            {
                Debug.Log(isLocked ? "It is a locked chest." : "It is a chest.");
            }
            else if (type == unlockInteraction)
            {
                if (isLocked)
                {
                    isLocked = false;
                    Debug.Log("You unlocked the chest.");
                    // Optional: play sound or animation
                }
                else
                    Debug.Log("The chest is already unlocked.");
            }
            else if (type == openInteraction)
            {
                if (isLocked)
                    Debug.Log("The chest is locked. You can't open it.");
                else
                {
                    Debug.Log("You opened the chest. There's treasure inside!");
                    // Trigger loot, animation, etc.
                }
            }
            else if (type == lockInteraction)
            {
                if(isLocked)
                    Debug.Log("The chest is already locked.");
                else
                {
                    isLocked = true;
                    Debug.Log("You locked the chest.");
                }
            }
        }
    }
}