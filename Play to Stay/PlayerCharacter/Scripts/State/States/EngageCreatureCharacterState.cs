using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using Opsive.UltimateInventorySystem.ItemActions;
using FinnTeichler.Event.Bus;
using SpiritGarden.Creature.Interaction.Data;
using SpiritGarden.Creature;

namespace SpiritGarden.Character
{
    public class EngageCreatureCharacterState : CharacterState
    {
        public EngageCreatureCharacterState(CharacterReference characterReference)
        {
            this.characterReference = characterReference;
        }

        private CharacterReference characterReference;

        private GameObject focusPrevious;
        private GameObject focusCurrent;

        private float deadZoneThreshold = 0.02f;
        private Vector3 handTargetPosDefault;

        private DropItemAction dropItemAction;
        private GameObject itemPickUpPrefab;

        private CreatureReference creatureReference;

        public struct OnEngageCreatureEnter : IEvent { }
        public struct OnEngageCreatureExit : IEvent { }

        public struct OnCreatureFocusEnter : IEvent { public GameObject creatureObject; }
        public struct OnCreatureFocusExit : IEvent { public GameObject creatureObject; }

        public Action<GameObject> OnItemDropped;

        public override void OnEnter()
        {
            characterReference.CharacterControls.activated = false;
            characterReference.CreatureSensorCamera.enabled = true;
            characterReference.CreatureSensorModel.enabled = true;

            characterReference.LookAnimatorHead.enabled = true;
            characterReference.LookAnimatorHead.SetLookTarget(null);
            //characterReference.LookAnimatorHand.enabled = true;
            //characterReference.LookAnimatorHand.SetLookTarget(characterReference.LookAnimatorHandTarget);
            
            //handTargetPosDefault = characterReference.LookAnimatorHandTarget.position;

            EventBus<OnEngageCreatureEnter>.Raise(new OnEngageCreatureEnter { });
        }

        public override void OnExit()
        {
            characterReference.CharacterControls.activated = true;
            characterReference.CreatureSensorCamera.enabled = false;
            characterReference.CreatureSensorModel.enabled = false;

            characterReference.LookAnimatorHead.enabled = true;
            characterReference.LookAnimatorHead.SetLookTarget(null);
            //characterReference.LookAnimatorHand.enabled = false;
            //characterReference.LookAnimatorHand.SetLookTarget(null);


            //characterReference.LookAnimatorHandTarget.position = handTargetPosDefault;

            if (focusCurrent != null)
            {
                EventBus<OnCreatureFocusExit>.Raise(new OnCreatureFocusExit { creatureObject = focusCurrent });
            }
            focusCurrent = null;
            focusPrevious = null;

            EventBus<OnEngageCreatureExit>.Raise(new OnEngageCreatureExit { });
        }

        public override void OnUpdate()
        {
            HandleCreatureFocus();
            //HandleHandMovement();
        }

        private void HandleCreatureFocus()
        {
            GameObject focusCurrentCamera = characterReference.CreatureSensorCamera.GetNearest();
            //GameObject focusCurrentModel = characterReference.CreatureSensorModel.GetNearest();

            //if (focusCurrentCamera == focusCurrentModel && focusCurrentCamera != null)
            //{
            //    focusCurrent = focusCurrentModel;
            //}

            if (focusCurrentCamera != null)
            {
                focusCurrent = focusCurrentCamera;
            }

            if (focusCurrent == null)
            {
                if (focusPrevious != null)
                {
                    EventBus<OnCreatureFocusExit>.Raise(new OnCreatureFocusExit { creatureObject = focusPrevious });
                    focusPrevious = focusCurrent;

                    characterReference.LookAnimatorHead.SetLookTarget(null);
                    //characterReference.LookAnimatorHand.SetLookTarget(characterReference.LookAnimatorHandTarget);
                }
            }
            else
            {
                if (ReceivedDropItemInput())
                    ManuallyCallDropItemAction();

                if (focusCurrent != focusPrevious)
                {
                    EventBus<OnCreatureFocusEnter>.Raise(new OnCreatureFocusEnter { creatureObject = focusCurrent });
                    focusPrevious = focusCurrent;

                    creatureReference = focusCurrent.GetComponent<CreatureReference>();
                    if (creatureReference)
                    {
                        ItemSlotCollection collection = characterReference.Inventory.GetItemCollection("Equipped") as ItemSlotCollection;
                        ItemInfo itemInfo = collection.GetItemInfoAtSlot(0);

                        InteractionData interactionData = new InteractionData(InteractionData.InteractionMethod.CHARACTER, characterReference.MainGameObject, itemInfo, false, null);
                        //creatureReference.Friendship.EvaluateInteraction(interactionData);
                        creatureReference.Friendship.InteractionEnter(interactionData);

                        characterReference.LookAnimatorHead.SetLookTarget(creatureReference.TransformFace);
                    }
                    else
                    {
                        Debug.Log($"Creature {focusCurrent} is missing a {creatureReference.Friendship} component. Interaction can not be initiated.");
                    }
                }
            }
        }

        public bool ReceivedDropItemInput()
        {
            switch (CurrentInputMethod())
            {
                case InputMethod.KEYBOARD_MOUSE:
                    return Keyboard.current.fKey.wasPressedThisFrame;

                case InputMethod.GAMEPAD:
                    return Gamepad.current.buttonNorth.wasPressedThisFrame;

                case InputMethod.NONE:
                    return false;
            }
            return false;
        }

        private void ManuallyCallDropItemAction()
        {
            ItemSlotCollection collection = characterReference.Inventory.GetItemCollection("Equipped") as ItemSlotCollection;
            ItemInfo itemInfo = collection.GetItemInfoAtSlot(0);

            if (itemInfo.Item == null)
            {
                return;
            }
            else
            {
                ItemUser itemUser = characterReference.Inventory.GetComponent<ItemUser>();
                dropItemAction = new DropItemAction();
                dropItemAction.Initialize(false);
                dropItemAction.PickUpItemPrefab = itemPickUpPrefab;
                dropItemAction.RemoveOnDrop = true;
                dropItemAction.DropRadius = 0.1f;

                if (dropItemAction.PickUpItemPrefab == null)
                    Debug.LogError("EngageCreature State can not proceed ItemDropAction because ItemPickUpPrefab is null");

                bool canInvoke = dropItemAction.CanInvoke(itemInfo, itemUser);
                if (canInvoke)
                {
                    dropItemAction.InvokeAction(itemInfo, itemUser);

                    InteractionData interactionData = new InteractionData(InteractionData.InteractionMethod.DROP_ITEM, characterReference.MainGameObject, itemInfo, true, dropItemAction.PickUpGameObject);
                    //creatureReference.Friendship.EvaluateInteraction(interactionData);
                    creatureReference.Friendship.InteractionEnter(interactionData);

                    Vector3 pos = characterReference.ModelTransform.position + new Vector3(0, 2, 0) + characterReference.ModelTransform.forward * 2; 
                    dropItemAction.PickUpGameObject.transform.position = pos;
                }
            }            
        }

        public void SetItemPickUpPrefab(GameObject prefab)
        {
            itemPickUpPrefab = prefab;
        }

        private void HandleHandMovement()
        {
            Vector3 move = Vector3.zero;
            float inputHorizontal = 0;
            float inputVertical = 0;

            switch (CurrentInputMethod())
            {
                case InputMethod.KEYBOARD_MOUSE:
                    Keyboard keyboard = Keyboard.current;

                    if (keyboard.aKey.ReadValue() >= deadZoneThreshold)
                        inputHorizontal = -1;
                    else if (keyboard.dKey.ReadValue() >= deadZoneThreshold)
                        inputHorizontal = 1;

                    if (keyboard.wKey.ReadValue() >= deadZoneThreshold)
                        inputVertical = 1;
                    else if (keyboard.sKey.ReadValue() >= deadZoneThreshold)
                        inputVertical = -1;

                    break;

                case InputMethod.GAMEPAD:
                    inputHorizontal = ProcessedInput(Gamepad.current.leftStick.ReadValue().x);
                    inputVertical = ProcessedInput(Gamepad.current.leftStick.ReadValue().y);
                    break;

                case InputMethod.NONE:
                    break;
            }

            move = new Vector3(inputHorizontal, inputVertical, 0);
            float speed = characterReference.HandTargetSpeedInputCurve.Evaluate(move.magnitude);

            Vector3 posNext = characterReference.LookAnimatorHandTarget.localPosition + move * speed;

            if (posNext.x >= characterReference.HandTargetCapX.x || posNext.x <= characterReference.HandTargetCapX.y)
                posNext.x = characterReference.LookAnimatorHandTarget.localPosition.x;

            if (posNext.y >= characterReference.HandTargetCapY.x || posNext.y <= characterReference.HandTargetCapY.y)
                posNext.y = characterReference.LookAnimatorHandTarget.localPosition.y;


            characterReference.LookAnimatorHandTarget.localPosition = posNext;
        }

        private float ProcessedInput(float rawInput)
        {
            float processedInput = rawInput;

            if (Mathf.Abs(processedInput) < deadZoneThreshold)
                processedInput = 0f;

            return processedInput;
        }

        private enum InputMethod
        {
            KEYBOARD_MOUSE,
            GAMEPAD,
            NONE
        }

        private InputMethod CurrentInputMethod()
        {
            Gamepad gamepad = Gamepad.current;

            if (gamepad == null)
            {
                Keyboard keyboard = Keyboard.current;
                Mouse mouse = Mouse.current;

                if (keyboard == null || mouse == null)
                {
                    if (keyboard == null)
                        Debug.LogError("No Keyboard detected for Input.");

                    if (mouse == null)
                        Debug.LogError("No Mouse detected for Input.");

                    return InputMethod.NONE;
                }
                else
                    return InputMethod.KEYBOARD_MOUSE;
            }
            else
            {
                return InputMethod.GAMEPAD;
            }
        }
    }
}