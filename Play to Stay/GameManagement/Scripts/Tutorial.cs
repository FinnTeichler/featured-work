using FinnTeichler.Event.Bus;
using FinnTeichler.UI;
using Opsive.UltimateInventorySystem.Core.DataStructures;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using SpiritGarden;
using SpiritGarden.Character;
using SpiritGarden.Creature;
using SpiritGarden.Creature.Behaviour;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tutorial : MonoBehaviour
{
    [HideInInspector] public GameReference refGame;

    [SerializeField] private float cooldownDuration = 3;
    [Space(5)]
    [SerializeField] private GameObject textCamera;
    [SerializeField] private GameObject textCharacterMove;
    [SerializeField] private GameObject textJump;
    [SerializeField] private GameObject textInteractItem;
    [SerializeField] private GameObject textEquipItem;
    [SerializeField] private GameObject textInteractCreature;
    [SerializeField] private GameObject textInteractCreatureDrop;

    private List<GameObject> texts = new List<GameObject>();

    private EventBinding<EngageCreatureCharacterState.OnCreatureFocusEnter> onFocusEnterBinding;
    private EventBinding<EngageCreatureCharacterState.OnCreatureFocusExit> onFocusExitBinding;

    private int tutorialStepIndex = 0;
    private float cooldown = 0f;
    private bool cooldownActive = false;

    private bool hasCreatureFocus = false;
    private bool hasCreatureFearFocus = false;

    private void OnEnable()
    {
        onFocusEnterBinding = new EventBinding<EngageCreatureCharacterState.OnCreatureFocusEnter>(HandleOnFocusEnter);
        EventBus<EngageCreatureCharacterState.OnCreatureFocusEnter>.Register(onFocusEnterBinding);

        onFocusExitBinding = new EventBinding<EngageCreatureCharacterState.OnCreatureFocusExit>(HandleOnFocusExit);
        EventBus<EngageCreatureCharacterState.OnCreatureFocusExit>.Register(onFocusExitBinding);
    }

    private void OnDisable()
    {
        EventBus<EngageCreatureCharacterState.OnCreatureFocusEnter>.Deregister(onFocusEnterBinding);
        EventBus<EngageCreatureCharacterState.OnCreatureFocusExit>.Deregister(onFocusExitBinding);
    }

    private void Start()
    {
        texts.Add(textCamera);
        texts.Add(textCharacterMove);
        texts.Add(textJump);
        texts.Add(textInteractItem);
        texts.Add(textEquipItem);
        texts.Add(textInteractCreature);
        texts.Add(textInteractCreatureDrop);

        tutorialStepIndex = -1;
        cooldown = cooldownDuration;
        cooldownActive = true;
    }

    private void Update()
    {
        if (cooldownActive)
            cooldown -= Time.deltaTime;

        if (cooldown <= 0) 
        {
            cooldownActive = false;
            cooldown = cooldownDuration;

            tutorialStepIndex++;

            if (tutorialStepIndex < texts.Count - 1)
                DisplayTutorialText(texts[tutorialStepIndex]);
        }

        if (HasReceivedThisInput((TutorialInput)tutorialStepIndex))
        {
            texts[tutorialStepIndex].SetActive(false);
            cooldownActive = true;

            if (tutorialStepIndex >= texts.Count - 1)
            {
                this.enabled = false;
            }
        }
    }

    private void DisplayTutorialText(GameObject textObject)
    {
        textObject.SetActive(true);

        switch (CurrentInputMethod())
        {
            case InputMethod.KEYBOARD_MOUSE:
                textObject.transform.GetChild(1).gameObject.SetActive(true);
                textObject.transform.GetChild(2).gameObject.SetActive(false);
                break;
            case InputMethod.GAMEPAD:
                textObject.transform.GetChild(1).gameObject.SetActive(false);
                textObject.transform.GetChild(2).gameObject.SetActive(true);
                break;
            case InputMethod.NONE:
                Debug.LogError("Tutorial can not display text for the correct input method, because no input method was chosen.");
                break;
        }
    }

    private bool HasReceivedThisInput(TutorialInput input)
    {
        switch (input)
        {
            case TutorialInput.MOVE_CAMERA:
                return ReceivedInputMoveCamera();
            case TutorialInput.MOVE_CHARACTER:
                return ReceivedInputMoveCharacter();
            case TutorialInput.JUMP:
                return ReceivedInputJump();
            case TutorialInput.INTERACT_ITEM:
                return ReceivedInputInteractItem();
            case TutorialInput.EQUIP:
                return ReceivedInputEquipItem();
            case TutorialInput.INTERACT_CREATURE:
                return ReceivedInputInteractCreature();
            case TutorialInput.INTERACT_DROP:
                return ReceivedInputInteractDrop();
        }

        return false;
    }

    private bool ReceivedInputMoveCamera()
    {
        if (Mathf.Abs(refGame.RefCharacter.CameraControls.GetHorizontalCameraInput()) > 0.5)
            return true;
        else
            return false;
    }

    private bool ReceivedInputMoveCharacter()
    {
        if (Mathf.Abs(refGame.RefCharacter.CharacterControls.GetVerticalMovementInput()) > 0.5)
            return true;
        else
            return false;
    }

    private bool ReceivedInputJump()
    {
        if (refGame.RefCharacter.CharacterControls.IsJumpKeyPressed())
            return true;
        else
            return false;
    }

    private bool ReceivedInputInteractItem()
    {
        bool receivedInput = false;
        switch (CurrentInputMethod())
        {
            case InputMethod.KEYBOARD_MOUSE:
                if (Keyboard.current.fKey.wasPressedThisFrame)
                    receivedInput = true;
                break;
            case InputMethod.GAMEPAD:
                if (Gamepad.current.buttonNorth.wasPressedThisFrame)
                    receivedInput = true;
                break;
            case InputMethod.NONE:
                return false;
        }

        if (receivedInput)
        {
            GameObject nearestItem = refGame.RefCharacter.ItemSensor.GetNearest();

            if (nearestItem)
            {
                if (nearestItem.CompareTag("ItemPickUp"))
                {
                    return true;
                }
                else
                    Debug.Log(nearestItem.tag.ToString());
            }
        }

        return false;
    }

    private bool ReceivedInputEquipItem()
    {
        ItemSlotCollection collection = refGame.RefCharacter.Inventory.GetItemCollection("Equipped") as ItemSlotCollection;
        ItemInfo itemInfo = collection.GetItemInfoAtSlot(0);

        if (itemInfo.Item == null)
            return false;
        else
            return true;
    }

    private bool ReceivedInputInteractCreature()
    {
        if (hasCreatureFocus)
        {
            switch (CurrentInputMethod())
            {
                case InputMethod.KEYBOARD_MOUSE:
                    return Keyboard.current.ctrlKey.wasPressedThisFrame;

                case InputMethod.GAMEPAD:
                    return Gamepad.current.buttonWest.wasPressedThisFrame;

                case InputMethod.NONE:
                    return false;
            }
        }
        return false;
    }

    private bool ReceivedInputInteractDrop()
    {
        if (hasCreatureFearFocus)
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
        }

        return false;
    }

    private enum TutorialInput
    {
        MOVE_CAMERA,
        MOVE_CHARACTER,
        JUMP,
        INTERACT_ITEM,
        EQUIP,
        INTERACT_CREATURE,
        INTERACT_DROP
    }

    private void HandleOnFocusEnter(EngageCreatureCharacterState.OnCreatureFocusEnter onCreatureFocusEnter)
    {
        if (onCreatureFocusEnter.creatureObject.GetComponent<CreatureReference>().BehaviourManager.BehaviourCurrent is CB_Fearful)
        {
            hasCreatureFearFocus = true;
            DisplayTutorialText(texts[texts.Count - 1]);
        }
        else
            hasCreatureFocus = true;

    }

    private void HandleOnFocusExit(EngageCreatureCharacterState.OnCreatureFocusExit onCreatureFocusExit)
    {
        hasCreatureFearFocus = false;
        hasCreatureFocus = false;

        texts[texts.Count - 1].SetActive(false);
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