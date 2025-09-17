using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UBGKO.Interactables;
using System;

namespace UBGKO.Party
{
    public class PartyManager : MonoBehaviour
    {
        public static PartyManager Instance { get; private set; }

        public IEnumerable<PartyMember> GetPartyMembers() => partyMembers;
        private readonly List<PartyMember> partyMembers = new();

        public List<InteractionGoal> interactionGoals = new();
        public event Action OnInteractionGoalsUpdated;

        private void OnEnable()
        {
            InteractionEvents.OnGoalAccepted += AddInteractionGoal;
            InteractionEvents.OnInteractionPerformed += HandleInteractionPerformed;
        }

        private void OnDisable()
        {
            InteractionEvents.OnGoalAccepted -= AddInteractionGoal;
            InteractionEvents.OnInteractionPerformed -= HandleInteractionPerformed;
        }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void AddMember(PartyMember member)
        {
            if (!partyMembers.Contains(member))
            {
                partyMembers.Add(member);
                Debug.Log($"Added party member: {member.name} ({member.GetInstanceID()})");

                if (partyMembers.ToList().Count == 1)
                    PartyEvents.RaiseOnPartyCreated();
            }
            else
            {
                Debug.LogWarning($"Duplicate party member not added: {member.name} ({member.GetInstanceID()})");
            }
        }

        public void RemoveMember(PartyMember member)
        {
            partyMembers.Remove(member);
        }

        public void AddInteractionGoal(InteractionGoal goal)
        {
            if (!interactionGoals.Contains(goal))
            {
                interactionGoals.Add(goal);
                OnInteractionGoalsUpdated?.Invoke();
            }
        }

        public void RemoveInteractionGoal(InteractionGoal goal)
        {
            if (interactionGoals.Contains(goal))
            {
                interactionGoals.Remove(goal);
                OnInteractionGoalsUpdated?.Invoke();
            }
        }

        private void HandleInteractionPerformed(IInteractable target, InteractionType type)
        {
            for (int i = interactionGoals.Count - 1; i >= 0; i--) // backwards to safely remove
            {
                var goal = interactionGoals[i];

                if (goal.Matches(target.ID, type))
                {
                    Debug.Log($"InteractionGoal completed: {goal.GetDisplayName()}");
                    RemoveInteractionGoal(goal);
                }
            }
        }
    }
}