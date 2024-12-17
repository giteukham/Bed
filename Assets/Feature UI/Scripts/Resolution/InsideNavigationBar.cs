
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InsideNavigationBar : MonoBehaviour, IPointerClickHandler
{
    private ResolutionManagement resolutionManagement;
    private GameObject navigationBar;
    private Image navBarImage;

    private float insideMaxWidth;

    private void Awake()
    {
        resolutionManagement = ResolutionManagement.Instance;
        navigationBar = this.gameObject;
        navBarImage = navigationBar.GetComponent<Image>();
        
        insideMaxWidth = resolutionManagement.PreviewMaxLength;
    }

    private void OnEnable()
    {
        SetNavigationBarActive(resolutionManagement.IsWindowedScreenReady);
        resolutionManagement.SubscribeToInsideSizeChangedEvent(OnInsideSizeChanged);
    }
    
    private void OnDisable()
    {
        resolutionManagement.UnsubscribeFromInsideSizeChangedEvent(OnInsideSizeChanged);
    }

    private void OnInsideSizeChanged(RectTransform insideRect)
    {
        float normalizedWidth = Mathf.InverseLerp(insideMaxWidth, 0f, insideRect.rect.width);
        //ResizePixelPerUnit(normalizedWidth);
    }

    private void ResizePixelPerUnit(float normalizedWidth)
    {
        navBarImage.pixelsPerUnitMultiplier = Mathf.Lerp(5f, 100f, normalizedWidth);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        resolutionManagement.DoZoom(eventData);
    }

    public void SetNavigationBarActive(bool active)
    {
        navigationBar.SetActive(active);
    }
}
