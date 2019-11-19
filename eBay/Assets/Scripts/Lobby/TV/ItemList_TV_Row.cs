using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemList_TV_Row : MonoBehaviour
{
    public Text label;
    public Image checkmark;

    public Item item { private set; get; }
    public bool isChecked { get { return this.checkmark.gameObject.activeInHierarchy; } }
    
    public void Setup(Item item)
    {
        this.item = item;
        label.text = item.hint;
        SetCheckmarkVisualState(false);
    }

    public void SetCheckmarkVisualState(bool visible)
    {
        this.checkmark.gameObject.SetActive(visible);
    }
}
