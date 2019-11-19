using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bill : MonoBehaviour
{
    [Header("Settings")]
    public bool displayBalanceDue = true;

    [Header("Components")]
    public Bill_ItemRow originalItemRow;
    [Header("Header")]
    public Transform headerRow;
    [Header("Balance Due")]
    public Transform balanceDueRow;
    public Text balanceDue_valueText;

    List<Bill_ItemRow> itemRows = new List<Bill_ItemRow>();

    public void Refresh()
    {
        uint total = 0;
        if (itemRows != null && itemRows.Count > 0)
        {
            for (int i = 0; i < itemRows.Count; i++)
            {
                itemRows[i].Refresh();
                total += itemRows[i].GetTotalPrice();
            }
        }
        balanceDueRow.gameObject.SetActive(displayBalanceDue);
        balanceDueRow.transform.SetAsLastSibling();
        balanceDue_valueText.text = "$" + total + ".00";
        headerRow.transform.SetAsFirstSibling();
    }

    public void Clear()
    {
        originalItemRow.gameObject.SetActive(false);
        if (itemRows != null && itemRows.Count > 0)
        {
            Bill_ItemRow[] rows = itemRows.ToArray();
            rows.DestroyGameObjectArray();
            itemRows.Clear();
        }
        Refresh();
    }

    public void AddItem(Item item, uint maxAmount = uint.MaxValue)
    {
        bool isNewItem = true; // just if you need to use it for future logic
        if (itemRows != null && itemRows.Count > 0)
        {
            for (int i = 0; i < itemRows.Count; i++)
            {
                if (itemRows[i].item.tagID == item.tagID)
                {
                    isNewItem = false;
                    if (itemRows[i].amount < maxAmount)
                    {
                        itemRows[i].amount++;
                    }
                    Refresh();
                    return;
                }
            }
        }
        CreateItemRow(item);
    }

    void CreateItemRow(Item item)
    {
        originalItemRow.gameObject.SetActive(false);
        Bill_ItemRow row = Instantiate(originalItemRow);
        row.gameObject.SetActive(true);
        row.transform.SetParent(originalItemRow.transform.parent, false);
        row.Set(item);
        itemRows.Add(row);
        Refresh();
    }

}
