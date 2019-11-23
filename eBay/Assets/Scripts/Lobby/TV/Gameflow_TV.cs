using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Gameflow_TV : MonoBehaviour
{
    [Header("Settings")]
    public string[] passcodeRFID_ID_Index;

    [Header("Components")]
    public ItemListGroup listsGroup;
    public Passcode passcode;
    public GameObject lastItemScanned_WrongOrder_Screen;
    public Image lastItemIcon;

    public delegate void CALLBACK();
    public CALLBACK OnAllItemsChecked = delegate () { };

    public bool allItemsScanned { private set; get; }
    internal bool usingPasscodeNumbers = false;

    public void Setup(uint passcode, int itemCount = -1)
    {
        listsGroup.Setup(itemCount);
        listsGroup.OnAllItemsChecked = () => { if (!allItemsScanned) { OnAllItemsChecked(); } allItemsScanned = true; };
        allItemsScanned = false;
        SetLastItemIncorrectOrderScreenVisibleState(false,null);
        if (usingPasscodeNumbers)
        {
            this.passcode.Set(passcode);
        }
        this.passcode.gameObject.SetActive(false);
    }

    public void SetLastItemIncorrectOrderScreenVisibleState(bool isVisible, Item lastItem)
    {
        lastItemScanned_WrongOrder_Screen.SetActive(isVisible);
        lastItemIcon.sprite = lastItem == null ? null : lastItem.icon;
    }

    public void RFID_Scan(string ID)
    {
        Item item = ItemDatabase.GetItem(ID);
        if (item != null)
        {
            ItemList_TV list = listsGroup.GetList(item.category);
            Debug.Log("Here ");
            if (list != null)
            {
                list.CheckmarkItem(item, true);
            }
        }
        else
        {
            if (passcodeRFID_ID_Index != null && usingPasscodeNumbers)
            {
                for (uint i = 0; i < passcodeRFID_ID_Index.Length; i++)
                {
                    if (passcodeRFID_ID_Index[i] == ID)
                    {
                        if (!passcode.gameObject.activeInHierarchy) { passcode.gameObject.SetActive(true); }
                        passcode.RevealByIndex(i);
                    }
                }
            }
        }
    }

}
