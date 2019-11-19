using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class SettingsScreen : MonoBehaviour
{
    public GameObject screen;
    [Header("Timer Settings")]
    public ToggleGroup timerSettings;
    public Toggle[] timerToggles;
    [Header("Item Count Settings")]
    public ToggleGroup itemCountSettings;
    public Toggle[] itemCountToggles;

    public static TimerOptions timerOption = TimerOptions.MIN_15;
    public static ItemCountOptions itemCountOptions = ItemCountOptions.Twelve;

    public enum TimerOptions
    {
        MIN_10,
        MIN_15
    }

    public static float GetSettings_TimerDuration()
    {
        switch (timerOption)
        {
            default:
            case TimerOptions.MIN_10: return 600;
            case TimerOptions.MIN_15: return 900;
        }
    }


    public enum ItemCountOptions
    {
        Three,
        Six,
        Nine,
        Twelve
    }

    public static int GetSettings_ItemCount()
    {
        switch (itemCountOptions)
        {
            default:
            case ItemCountOptions.Twelve: return 12;
            case ItemCountOptions.Nine: return 9;
            case ItemCountOptions.Six: return 6;
            case ItemCountOptions.Three: return 3;
        }
    }

    void Awake()
    {
        Hide();
        GAME.OnGameStarted += Close;
    }

    public void Open()
    {
        if (GAME.gameHasEnded || !GAME.gameHasStarted)
        {
            screen.gameObject.SetActive(true);
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.selectedObject = timerToggles[(int)timerOption].gameObject;
            timerToggles[(int)timerOption].OnPointerClick(pointer);
            pointer.selectedObject = itemCountToggles[(int)itemCountOptions].gameObject;
            itemCountToggles[(int)itemCountOptions].OnPointerClick(pointer);
        }
        else { Hide(); }
    }

    public void OnItemCountSettingToggleSelected(bool wasPressed)
    {
        if (wasPressed)
        {
            int index = 0;
            for (int i = 0; i < itemCountToggles.Length; i++)
            {
                if (itemCountToggles[i].isOn)
                {
                    index = i;
                }
            }
            itemCountOptions = (ItemCountOptions)index;
            ApplyUserSettings();
        }
    }

    public void OnTimerSettingToggleSelected(bool wasPressed)
    {
        if (wasPressed)
        {
            int index = 0;
            for (int i = 0; i < timerToggles.Length; i++)
            {
                if (timerToggles[i].isOn)
                {
                    index = i;
                }
            }
            timerOption = (TimerOptions)index;
            ApplyUserSettings();
        }
    }

    public static void ApplyUserSettings()
    {
        GAME.SetGameDuration(GetSettings_TimerDuration());
    }

    public void Close() { Hide(); }
    public void Hide()
    {
        screen.gameObject.SetActive(false);
    }

}
