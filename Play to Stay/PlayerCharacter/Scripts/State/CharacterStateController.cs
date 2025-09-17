using UnityEngine;
using UnityEngine.InputSystem;
using FinnTeichler.StateMachine.Old;
using System;
using SensorToolkit;
using FIMSpace.FLook;
using SpiritGarden;

namespace SpiritGarden.Character
{
    [RequireComponent(typeof(CharacterReference))]
    public class CharacterStateController : MonoBehaviour
    {
        [SerializeField] private bool debugMode;

        [SerializeField] private Transform floatingUICanvas;
        [SerializeField] private GameObject floatingUIPrefab;
        [SerializeField] private GameObject itemPickUpPrefab;
        [SerializeField] private Camera gameCamera;
        [SerializeField] private Transform handTarget;

        private StateMachineV1 stateMachine = new();
        private MovementCharacterState stateMovement;
        private EngageCreatureCharacterState stateEngageCreature;
        private TriggerSensor creatureSensor;
        private FLookAnimator lookAnimator;

        [HideInInspector] public CharacterReference characterReference;

        private CMF.CharacterInput characterInput;

        public event Action OnStateMovement;
        public event Action OnStateEngageCreature;

        void Start()
        {
            characterReference = GetComponent<CharacterReference>();
            stateMovement = new MovementCharacterState(characterReference);
            stateEngageCreature = new EngageCreatureCharacterState(characterReference);
            stateEngageCreature.SetItemPickUpPrefab(itemPickUpPrefab);

            stateMachine.ChangeState(stateMovement);
        }

        void Update()
        {
            stateMachine.Update();

            if (characterReference.CMFMover.IsGrounded())
            {
                switch (CurrentInputMethod())
                {
                    case InputMethod.KEYBOARD_MOUSE:

                        Keyboard keyboard = Keyboard.current;
                        if (keyboard.ctrlKey.wasPressedThisFrame)
                        {
                            ToggleEngageCreatureState();
                            break;
                        }
                        else
                            break;

                    case InputMethod.GAMEPAD:

                        Gamepad gamepad = Gamepad.current;
                        if (gamepad.buttonWest.wasPressedThisFrame)
                        {
                            ToggleEngageCreatureState();
                            break;
                        }
                        else
                            break;

                    case InputMethod.NONE:
                        break;
                }
            }
        }

        private void ToggleEngageCreatureState()
        {
            if (stateMachine.CurrentState.Equals(stateMovement))
            {
                stateMachine.ChangeState(stateEngageCreature);

                OnStateEngageCreature.Invoke();

                if (debugMode)
                    Debug.Log("Character State: Movement");
            }
            else if (stateMachine.CurrentState.Equals(stateEngageCreature))
            {
                stateMachine.ChangeState(stateMovement);
                OnStateMovement.Invoke();

                if (debugMode)
                    Debug.Log("Character State: EngageCreature");
            }
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