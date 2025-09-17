using System.Collections.Generic;
using System.Linq;
using UBGKO.Party;
using UnityEngine;

namespace UBGKO.Interactables
{
    public class InteractionGoalManager : MonoBehaviour
    {
        public static InteractionGoalManager Instance { get; private set; }

        private List<InteractionGoal> availableGoals = new();

        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
                Instance = this;
        }

        public void RegisterGoal(InteractionGoal goal)
        {
            availableGoals.Add(goal);
            // Evaluate goal with each character's motivations
        }

        public List<InteractionGoal> GetGoalsAlignedWith(PartyMember partyMember)
        {
            return availableGoals
                .Where(goal => partyMember.IsAlignedWith(goal))
                .ToList();
        }
    }
}