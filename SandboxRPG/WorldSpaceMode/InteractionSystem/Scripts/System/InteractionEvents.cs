using System;
using UnityEngine;

namespace UBGKO.Interactables
{
    public static class InteractionEvents
    {
        public static event Action<IInteractable, Vector2> RequestInteractionMenu;
        public static void RaiseMenuRequest(IInteractable target, Vector2 screenPosition)
        {
            RequestInteractionMenu?.Invoke(target, screenPosition);
        }

        public static event Action<IInteractable, InteractionType> OnInteractionPerformed;
        public static void RaiseInteraction(IInteractable target, InteractionType type)
        {
            OnInteractionPerformed?.Invoke(target, type);
        }

        public static event Action<InteractionGoal> OnGoalRequested;
        public static void RaiseGoalRequest(InteractionGoal goal)
        {
            OnGoalRequested?.Invoke(goal);
        }

        public static event Action<InteractionGoal> OnGoalAccepted;
        public static void RaiseGoalAccepted(InteractionGoal goal)
        {
            OnGoalAccepted?.Invoke(goal);
        }

        public static event Action<InteractionGoal> OnGoalDeclined;
        public static void RaiseGoalDeclined(InteractionGoal goal)
        {
            OnGoalDeclined?.Invoke(goal);
        }
    }
}