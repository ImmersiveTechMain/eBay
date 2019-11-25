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
    public Transform totalSavingsRow;

    [Header("Header")]
    public Transform headerRow;
    [Header("Balance Due")]
    public Transform balanceDueRow;
    public Text balanceDue_valueText;

    List<Bill_ItemRow> itemRows = new List<Bill_ItemRow>();

    public void Refresh()
    {
        float total = 0;
        if (itemRows != null && itemRows.Count > 0)
        {
            for (int i = 0; i < itemRows.Count; i++)
            {
                itemRows[i].Refresh();
                total += itemRows[i].GetTotalPrice();
            }
        }
        if (totalSavingsRow.gameObject.activeInHierarchy) { totalSavingsRow.SetAsLastSibling(); }
        balanceDueRow.gameObject.SetActive(displayBalanceDue);
        balanceDueRow.transform.SetAsLastSibling();
        balanceDue_valueText.text = "$" + total;
        headerRow.transform.SetAsFirstSibling();
    }

    public void Clear()
    {
        totalSavingsRow.gameObject.SetActive(false);
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
                for (int t = 0; t < itemRows[i].item.tagID.Length; t++)
                {
                    for (int it = 0; it < item.tagID.Length; it++)
                    {
                        if (itemRows[i].item.tagID[t] == item.tagID[it])
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
