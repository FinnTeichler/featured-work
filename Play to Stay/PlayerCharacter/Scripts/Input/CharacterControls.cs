using SpiritGarden.Character;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FinnTeichler.CMFExtension
{
    public class CharacterControls : CMF.CharacterInput
    {
        [HideInInspector] public CharacterReference refCharacter;

        [SerializeField] private float deadZoneThreshold = 0.1f;

        public override float GetHorizontalMovementInput()
        {
            Vector2 move = Vector2.zero;
            float inputHorizontal = 0;

            switch (CurrentInputMethod())
            {
                case InputMethod.KEYBOARD_MOUSE:

                    Keyboard keyboard = Keyboard.current;

                    if (keyboard.aKey.ReadValue() >= deadZoneThreshold)
                        inputHorizontal = -1;
                    else if (keyboard.dKey.ReadValue() >= deadZoneThreshold)
                        inputHorizontal = 1;

                    return inputHorizontal;

                case InputMethod.GAMEPAD:
                    return ProcessedInput(Gamepad.current.leftStick.ReadValue().x);

                case InputMethod.NONE:
                    return 0f;
            }
            return 0f;
        }

        public override float GetVerticalMovementInput()
        {
            Vector2 move = Vector2.zero;
            float inputVertical = 0;

            switch (CurrentInputMethod())
            {
                case InputMethod.KEYBOARD_MOUSE:

                    Keyboard keyboard = Keyboard.current;

                    if (keyboard.wKey.ReadValue() >= deadZoneThreshold)
                        inputVertical = 1;
                    else if (keyboard.sKey.ReadValue() >= deadZoneThreshold)
                        inputVertical = -1;

                    return inputVertical;

                case InputMethod.GAMEPAD:
                    return ProcessedInput(Gamepad.current.leftStick.ReadValue().y);

                case InputMethod.NONE:
                    return 0f;
            }
            return 0f;
        }

        public override bool IsJumpKeyPressed()
        {
            if (refCharacter.refGame.GamePause.IsPaused)
                return false;

            switch (CurrentInputMethod())
            {
                case InputMethod.KEYBOARD_MOUSE:
                    return Keyboard.current.spaceKey.wasPressedThisFrame;

                case InputMethod.GAMEPAD:
                    return Gamepad.current.buttonSouth.wasPressedThisFrame;

                case InputMethod.NONE:
                    return false;
            }
            return false;
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