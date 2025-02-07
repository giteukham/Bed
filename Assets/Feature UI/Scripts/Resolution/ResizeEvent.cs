
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResizeEvent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    private ResizeType resizeType;
    
    public ResizeType ResizeType
    {
        get => resizeType;
        set => resizeType = value;
    }
    
    public Action<ResizeType, PointerEventData> OnResizeStart;
    public Action OnResizeEnd;
    public Action<PointerEventData> OnResizeStay;
    public Action<ResizeType> OnPointerEnterEvent;
    public Action OnPointerExitEvent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnResizeStart?.Invoke(resizeType, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnResizeStay?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnResizeEnd?.Invoke();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent?.Invoke(resizeType);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent?.Invoke();
    }
}
