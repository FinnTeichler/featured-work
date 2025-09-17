using System;
using UnityEngine;
using SpiritGarden.Creature.Behaviour;

namespace SpiritGarden.Creature.Interaction.Data
{

    [Serializable]
    public class FriendshipLevel
    {
        public int Cost { get => cost; }
        [SerializeField] private int cost;

        public string Name { get => name; }
        [SerializeField] private string name;

        public Color UIColor { get => uiColor; }
        [SerializeField] private Color uiColor;

        public CreatureBehaviourID BehaviourID { get => behaviourID; }
        [SerializeField] private CreatureBehaviourID behaviourID;
    }
}