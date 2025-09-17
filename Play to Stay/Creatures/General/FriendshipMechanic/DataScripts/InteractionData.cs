using System;
using UnityEngine;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using System.Collections.Generic;
using static SpiritGarden.Creature.Interaction.CreatureFriendship;

namespace SpiritGarden.Creature.Interaction.Data
{

    [Serializable]
    public class InteractionData

    {
        public enum Type
        {
            PET,
            FEED
        }
        public Type InteractionType { get => interactionType; }
        private Type interactionType;

        public enum InteractionMethod
        {
            ANY,
            CHARACTER,
            DROP_ITEM
        }
        public InteractionMethod Method { get {  return interactionMethod; } }
        private InteractionMethod interactionMethod;

        public ItemInfo ItemInfo { get => itemInfo; }
        private ItemInfo itemInfo;

        public List<InteractionProperty> Properties { get => properties; }
        private List<InteractionProperty> properties;

        public GameObject Character { get => character; }
        private GameObject character;

        public bool IsDropInteraction { get => isDropInteraction; }
        private bool isDropInteraction;

        public GameObject DroppedItem { get => droppedItem; }
        private GameObject droppedItem;

        public InteractionData(InteractionMethod method, GameObject _character, ItemInfo _itemInfo, bool _isDropInteraction, GameObject _droppedItem)
        {
            interactionMethod = method;
            itemInfo = _itemInfo;
            character = _character;
            isDropInteraction = _isDropInteraction;
            droppedItem = _droppedItem;

            var food = InventorySystemManager.GetItemCategory("Food");

            if (itemInfo.Item == null)
                interactionType = Type.PET;

            else if (food.InherentlyContains(itemInfo.Item))
                interactionType = Type.FEED;

            if (itemInfo.Item == null)
            {
                properties = new List<InteractionProperty>();
            }
            else
            {
                Attribute<InteractionDataObject> idoAttribute = itemInfo.Item.ItemDefinition.GetAttribute<Attribute<InteractionDataObject>>("InteractionDataObject");

                if (idoAttribute == null)
                    Debug.LogError($"{itemInfo.Item.name} is missing an InteractionDataObject attribute.");
                else
                {
                    InteractionDataObject ido = idoAttribute.GetValue();
                    if (ido)
                    {
                        properties = new List<InteractionProperty>(ido.InteractionProperties);
                    }
                    else
                    {
                        properties = new List<InteractionProperty>();
                        Debug.LogError($"{itemInfo.Item.name}'s InteractionDataObject attribute is missing an InteractionDataObject.");
                    }
                }
            }
        }
    }
}