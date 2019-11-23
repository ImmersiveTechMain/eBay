using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemList_TV_Row : MonoBehaviour
{
    [Header("Components")]
    public Text label;
    public GameObject checkmark;
    public GameObject checkedBackground;

    [Header("Settings")]
    public float text_RightSpacingWhenChecked = 200;
    public float text_RightSpacingWhenUnchecked = 10;
    public Color fontColor_Checked = Color.white;
    public Color fontColor_unChecked = Color.black;


    public Item item { private set; get; }
    public bool isChecked { get { return this.checkmark.gameObject.activeInHierarchy; } }

    public void Setup(Item item)
    {
        gameObject.name = "ItemRow_" + item.name;
        this.item = item;
        label.text = "\"" + item.hint + "\"";
        SetCheckmarkVisualState(false);
    }

    public void SetCheckmarkVisualState(bool visible)
    {
        this.checkmark.gameObject.SetActive(visible);
        Vector2 size = label.rectTransform.sizeDelta;
        size.x = (label.rectTransform.parent as RectTransform).rect.width - (visible ? text_RightSpacingWhenChecked : text_RightSpacingWhenUnchecked);
        label.rectTransform.sizeDelta = size;
        label.color = visible ? fontColor_Checked : fontColor_unChecked;
        checkedBackground.SetActive(visible);
    }
}
