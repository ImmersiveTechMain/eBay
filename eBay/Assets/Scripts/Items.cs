using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Item
{
    public string name;
    public string hint;
    public string silloute_hint;
    public string[] tagID;
    public uint price;
    public Category category;
    public Sprite icon;
    public Color backgroundColor = Color.gray;
    public bool isOnSilluetePuzzle; // object that will appear on silluete hide and seek game.
    public bool isListedOnTV; // object that will be listed on the category list.
    public bool appearsOnCheckout; // object that will be listed on the touchscreen self checkout.
    public bool isKey; // object marked to be key for a puzzle to progress.

    public enum Category
    {
        Electronics,
        Home,
        Toys
    }
}
