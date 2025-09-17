using UnityEngine;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using SpiritGarden.Character;
using Opsive.UltimateInventorySystem.Interactions;

namespace SpiritGarden.Creature.Behaviour
{
    public class CBS_GiftItem : CBStateBase
    {
        protected ItemDefinition[] randomItems;

        private CharacterReference characterReference;

        private GameObject spawnedItemPickUp;

        public bool IsDone { get { return isDone; } }
        private bool isDone = false;

        private float turnDuration = 1f;
        private float turnDurationTimer = 0f;

        public CBS_GiftItem(CreatureReference creatureReference, ItemDefinition[] randomItems) : base(creatureReference)
        {
            this.randomItems = randomItems;
            characterReference = creatureReference.RefGame.RefCharacter;
        }

        public override void OnEnter()
        {
            isDone = false;

            SpawnItemPickUp();
            MoveToCharacter();
        }

        public override void Update()
        {
            if (creatureReference.InfoHub.AgentReachedDestination())
            {
                if (!isDone)
                {
                    turnDurationTimer += Time.deltaTime;
                    float relativeTime = turnDurationTimer / turnDuration;

                    Vector3 direction = characterReference.ModelTransform.position - creatureReference.MainTransform.position;
                    direction.y = 0;
                    Quaternion rotation = Quaternion.LookRotation(direction);
                    creatureReference.MainTransform.rotation = Quaternion.Slerp(creatureReference.MainTransform.rotation, rotation, relativeTime);

                    if (turnDurationTimer >= turnDuration)
                    {
                        turnDurationTimer = 0f;
                        DropItemPickUp();
                    }
                }
            }
            else
            {
                MoveToCharacter();
            }
        }

        public override void OnExit()
        {
            creatureReference.Agent.SetDestination(creatureReference.MainTransform.position);

            isDone = false;
        }

        private void MoveToCharacter()
        {
            Vector3 pos = characterReference.ModelTransform.position + characterReference.ModelTransform.forward * 2f;
            creatureReference.Agent.SetDestination(pos);
        }

        private void SpawnItemPickUp()
        {
            spawnedItemPickUp = GameObject.Instantiate(creatureReference.RefGame.ItemPickupTemplate, creatureReference.TransformFace);

            ItemObject itemObject = spawnedItemPickUp.GetComponent<ItemObject>();
            if (itemObject == null)
                Debug.LogError($"Spawned ItemPickUp does not have Item component on its GameObject. Cannot assign Item that creature should gift. Check if Prefab of {spawnedItemPickUp.name}");
            else
            { 
                ItemDefinition randomItem = randomItems[UnityEngine.Random.Range(0, randomItems.Length)];
                itemObject.SetItem(new ItemInfo(randomItem, 1));
            } 

            EnableComponents(false);
        }

        private void DropItemPickUp()
        {
            spawnedItemPickUp.transform.parent = null;
            spawnedItemPickUp.transform.localScale = Vector3.one;
            EnableComponents(true);

            isDone = true;
        }

        private void EnableComponents(bool enableComponents)
        {
            Collider collider = spawnedItemPickUp.GetComponent<Collider>();
            if (collider == null)
                Debug.LogError($"Spawned ItemPickUp does not have Rigidbody component on its GameObject. Cannot adjust gravity. Check if Prefab of {spawnedItemPickUp.name}");
            else
                collider.enabled = enableComponents;

            Rigidbody rigidBody = spawnedItemPickUp.GetComponent<Rigidbody>();
            if (rigidBody == null)
                Debug.LogError($"Spawned ItemPickUp does not have Rigidbody component on its GameObject. Cannot adjust gravity. Check if Prefab of {spawnedItemPickUp.name}");
            else
            {
                rigidBody.isKinematic = enableComponents;
                rigidBody.useGravity = enableComponents;
            }

            Interactable interactable = spawnedItemPickUp.GetComponent<Interactable>();
            if (interactable == null)
                Debug.LogError($"Spawned ItemPickUp does not have Interactable component on its GameObject. Cannot disable it for behaviour. Check if Prefab of {spawnedItemPickUp.name}");
            else
                interactable.SetIsInteractable(enableComponents);
        }
    }
}