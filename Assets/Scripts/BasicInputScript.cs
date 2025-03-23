using System;
using UnityEngine;
using static GameSetting;
using static UnityEngine.InputSystem.InputAction;

public class BasicInputScript : MonoBehaviour
{

    public static WASDInput Input;

    void Awake()
    {
        Input = new WASDInput();
        Input.Enable();
    }

    private void ScreenShot(CallbackContext obj)
    {
        ScreenShot();
    }

    private void ScreenShot()
    {
        if (!Application.isFocused) return;
        int hour = DateTime.Now.Hour;
        int minute = DateTime.Now.Minute;
        int second = DateTime.Now.Second;
        int year = DateTime.Now.Year;
        int month = DateTime.Now.Month;
        int day = DateTime.Now.Day;

        ScreenCapture.CaptureScreenshot(string.Format("{6}/{0}{1}{2}{3}{4}{5}.png", year, month, day, hour, minute, second, Environment.CurrentDirectory));
    }

    private void OnDestroy()
    {
        Input.Disable();
    }

    public static bool Refresh { get; set; } = true;
}
