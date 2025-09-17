using System;
using System.Collections.Generic;
using System.Linq;
using UBGKO.Interactables;
using UnityEngine;
using UnityEngine.AI;

namespace UBGKO.Party
{
    public class PartyMember : MonoBehaviour
    {
        [SerializeField] private CharacterData data;
        public List<InteractionMotivation> motivations;

        public NavMeshAgent GetAgent() => GetComponent<NavMeshAgent>();
        public Animator GetAnimator() => GetComponentInChildren<Animator>();

        public CharacterData Data => data;
        public void SetData(CharacterData _data) { data = _data; }


        private void Start()
        {
            PartyManager.Instance.AddMember(this);
        }

        public bool IsAlignedWith(InteractionGoal goal)
        {
            return motivations.Any(m => m.IsGoalAligned(goal));
        }

        public bool IsContradictedWith(InteractionGoal goal)
        {
            return motivations.Any(m => m.IsGoalContradicted(goal));
        }

        
    }
}