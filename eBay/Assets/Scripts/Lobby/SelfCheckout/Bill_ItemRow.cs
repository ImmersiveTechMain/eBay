﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bill_ItemRow : MonoBehaviour
{
    public Image background;
    public Text title;
    public Text price;
    public uint amount = 1;

    public Item item { private set; get; }

    public uint GetTotalPrice()
    {
        if (item != null)
        {
            return item.price * amount;
        }
        return 0;
    }

    public void Set(Item item)
    {
        this.item = item;
        Refresh();
    }

    public void Refresh()
    {
        title.text = "x" + amount.ToString() + " " + item.name;
        price.text = "$" + GetTotalPrice() + ".00";
    }
}
