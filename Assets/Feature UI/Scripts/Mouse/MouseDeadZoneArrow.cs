
using System;
using System.Windows.Forms.VisualStyles;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MouseDeadZoneArrow : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private MouseSettings mouseSettings;
    
    private RectTransform arrowTransform;
    private RectTransform backgroundTransform;
    private Vector2 arrowPos = Vector2.zero;
    private Image arrowImage;
    
    private float minPosX, maxPosX;

    private bool isDraggable = false;
    
    public event Action<float> OnArrowDrag;

    public void Init(RectTransform backgroundTransform, Color initColor)
    {
        mouseSettings = MouseSettings.Instance;
        
        this.backgroundTransform = backgroundTransform;
        arrowTransform = GetComponent<RectTransform>();
        arrowImage = GetComponent<Image>();
        arrowImage.color = initColor;
        
        float width = backgroundTransform.rect.width;
        minPosX = (-width) * mouseSettings.DeadZoneLimit + (width / 2) + 10;
        maxPosX = width * 0.5f + 10;
    }
    
    public void ChangeArrowColor(Color color)
    {
        arrowImage.DOColor(color, 0.5f);
    }

    public void ChangeArrowPositionWithLerp(float x)
    {
        var value = Mathf.Lerp(maxPosX, minPosX, x);
        ChangeArrowWidthPosition(value);
    }

    public void ChangeArrowWidthPosition(float x)
    {
        arrowPos.x = x;
        arrowPos.y = arrowTransform.anchoredPosition.y;
        arrowTransform.anchoredPosition = arrowPos;
        
        var normalValue = Mathf.InverseLerp(maxPosX, minPosX, arrowPos.x);
        OnArrowDrag?.Invoke(Mathf.Lerp(0f, mouseSettings.DeadZoneLimit, normalValue));
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isDraggable = RectTransformUtility.RectangleContainsScreenPoint(backgroundTransform, eventData.position);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundTransform, eventData.position, eventData.pressEventCamera, out var localPoint);

        var percent = localPoint.x / backgroundTransform.rect.width;
        var deltaX = percent >= 1f - mouseSettings.DeadZoneLimit && percent <= 1f ? eventData.delta.x : 0f;
        
        var x = Mathf.Clamp(arrowTransform.anchoredPosition.x + deltaX, minPosX, maxPosX);
        ChangeArrowWidthPosition(x);
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        isDraggable = false;
    }
}
