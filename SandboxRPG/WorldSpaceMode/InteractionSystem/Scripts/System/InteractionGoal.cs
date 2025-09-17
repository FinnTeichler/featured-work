using UnityEngine;

namespace UBGKO.Interactables
{
    [CreateAssetMenu(fileName = "InteractionGoal", menuName = "Game/Interaction/InteractionGoal")]
    public class InteractionGoal : ScriptableObject
    {
        public InteractableID interactionTarget;
        public InteractionType interactionType;

        //public bool IsCompleted { get; private set; }
        //public void MarkComplete() => IsCompleted = true;

        public bool Matches(InteractableID id, InteractionType type)
        {
            return interactionType == type &&
                   interactionTarget == id;
        }

        public string GetDisplayName()
        {
            return interactionType.displayName + " " + interactionTarget.displayName;
        }
    }
}