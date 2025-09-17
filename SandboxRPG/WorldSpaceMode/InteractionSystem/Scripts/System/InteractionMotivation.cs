using System.Collections.Generic;
using UBGKO.PartyControls;
using UnityEngine;

namespace UBGKO.Interactables
{  
    [CreateAssetMenu(fileName = "InteractionMotivation", menuName = "Game/Interaction/InteractionMotivation")]
    public class InteractionMotivation : ScriptableObject
    {
        public string motivationName;

        public List<InteractionGoal> alignedGoals = new List<InteractionGoal>();
        public List<InteractionGoal> disallowedGoals = new List<InteractionGoal>();

        public List<InteractionType> alignedTypes = new List<InteractionType>();
        public List<InteractionType> disallowedTypes = new List<InteractionType>();

        public bool IsGoalAligned(InteractionGoal goal)
        {
            if (alignedGoals.Contains(goal)) return true;
            if (alignedTypes.Contains(goal.interactionType)) return true;
            return false;
        }

        public bool IsGoalContradicted(InteractionGoal goal)
        {
            if (disallowedGoals.Contains(goal)) return true;
            if (disallowedTypes.Contains(goal.interactionType)) return true;
            return false;
        }
    }
}