using System;
using UnityEngine;
using Opsive.Shared.Game;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.ItemActions;
using FinnTeichler.Event.DataObject;

namespace SpiritGarden.Creature.Interaction.Data
{
    [Serializable]
    public class FeedItemAction : ItemAction
    {
        [Tooltip("The pickup item prefab, it must have a ItemPickup component.")]
        [SerializeField] protected GameObject m_PickUpItemPrefab;
        [Tooltip("Feed One item instead of the item amount specified by the item info.")]
        [SerializeField] protected bool m_FeedOne;
        [Tooltip("Remove the item that is fed.")]
        [SerializeField] protected bool m_RemoveOnFeed;
        [Tooltip("The radius where the item should be spawned around the item user.")]
        [SerializeField] protected float m_SpawnRadius = 2f;
        [Tooltip("The center of the random spawn radius.")]
        [SerializeField] protected Vector3 m_CenterOffset;

        public GameObject PickUpItemPrefab { get => m_PickUpItemPrefab; set => m_PickUpItemPrefab = value; }
        public bool FeedOne { get => m_FeedOne; set => m_FeedOne = value; }
        public bool RemoveOnDrop { get => m_RemoveOnFeed; set => m_RemoveOnFeed = value; }
        public float DropRadius { get => m_SpawnRadius; set => m_SpawnRadius = value; }
        public Vector3 CenterOffset { get => m_CenterOffset; set => m_CenterOffset = value; }

        protected GameObject m_PickUpGameObject;
        public GameObject PickUpGameObject => m_PickUpGameObject;

        //[SerializeField] private FeedInteractionDataContainer feedInteractionDataContainer;
        [SerializeField] private GameEvent OnFeedInteraction;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FeedItemAction()
        {
            m_Name = "Feed";
        }

        /// <summary>
        /// Check if the action can be invoked.
        /// </summary>
        /// <param name="itemInfo">The item.</param>
        /// <param name="itemUser">The item user (can be null).</param>
        /// <returns>True if the action can be invoked.</returns>
        protected override bool CanInvokeInternal(ItemInfo itemInfo, ItemUser itemUser)
        {
            return true;
        }

        /// <summary>
        /// Invoke the action.
        /// </summary>
        /// <param name="itemInfo">The item.</param>
        /// <param name="itemUser">The item user (can be null).</param>
        protected override void InvokeActionInternal(ItemInfo itemInfo, ItemUser itemUser)
        {
            if (m_PickUpItemPrefab == null)
            {
                Debug.LogWarning("Item Pickup Prefab is null on the Feed Item Action.");
                return;
            }

            var gameObject = itemUser?.gameObject ?? itemInfo.Inventory?.gameObject;

            if (gameObject == null)
            {
                Debug.LogWarning("The game object where the Item Pickup should spwaned to is null.");
                return;
            }

            if (m_FeedOne) { itemInfo = (1, itemInfo); }

            if (m_RemoveOnFeed)
            {
                itemInfo.ItemCollection?.RemoveItem(itemInfo);
            }

            m_PickUpGameObject = FeedItem(itemInfo, m_PickUpItemPrefab, gameObject.transform.position);

            //feedInteractionDataContainer.GameObject = m_PickUpGameObject;
            //feedInteractionDataContainer.ItemInfo = itemInfo;
            OnFeedInteraction.Raise();
        }

        public static GameObject FeedItem(ItemInfo itemInfo, GameObject prefab, Vector3 position)
        {
            var pickupGameObject = ObjectPool.Instantiate(prefab, position, Quaternion.identity);
            var itemObject = pickupGameObject.GetComponent<ItemObject>();

            if (itemObject != null)
            {
                itemObject.SetItem(itemInfo.Item);
                itemObject.SetAmount(itemInfo.Amount);
                return pickupGameObject;
            }

            var inventory = pickupGameObject.GetComponent<Inventory>();
            if (inventory != null)
            {
                inventory.MainItemCollection.RemoveAll();
                inventory.AddItem(itemInfo);
                return pickupGameObject;
            }

            Debug.LogWarning("The Item Feed Action could not find the ItemPickup or Inventory Pickup component on the fed pickup.");
            return pickupGameObject;
        }
    }
}