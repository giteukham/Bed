
using System;
using System.Collections.Generic;
using Bed.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
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

public class Resize : MonoBehaviour
{
    [Header("드래그 가능한 영역")]
    [SerializeField] private ResizeBounds resizeBounds;
    
    [Header("마우스 커서")]
    
    
    private Dictionary<ResizeType, ResizeEvent> resizeEvents = new Dictionary<ResizeType, ResizeEvent>();
    private ResizeType currentResizeType;
    
    private ResolutionManagement resolutionManager;
    private Vector2 insideOffsetMin, insideOffsetMax, outsideOffsetMin, outsideOffsetMax;
    
    private Action<Vector2> cardinalEvent, diagonalEvent;
    
    private const float edgeDraggableThickness = 300f;

    private void Awake()
    {
        AddAllResizeEvent();
    }

    private void OnEnable()
    {
        resolutionManager = ResolutionManagement.Instance;
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
        
        switch (type)
        {
            case ResizeType.Left:
            case ResizeType.Right:
            case ResizeType.Up:
            case ResizeType.Down:
                DragCardinal(type);
                break;
            case ResizeType.LeftUp:
            case ResizeType.LeftDown:
            case ResizeType.RightUp:
            case ResizeType.RightDown:
                DragDiagonal(type);
                break;
        }
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
        
        resolutionManager.InsideOffsetMin = insideOffsetMin;
        resolutionManager.InsideOffsetMax = insideOffsetMax;
    }
    
    private void OnResizeEnd()
    {
        Cursor.SetCursor(CursorType.Normal);
        cardinalEvent = null;
        diagonalEvent = null;
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
    
    private void DragDiagonal(ResizeType type)
    {
        switch (type)
        {
            case ResizeType.LeftUp:
                diagonalEvent += (delta) => resolutionManager.InsideOffsetMin = 
                    new Vector2(resolutionManager.InsideOffsetMin.x + delta.x, resolutionManager.InsideOffsetMin.y + delta.y);
                break;
            case ResizeType.LeftDown:
                diagonalEvent += (delta) => resolutionManager.InsideOffsetMin = 
                    new Vector2(resolutionManager.InsideOffsetMin.x + delta.x, resolutionManager.InsideOffsetMin.y - delta.y);
                break;
            case ResizeType.RightUp:
                diagonalEvent += (delta) => resolutionManager.InsideOffsetMax = 
                    new Vector2(resolutionManager.InsideOffsetMax.x + delta.x, resolutionManager.InsideOffsetMax.y + delta.y);
                break;
            case ResizeType.RightDown:
                diagonalEvent += (delta) => resolutionManager.InsideOffsetMax = 
                    new Vector2(resolutionManager.InsideOffsetMax.x + delta.x, resolutionManager.InsideOffsetMax.y - delta.y);
                break;
        }
    }
    
    private void DragCardinal(ResizeType type)
    {
        switch (type)
        {
            case ResizeType.Left:
                cardinalEvent += (delta) => resolutionManager.InsideOffsetMin = 
                    new Vector2(resolutionManager.InsideOffsetMin.x + delta.x, resolutionManager.InsideOffsetMin.y);
                break;
            case ResizeType.Right:
                cardinalEvent += (delta) => resolutionManager.InsideOffsetMax = 
                    new Vector2(resolutionManager.InsideOffsetMax.x + delta.x, resolutionManager.InsideOffsetMax.y);
                break;
            case ResizeType.Up:
                cardinalEvent += (delta) => resolutionManager.InsideOffsetMax = 
                    new Vector2(resolutionManager.InsideOffsetMax.x, resolutionManager.InsideOffsetMax.y + delta.y);
                break;
            case ResizeType.Down:
                cardinalEvent += (delta) => resolutionManager.InsideOffsetMin = 
                    new Vector2(resolutionManager.InsideOffsetMin.x, resolutionManager.InsideOffsetMin.y + delta.y);
                break;
        }
    }
}
