using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameflow_Selfcheckout : MonoBehaviour
{
    [Header("Settings")]
    public bool allowMultipleSameItem = false;
    public float itemNotRecognizedPopupDuration = 4;

    [Header("Audio")]
    public AudioClip SFX_CorrectScan;
    public AudioClip SFX_IncorrectScan;

    [Header("Components")]
    public Bill bill;
    public SettingsScreen settingScreen;
    public GameObject itemNotRecognizedScreen;

    public delegate void CALLBACK();
    public delegate void ITEMCALLBACK(Item item);
    public ITEMCALLBACK OnItemAdded = delegate (Item item) { };

    public void Setup()
    {
        bill.Clear();
        SetItemNotRecognizedScreenVisualState(false);
    }

    public void SettingButtonPressed()
    {
        settingScreen.Open();
    }

    public void SetItemNotRecognizedScreenVisualState(bool isVisible)
    {
        itemNotRecognizedScreen.SetActive(isVisible);
    }

    Coroutine autoHide = null;


    string lastMessage = null;
    public void RFID_Scan(string ID)
    {
        Item item = ItemDatabase.GetItem(ID);
        bool valid = false;
        if (item != null)
        {
            if (item.appearsOnCheckout)
            {
                bill.AddItem(item, allowMultipleSameItem ? uint.MaxValue : 1);
                OnItemAdded(item);
                SetItemNotRecognizedScreenVisualState(false);
                valid = true;
                if (lastMessage != ID) { Audio.PlaySFX(SFX_CorrectScan); }
            }
        }

        if (!valid)
        {
            if (lastMessage != ID) { Audio.PlaySFX(SFX_IncorrectScan); }
            SetItemNotRecognizedScreenVisualState(true);
            if (autoHide != null) { StopCoroutine(autoHide); autoHide = null; }
            autoHide = this.ActionAfterSecondDelay(itemNotRecognizedPopupDuration, () => SetItemNotRecognizedScreenVisualState(false));
        }

        lastMessage = ID;
    }

}
