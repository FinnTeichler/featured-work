using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using SpiritGarden;

public class GameMenu : MonoBehaviour
{
    [HideInInspector] public UIReference refUI;

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject quitConfirmPanel;
    [SerializeField] private GameObject buttonContinue;
    [SerializeField] private GameObject buttonCancelQuit;
    private Canvas activeMenu;

    private void Update()
    {
        if (IsMenuKeyPressed() && refUI.IntroIsClosed)
        {
            if (activeMenu == null)
            {
                activeMenu = refUI.CanvasGameMenu;
                refUI.SetMenu(activeMenu, true);
                EventSystem.current.SetSelectedGameObject(buttonContinue);
            }
            else
            {
                if (quitConfirmPanel.activeSelf)
                    quitConfirmPanel.SetActive(false);

                if (activeMenu == refUI.CanvasInventory)
                    refUI.InventoryDisplayPanel.Close();

                if (activeMenu == refUI.CanvasInstructions)
                    refUI.GameInstructionsMenu.Close();

                refUI.SetMenu(activeMenu, false);
                activeMenu = null;
                EventSystem.current.SetSelectedGameObject(null);
            }           
        }
    }

    private bool IsMenuKeyPressed()
    {
        switch (CurrentInputMethod())
        {
            case InputMethod.KEYBOARD_MOUSE:
                return Keyboard.current.escapeKey.wasPressedThisFrame;

            case InputMethod.GAMEPAD:
                return Gamepad.current.selectButton.wasPressedThisFrame;

            case InputMethod.NONE:
                return false;
        }
        return false;
    }

    public void Continue()
    {
        refUI.SetMenu(refUI.CanvasGameMenu, false);
        activeMenu = null;
    }

    public void OpenInventory()
    {
        activeMenu = refUI.CanvasInventory;
        refUI.InventoryDisplayPanel.Open();
        refUI.SetMenu(refUI.CanvasGameMenu, false);
        refUI.SetMenu(refUI.CanvasInventory, true);
    }

    public void OpenTutorial()
    {
        activeMenu = refUI.CanvasInstructions;
        refUI.GameInstructionsMenu.Open();
        refUI.SetMenu(refUI.CanvasGameMenu, false);
        refUI.SetMenu(refUI.CanvasInstructions,true);
    }

    public void OpenQuitConfirmation()
    {
        activeMenu = refUI.CanvasGameMenu;
        quitConfirmPanel.SetActive(true);
        menuPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(buttonCancelQuit);
    }

    public void CloseQuitConfirmation()
    {
        activeMenu = null;
        quitConfirmPanel.SetActive(false);
        menuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(buttonContinue);
    }

    public void Quit()
    {
        Application.Quit();
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