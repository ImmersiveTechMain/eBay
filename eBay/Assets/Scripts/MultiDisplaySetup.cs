using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiDisplaySetup : MonoBehaviour
{
    public int displaysSupported = 5;
    public static bool displaysSetupCompleted = false;
    public bool forceMaximizeScreen = false;

    private void Awake()
    {
        if (!displaysSetupCompleted)
        {
            if (forceMaximizeScreen) { Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; }

            if (Display.displays != null && Display.displays.Length > 1)
            {
                int min = Mathf.Min(displaysSupported, Display.displays.Length);
                for (int i = 1; i < min; i++) { Display.displays[i].Activate(); }
            }
            displaysSetupCompleted = true;
        }
    }
}
