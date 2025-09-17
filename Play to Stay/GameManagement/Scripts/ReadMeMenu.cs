using SpiritGarden;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ReadMeMenu : MonoBehaviour
{
    [HideInInspector] public UIReference refUI;

    [SerializeField] private bool openOnStart;
    [SerializeField] private List<GameObject> panels = new();

    private int currentIndex = 0;

    public Action OnOpen;
    public Action OnClose;

    public bool IsOpen { get { return isOpen; } }
    private bool isOpen;

    private void Start()
    {
        if (openOnStart)
            Open();
    }

    public void Open()
    {
        refUI.refGame.GamePause.Pause();
        isOpen = true;

        currentIndex = 0;
        panels[currentIndex].SetActive(true);
        EventSystem.current.SetSelectedGameObject(panels[currentIndex].GetComponentInChildren<Button>().gameObject);

        OnOpen?.Invoke();
    }

    public void Continue()
    {
        panels[currentIndex].SetActive(false);
        currentIndex++;

        if (currentIndex < panels.Count)
        {
            panels[currentIndex].SetActive(true);
            EventSystem.current.SetSelectedGameObject(panels[currentIndex].GetComponentInChildren<Button>().gameObject);
        }
        else
        {
            currentIndex--;
            Close();
        }
    }

    public void Close()
    {
        panels[currentIndex].SetActive(false);
        currentIndex = 0;

        isOpen = false;
        refUI.refGame.GamePause.Resume();

        OnClose?.Invoke();
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