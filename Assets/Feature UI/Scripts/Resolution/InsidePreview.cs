
using System;
using System.Collections.Generic;
using Bed.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Cursor = Bed.UI.Cursor;

public enum ResizeType
{
    Left,
    Right,
    Up,
    Down,
    LeftUp,
    LeftDown,
    RightUp,
    RightDown
}

[Serializable]
public class ResizeBounds
{
    public GameObject left, right, up, down;
    public GameObject leftUp, leftDown, rightUp, rightDown;
}

public class InsidePreview : MonoBehaviour, IDragHandler
{
    private ResolutionManagement resolutionManager;
    
    [Header("드래그 가능한 영역")]
    [SerializeField] private ResizeBounds resizeBounds;
    
    [Header("기타 설정")]
    [SerializeField] private Material previewMaskMaterial;
    
    private Dictionary<ResizeType, ResizeEvent> resizeEvents = new Dictionary<ResizeType, ResizeEvent>();
    private ResizeType currentResizeType;
    
    private Vector2 insideOffsetMin, insideOffsetMax, outsideOffsetMin, outsideOffsetMax;
    
    private const float edgeDraggableThickness = 200f;
    
    private void OnEnable()
    {
        resolutionManager = ResolutionManagement.Instance;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.delta;
        Vector2 insideAnchoredPosition = resolutionManager.InsideAnchoredPosition;
        Vector2 insideSize = resolutionManager.InsideSize;
        Vector2 outsideSize = resolutionManager.OutsideSize;
        
        float distanceX = (outsideSize.x - insideSize.x) * 0.5f;
        float distanceY = (outsideSize.y - insideSize.y) * 0.5f;
        
        insideAnchoredPosition = new Vector2(
            Mathf.Clamp(insideAnchoredPosition.x + delta.x, -distanceX, distanceX),
            Mathf.Clamp(insideAnchoredPosition.y + delta.y, -distanceY, distanceY));
        
        resolutionManager.InsideAnchoredPosition = insideAnchoredPosition;
    }
    
    private void Awake()
    {
        AddAllResizeEvent();
    }

    private void AddAllResizeEvent()
    {
        foreach (ResizeType type in Enum.GetValues(typeof(ResizeType)))
        {
            GameObject resizeBound = null;
            switch (type)
            {
                case ResizeType.Left:
                    resizeBound = resizeBounds.left;
                    break;
                case ResizeType.Right:
                    resizeBound = resizeBounds.right;
                    break;
                case ResizeType.Up:
                    resizeBound = resizeBounds.up;
                    break;
                case ResizeType.Down:
                    resizeBound = resizeBounds.down;
                    break;
                case ResizeType.LeftUp:
                    resizeBound = resizeBounds.leftUp;
                    break;
                case ResizeType.LeftDown:
                    resizeBound = resizeBounds.leftDown;
                    break;
                case ResizeType.RightUp:
                    resizeBound = resizeBounds.rightUp;
                    break;
                case ResizeType.RightDown:
                    resizeBound = resizeBounds.rightDown;
                    break;
            }
            
            ResizeEvent resizeEvent = CreateResizeEvent(resizeBound, type);

            if (resizeBound != null)
            {
                resizeEvents.TryAdd(type, resizeEvent);
            }
        }
    }
    
    private ResizeEvent CreateResizeEvent(GameObject resizeBound, ResizeType type)
    {
        ResizeEvent resizeEvent = resizeBound.AddComponent<ResizeEvent>();

        resizeEvent.ResizeType = type;
        resizeEvent.OnResizeStart = OnResizeStart;
        resizeEvent.OnResizeEnd = OnResizeEnd;
        resizeEvent.OnResizeStay = OnResizeStay;
        resizeEvent.OnPointerEnterEvent = OnPointerEnterEvent;
        resizeEvent.OnPointerExitEvent = OnPointerExitEvent;

        return resizeEvent;
    }

    private void OnResizeStart(ResizeType type)
    {
        currentResizeType = type;
    }
    
    private void OnResizeStay(PointerEventData eventData)
    {
        insideOffsetMax = resolutionManager.InsideOffsetMax;
        insideOffsetMin = resolutionManager.InsideOffsetMin;
        outsideOffsetMax = resolutionManager.OutsideOffsetMax;
        outsideOffsetMin = resolutionManager.OutsideOffsetMin;
        
        Vector2 delta = eventData.delta;
        ChangeCursor(currentResizeType);
        
        switch (currentResizeType)
        {

            case ResizeType.Left:
                insideOffsetMin.x = Mathf.Clamp(insideOffsetMin.x + delta.x, outsideOffsetMin.x, insideOffsetMax.x - edgeDraggableThickness);
                break;
            case ResizeType.Right:
                insideOffsetMax.x = Mathf.Clamp(insideOffsetMax.x + delta.x, insideOffsetMin.x + edgeDraggableThickness, outsideOffsetMax.x);
                break;
            case ResizeType.Up:
                insideOffsetMax.y = Mathf.Clamp(insideOffsetMax.y + delta.y, insideOffsetMin.y + edgeDraggableThickness, outsideOffsetMax.y);
                break;
            case ResizeType.Down:
                insideOffsetMin.y = Mathf.Clamp(insideOffsetMin.y + delta.y, outsideOffsetMin.y, insideOffsetMax.y - edgeDraggableThickness);
                break;
            case ResizeType.LeftDown:
                insideOffsetMin.x = Mathf.Clamp(insideOffsetMin.x + delta.x, outsideOffsetMin.x, insideOffsetMax.x - edgeDraggableThickness);
                insideOffsetMin.y = Mathf.Clamp(insideOffsetMin.y + delta.y, outsideOffsetMin.y, insideOffsetMax.y - edgeDraggableThickness);
                break;
            case ResizeType.LeftUp:
                insideOffsetMin.x = Mathf.Clamp(insideOffsetMin.x + delta.x, outsideOffsetMin.x, insideOffsetMax.x - edgeDraggableThickness);
                insideOffsetMax.y = Mathf.Clamp(insideOffsetMax.y + delta.y, insideOffsetMin.y + edgeDraggableThickness, outsideOffsetMax.y);
                break;
            case ResizeType.RightDown:
                insideOffsetMax.x = Mathf.Clamp(insideOffsetMax.x + delta.x, insideOffsetMin.x + edgeDraggableThickness, outsideOffsetMax.x);
                insideOffsetMin.y = Mathf.Clamp(insideOffsetMin.y + delta.y, outsideOffsetMin.y, insideOffsetMax.y - edgeDraggableThickness);
                break;
            case ResizeType.RightUp:
                insideOffsetMax.x = Mathf.Clamp(insideOffsetMax.x + delta.x, insideOffsetMin.x + edgeDraggableThickness, outsideOffsetMax.x);
                insideOffsetMax.y = Mathf.Clamp(insideOffsetMax.y + delta.y, insideOffsetMin.y + edgeDraggableThickness, outsideOffsetMax.y);
                break;
        }
        
        previewMaskMaterial.SetVector("_insideOffsetMin", insideOffsetMin);
        previewMaskMaterial.SetVector("_insideOffsetMax", insideOffsetMax);
        previewMaskMaterial.SetVector("_insideSize", resolutionManager.InsideSize);
        
        resolutionManager.InsideOffsetMin = insideOffsetMin;
        resolutionManager.InsideOffsetMax = insideOffsetMax;
    }
    
    private void OnResizeEnd()
    {
        Cursor.SetCursor(CursorType.Normal);
    }
    
    private void OnPointerEnterEvent(ResizeType type)
    {
        ChangeCursor(type);
    }
    
    private void OnPointerExitEvent()
    {
        Cursor.SetCursor(CursorType.Normal);
    }
    
    private void ChangeCursor(ResizeType type)
    {
        switch (type)
        {
            case ResizeType.Left:
            case ResizeType.Right:
                Cursor.SetCursor(CursorType.HorizontalResize);
                break;
            case ResizeType.Up:
            case ResizeType.Down:
                Cursor.SetCursor(CursorType.VerticalResize);
                break;
            case ResizeType.LeftUp:
            case ResizeType.RightDown:
                Cursor.SetCursor(CursorType.DiagonalResize1);
                break;
            case ResizeType.LeftDown:
            case ResizeType.RightUp:
                Cursor.SetCursor(CursorType.DiagonalResize2);
                break;
        }
    }
}
