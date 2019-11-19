using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Passcode_Digit : MonoBehaviour
{
    [Header("Settings")]
    public string messageWhenHidden = "?";
    public Color hiddenColor;
    public Color revealedColor;

    [Header("Components")]
    public Text digit;
    public Image background;

    public int value { private set; get; }
    public bool isRevealed = false;

    public void Setup(int value)
    {
        this.value = value;
        Refresh();
    }

    void Refresh()
    {
        digit.text = isRevealed ? value.ToString() : messageWhenHidden;
        digit.color = isRevealed ? revealedColor : hiddenColor;
    }

    public void Reveal()
    {
        isRevealed = true;
        Refresh();
    }

    public void Hide()
    {
        isRevealed = false;
        Refresh();
    }
}
