using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    [Header("Settings")]
    public int capacity = 4;
    [Header("Components")]
    public ItemCell originalCell;

    ItemCell[] itemCells;

    public bool hasBeenSetup { private set; get; }

    public void Setup(Item[] items, Item.Category category)
    {
        CreateCells(items);
        hasBeenSetup = true;
    }

    void CreateCells(Item[] items)
    {
        itemCells.DestroyGameObjectArray();
        originalCell.gameObject.SetActive(false);
        if (items != null && items.Length > 0)
        {
            int totalAmount = Mathf.Min(items.Length, capacity);
            itemCells = new ItemCell[totalAmount];
            for (int i = 0; i < itemCells.Length; i++)
            {
                ItemCell itemCell = Instantiate(originalCell);
                itemCell.gameObject.SetActive(true);
                itemCell.transform.SetParent(originalCell.transform.parent, false);
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
                if (itemCells[i].item.tagID == id)
                {
                    itemCells[i].Reveal();
                    return true;
                }
            }
        }
        return false;
    }
}
