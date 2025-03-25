
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InsideNavigationBar : MonoBehaviour
{
    private Image navigationBarImage;

    public void Initialize()
    {
        navigationBarImage = GetComponent<Image>();
    }

    public void SetNavigationBarActive(bool active)
    {
        gameObject?.SetActive(active);
    }
    
    public void ChangeNavigationBarPixelMultiplier(float pixelMultiplier)
    {
        navigationBarImage.pixelsPerUnitMultiplier = pixelMultiplier;
    }
}
