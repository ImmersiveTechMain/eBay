using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGrid : MonoBehaviour
{
    [Header("Settings")]
    public int capacity = 4;
    [Header("Components")]
    public ItemCell originalCell;
    public RectTransform originalRow;
    public RectTransform rowsFolder;
    public Text title;

    ItemCell[] itemCells;
    RectTransform[] rows;

    public bool hasBeenSetup { private set; get; }

    public void Setup(Item[] items, Item.Category category)
    {
        CreateRows(items);
        CreateCells(items);
        title.text = category.ToString().ToUpper();
        hasBeenSetup = true;
    }

    void CreateRows(Item[] items)
    {
        rows.DestroyGameObjectArray();
        originalCell.gameObject.SetActive(false);
        originalRow.gameObject.SetActive(false);
        if (items != null && items.Length > 0)
        {
            int totalAmount = items.Length == 2 ? items.Length : Mathf.CeilToInt(items.Length / 2f);
            rows = new RectTransform[totalAmount];
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = Instantiate(originalRow);
                rows[i].gameObject.SetActive(true);
                rows[i].SetParent(originalRow.parent, false);
            }
        }
    }

    void CreateCells(Item[] items)
    {
        itemCells.DestroyGameObjectArray();
        originalCell.gameObject.SetActive(false);
        if (rows == null) { CreateRows(items); }
        if (items != null && items.Length > 0)
        {
            int totalAmount = Mathf.Min(items.Length, capacity);
            itemCells = new ItemCell[totalAmount];
            for (int i = 0; i < itemCells.Length; i++)
            {
                int rowIndex = items.Length == 2 ? i : Mathf.FloorToInt(i / 2f);
                ItemCell itemCell = Instantiate(originalCell);
                itemCell.gameObject.SetActive(true);
                itemCell.transform.SetParent(rows[rowIndex].transform, false);
                itemCell.Setup(items[i]);
                itemCell.HideItem();
                itemCells[i] = itemCell;
            }
        }
    }

    public bool Reveal(string id)
    {
        if (itemCells != null)
        {
            for (int i = 0; i < itemCells.Length; i++)
            {
                for (int t = 0; t < itemCells[i].item.tagID.Length; t++)
                {
                    if (itemCells[i].item.tagID[t] == id)
                    {
                        itemCells[i].Reveal();
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
