using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameflow_Selfcheckout : MonoBehaviour
{
    [Header("Settings")]
    public bool allowMultipleSameItem = false;

    [Header("Components")]
    public Bill bill;
    public SettingsScreen settingScreen;

    public delegate void CALLBACK();
    public delegate void ITEMCALLBACK(Item item);
    public ITEMCALLBACK OnItemAdded = delegate (Item item) { };

    public void Setup()
    {
        bill.Clear();
    }

    public void SettingButtonPressed()
    {
        settingScreen.Open();
    }

    public void RFID_Scan(string ID)
    {
        Item item = ItemDatabase.GetItem(ID);
        if (item != null && item.appearsOnCheckout)
        {
            bill.AddItem(item, allowMultipleSameItem ? uint.MaxValue : 1);
            OnItemAdded(item);
        }
    }

}
