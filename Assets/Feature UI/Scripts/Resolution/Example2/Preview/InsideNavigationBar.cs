
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class InsideNavigationBar : MonoBehaviour, IPointerClickHandler
{
    public OnZoomEvent onZoom = new OnZoomEvent();

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2) onZoom?.Invoke();
    }

    public void SetNavigationBarActive(bool active)
    {
        gameObject?.SetActive(active);
    }
}
