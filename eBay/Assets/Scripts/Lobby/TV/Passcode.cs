using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passcode : MonoBehaviour
{
    public delegate void CALLBACK();
    public CALLBACK onPasswordRevealed = delegate () { };

    [Header("Components")]
    public Passcode_Digit originalDigit;

    public uint passcode { private set; get; }
    Passcode_Digit[] digits;

    public void Set(uint passcode)
    {
        this.passcode = passcode;
        CreateDigits();
    }

    void CreateDigits()
    {
        originalDigit.gameObject.SetActive(false);
        digits.DestroyGameObjectArray();
        string password = passcode.ToString();
        digits = new Passcode_Digit[password.Length];
        for (int i = 0; i < digits.Length; i++)
        {
            Passcode_Digit digit = Instantiate(originalDigit);
            digit.gameObject.SetActive(true);
            digit.transform.SetParent(originalDigit.transform.parent, false);
            digit.Setup(int.Parse(password[i].ToString()));
            digit.Hide();
            digits[i] = digit;
        }
    }

    public void RevealByIndex(uint index)
    {
        if (digits != null)
        {
            bool allRevealed = true;
            string password = passcode.ToString();
            for (int i = 0; i < password.Length; i++)
            {
                if (i < digits.Length)
                {
                    if (i == index) { digits[i].Reveal(); }
                    allRevealed &= digits[i].isRevealed;
                }
            }

            if (allRevealed)
            {
                onPasswordRevealed?.Invoke(); // if (onPasswordRevealed != null) { onPasswordRevealed(); }
            }
        }
    }

    public void RevealByMatch(uint number)
    {
        if (digits != null)
        {
            bool allRevealed = true;
            string password = digits.ToString();
            for (int i = 0; i < password.Length; i++)
            {
                if (i < digits.Length)
                {
                    if (password[i] == number.ToString()[0])
                    {
                        digits[i].Reveal();
                    }
                    allRevealed &= digits[i].isRevealed;
                }
            }

            if (allRevealed)
            {
                onPasswordRevealed?.Invoke(); // if (onPasswordRevealed != null) { onPasswordRevealed(); }
            }
        }
    }

}
