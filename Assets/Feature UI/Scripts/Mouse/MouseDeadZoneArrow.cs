
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseDeadZoneArrow : MonoBehaviour, IDragHandler
{
    private MouseSettings mouseSettings;
    [SerializeField] private MouseDeadZoneUI mouseDeadZoneUI;
    private RectTransform arrowTransform;
    private Vector2 arrowPos = Vector2.zero;
    
    private float minPosX, maxPosX;
    
    public static event Action<float> OnArrowDrag;

    private void OnEnable()
    {
        mouseSettings = MouseSettings.Instance;
        TryGetComponent(out arrowTransform);
        
        MouseDeadZoneUI.OnDeadZoneOffsetChange += ChangeArrowPosition;

        float temp = mouseDeadZoneUI.BackgroundTransform.rect.width - 5f;
        minPosX = (-temp) * mouseSettings.DeadZoneLimit + (temp / 2);
        maxPosX = temp * 0.5f;
        ChangeArrowPosition();
    }
    
    private void ChangeArrowPosition()
    {
        float normalValue = Mathf.InverseLerp(0f, mouseSettings.DeadZoneLimit, mouseSettings.DeadZoneSliderValue);
        arrowPos.x = Mathf.Lerp(maxPosX, minPosX, normalValue);
        arrowPos.y = arrowTransform.anchoredPosition.y;
        arrowTransform.anchoredPosition = arrowPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        arrowPos.x = arrowTransform.anchoredPosition.x + eventData.delta.x;
        arrowPos.x = Mathf.Clamp(arrowPos.x, minPosX, maxPosX);
        arrowPos.y = arrowTransform.anchoredPosition.y;
        arrowTransform.anchoredPosition = arrowPos;
        
        float normalValue = Mathf.InverseLerp(maxPosX, minPosX, arrowPos.x);
        OnArrowDrag?.Invoke(Mathf.Lerp(0f, mouseSettings.DeadZoneLimit, normalValue));
    }
}
