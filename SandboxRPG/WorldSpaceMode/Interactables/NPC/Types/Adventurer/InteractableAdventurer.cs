using System.Collections.Generic;
using UBGKO.Party;
using UnityEngine;

namespace UBGKO.Interactables
{
    public class InteractableAdventurer : NPC
    {
        [Header("Adventurer Settings")]
        [SerializeField] private PartyMember partyMemberPrefab;
        [SerializeField] private InteractionType recruitInteraction;
        [SerializeField] private List<InteractionMotivation> motivations;
        [SerializeField] private CharacterData data;

        protected override void Start()
        {
            base.Start();
            availableInteractions.Add(recruitInteraction);
        }

        public override void Interact(InteractionType type)
        {
            if (type == recruitInteraction)
            {
                Recruit();
            }
            else
            {
                base.Interact(type);
            }
        }

        private void Recruit()
        {
            Debug.Log($"{interactableName} has joined your party!");

            if (partyMemberPrefab != null)
            {
                PartyMember partyMember = Instantiate(partyMemberPrefab, transform.position, transform.localRotation).GetComponent<PartyMember>();
                partyMember.motivations = motivations;
                partyMember.SetData(data);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("No PartyMember prefab assigned!");
            }
        }
    }
}