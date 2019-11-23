using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public Item[] items;

    static Item[] _AllItems;
    public static Item[] AllItems
    {
        get
        {
            if (_AllItems == null)
            {
                ItemDatabase database = FindObjectOfType<ItemDatabase>();
                _AllItems = database == null ? null : database.items;
            }
            return _AllItems;
        }
    }

    public static int Count { get { return AllItems == null ? 0 : AllItems.Length; } }

    public static Item GetItem(string id)
    {
        if (AllItems != null)
        {
            for (int i = 0; i < AllItems.Length; i++)
            {
                for (int t = 0; t < AllItems[i].tagID.Length; t++)
                {
                    if (AllItems[i].tagID[t] == id)
                    {
                        return AllItems[i];
                    }
                }
            }
        }
        return null;
    }
    
}
