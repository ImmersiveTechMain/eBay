using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCell : MonoBehaviour
{
    [Header("Settings")]
    public float revealDuration = 1;
    public Color bgColorWhenHidden = Color.gray;

    [Header("Components")]
    public Image background;
    public Image unknownIcon;
    public Image icon;
    public Text hint;

    public Item item { private set; get; }
    Coroutine revealCoroutine = null;

    public bool revealed { private set; get; }
    public bool hasBeenSetup { private set; get; }

    public void Setup(Item item)
    {
        this.item = item;
        if (item != null)
        {
            if (!hasBeenSetup) { icon.material = Instantiate(icon.material); }
            icon.material.SetFloat("_Transparency", 1);
            icon.sprite = item.icon;
            hasBeenSetup = true;
            hint.text = item.silloute_hint;
        }
        HideItem();
    }

    public void Reveal()
    {
        if (item == null || revealed) { return; }
        if (revealCoroutine != null) { StopCoroutine(revealCoroutine); revealCoroutine = null; }
        unknownIcon.enabled = false;
        revealed = true;
        Color initialIconColor = icon.color;
        Color initialBackgroundColor = background.color;
        revealCoroutine = this.InterpolateCoroutine(revealDuration, (n) =>
        {
            float N = n * n;
            icon.material.SetFloat("_Transparency", 1-n);
            //icon.color = Color.Lerp(initialIconColor, Color.white, N);
            background.color = Color.Lerp(initialBackgroundColor, item.backgroundColor, N);
        });
    }

    public void HideItem()
    {
        revealed = false;
        if (revealCoroutine != null) { StopCoroutine(revealCoroutine); revealCoroutine = null; }
       // icon.color = Color.black;
        icon.material.SetFloat("_Transparency", 1);
        background.color = bgColorWhenHidden;
        unknownIcon.enabled = true;
    }
}
