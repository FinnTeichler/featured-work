using UnityEngine;
using UnityEngine.InputSystem;

namespace FinnTeichler.CMFExtension
{
    public class CameraControls : CMF.CameraInput
    {
        [SerializeField] private bool invertHorizontalInput = false;
        [SerializeField] private bool invertVerticalInput = false;

        [SerializeField] private float deadZoneThreshold = 0.2f;
        [SerializeField] private float mouseInputMultiplier = 0.01f;

        public override float GetHorizontalCameraInput()
        {
            switch (CurrentInputMethod())
            {
                case InputMethod.KEYBOARD_MOUSE:
                    return ProcessedInputMouse(Mouse.current.delta.ReadValue().x, invertHorizontalInput);

                case InputMethod.GAMEPAD:
                    return ProcessedInputGamepad(Gamepad.current.rightStick.ReadValue().x, invertHorizontalInput); ;

                case InputMethod.NONE:
                    return 0f;
            }
            return 0f;
        }

        public override float GetVerticalCameraInput()
        {
            switch (CurrentInputMethod())
            {
                case InputMethod.KEYBOARD_MOUSE:
                    return ProcessedInputMouse(Mouse.current.delta.ReadValue().y, invertVerticalInput);

                case InputMethod.GAMEPAD:
                    return ProcessedInputGamepad(Gamepad.current.rightStick.ReadValue().y, invertVerticalInput);

                case InputMethod.NONE:
                    return 0f;
            }
            return 0f;
        }

        private float ProcessedInputGamepad(float rawInput, bool invert)
        {
            float processedInput = rawInput;

            if (Mathf.Abs(processedInput) < deadZoneThreshold)
                processedInput = 0f;

            if (invert)
                processedInput = -processedInput;

            return processedInput;
        }

        private float ProcessedInputMouse(float rawInput, bool invert)
        {
            float processedInput = rawInput;

            //Since raw mouse input is already time-based, we need to correct for this before passing the input to the camera controller;
            if (Time.timeScale > 0f && Time.deltaTime > 0f)
            {
                processedInput /= Time.deltaTime;
                processedInput *= Time.timeScale;
            }
            else
                processedInput = 0f;

            //Apply mouse sensitivity;
            processedInput *= mouseInputMultiplier;

            if (Mathf.Abs(processedInput) < deadZoneThreshold)
                processedInput = 0f;

            if (invert)
                processedInput = -processedInput;

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