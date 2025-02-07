using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MouseWheelHoverEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IScrollHandler
{
    public UnityEvent onScrollUp, onScrollDown;
    private bool isHovering = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (!isHovering) return;
        
        if (eventData.scrollDelta.y > 0) onScrollUp?.Invoke();
        if (eventData.scrollDelta.y < 0) onScrollDown?.Invoke();
    }
}
