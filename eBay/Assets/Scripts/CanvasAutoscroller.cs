using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasAutoscroller : MonoBehaviour
{
    [Header("Settings")]
    public float timerPerSlide = 10;
    [Header("Components")]
    public Canvas[] canvases;
    public RawImage rawImage;
    public RenderTexture[] textures;
    public Image[] progressionDots;

    Coroutine slideCoroutine = null;
    int currentIndex = 0;

    private void OnEnable()
    {
        if (slideCoroutine != null) { StopCoroutine(slideCoroutine); slideCoroutine = null; }
        currentIndex = 0;
        SetView(currentIndex);
        slideCoroutine = this.LoopCoroutine(timerPerSlide, (n) => { }, () => 
        {
            currentIndex = (int)Mathf.Repeat(currentIndex + 1, textures.Length);
            SetView(currentIndex);
        });
    }

    public void SetView(int index)
    {
        if (textures != null && index < textures.Length && index >= 0)
        {
            rawImage.texture = textures[index];

            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].targetDisplay = i == index ? 0 : 1;
            }

            for (int i = 0; i < progressionDots.Length; i++)
            {
                progressionDots[i].gameObject.SetActive(i == index);
            }
        }
    }

    public void Restore()
    {
        for (int i = 0; i < canvases.Length; i++)
        {
            canvases[i].targetDisplay = i;
        }        
    }

    private void OnDisable()
    {
        if (slideCoroutine != null) { StopCoroutine(slideCoroutine); slideCoroutine = null; }
        Restore();
    }
}
