using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ItemListGroup : MonoBehaviour
{
    [System.Serializable]
    public class ListSetting
    {
        public string title;
        public Item.Category itemsCategory;
        public Sprite icon;
        public Sprite patern;
    }

    public delegate void CALLBACK();
    public CALLBACK OnAllItemsChecked = delegate () { };


    [Header("Settings")]
    public ListSetting[] listSettings;

    [Header("Components")]
    public ItemList_TV originalItemListTV;

    ItemList_TV[] lists;

    public void Setup(int itemCount = -1)
    {
        CeateList(itemCount);
    }

    void CeateList(int itemCount)
    {
        int totalAmountOfItemsToUse = itemCount < 0 ? ItemDatabase.Count : Mathf.Clamp(itemCount, 0, ItemDatabase.Count);
        float _itemsPerList = totalAmountOfItemsToUse / (float)listSettings.Length; // evenDistribution;
        int itemsPerList = Mathf.FloorToInt(_itemsPerList);
        int remaining = Mathf.RoundToInt((_itemsPerList - itemsPerList) * listSettings.Length);
        
        lists.DestroyGameObjectArray();
        originalItemListTV.gameObject.SetActive(false);
        if (listSettings != null)
        {
            lists = new ItemList_TV[listSettings.Length];
            for (int i = 0; i < lists.Length; i++)
            {
                int itemCountForThisGrid = itemsPerList + (remaining-- > 0 ? 1 : 0);

                Item[] items = ItemDatabase.AllItems.Where((item) => { return item.category == listSettings[i].itemsCategory; }).ToArray();
                int nextPackageSize = Mathf.Min(itemCountForThisGrid, items == null ? 0 : items.Length);
                lists[i] = Instantiate(originalItemListTV);
                lists[i].gameObject.name = "[" + listSettings[i].itemsCategory.ToString() + "] List";
                lists[i].gameObject.SetActive(true);
                lists[i].transform.SetParent(originalItemListTV.transform.parent, false);
                int _i = i;
                lists[i].Setup(items, listSettings[_i], nextPackageSize);
                lists[i].OnRowCheckmarkChanged = CheckCompletion;
            }
        }
    }

    public void CheckCompletion()
    {
        if (lists != null)
        {
            bool allcompleted = true;
            for (int i = 0; i < lists.Length; i++)
            {
                allcompleted &= lists[i].AreAllItemsChecked();
            }
            if (allcompleted) { OnAllItemsChecked(); }
        }
    }

    public ItemList_TV GetList(Item.Category category)
    {
        if (lists != null)
        {
            for (int i = 0; i < lists.Length; i++)
            {
                Debug.Log(i);
                if (lists[i].category == category)
                {
                    return lists[i];
                }
            }
        }
        return null;
    }
}
