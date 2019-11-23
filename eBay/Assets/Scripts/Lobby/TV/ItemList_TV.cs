using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ItemList_TV : MonoBehaviour
{
    public delegate void CALLBACK();
    public CALLBACK OnRowCheckmarkChanged = delegate () { };
    public Text title;
    public Image titleIcon;
    public Image titlePattern;
    public ItemList_TV_Row originalRow;

    public Item.Category category { private set; get; }

    ItemList_TV_Row[] rows;
    Item[] items;

    public void Setup(Item[] items, ItemListGroup.ListSetting settings, int itemCount = -1)
    {
        itemCount = items == null ? 0 : itemCount < 0 ? items.Length : Mathf.Min(itemCount, items.Length);
        this.category = settings.itemsCategory;
        this.items = items.Where((item, index) => { return index < itemCount; }).ToArray();
        this.title.text = settings.title;
        this.titlePattern.sprite = settings.patern;
        this.titlePattern.enabled = this.titlePattern.sprite != null;
        this.titleIcon.sprite = settings.icon;
        CreateRows();
    }

    void CreateRows()
    {
        rows.DestroyGameObjectArray();
        originalRow.gameObject.SetActive(false);
        if (items != null)
        {
            rows = new ItemList_TV_Row[items.Length];
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = Instantiate(originalRow);
                rows[i].gameObject.SetActive(true);
                rows[i].transform.SetParent(originalRow.transform.parent, false);
                int _i = i;
                rows[i].ActionAfterFrameDelay(1, () => { rows[_i].Setup(items[_i]); });
                
            }
        }
    }

    public ItemList_TV_Row GetItemRow(string id)
    {
        if (rows != null)
        {
            for (int i = 0; i < rows.Length; i++)
            {
                for (int t = 0; t < rows[i].item.tagID.Length; t++)
                {
                    if (rows[i].item.tagID[t] == id) { return rows[i]; }
                }
            }
        }
        return null;
    }

    public void CheckmarkItem(Item item, bool checkmark)
    {
        ItemList_TV_Row row = GetItemRow(item.tagID == null || item.tagID.Length <= 0 ? "null" : item.tagID[0]);
        if (row != null) { row.SetCheckmarkVisualState(checkmark); OnRowCheckmarkChanged(); }        
    }

    public bool AreAllItemsChecked()
    {
        if (rows != null)
        {
            bool allChecked = true;
            for (int i = 0; i < rows.Length; i++)
            {
                allChecked &= rows[i].isChecked;
            }
            return allChecked;
        }
        return false;
    }
}
