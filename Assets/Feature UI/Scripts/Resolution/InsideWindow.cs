
using System;
using System.Collections.Generic;
using Bed.UI;
using JetBrains.Annotations;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
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

/// <summary>
/// TODO:
/// 위에 바 더블클릭하면 최대화
/// Apply 누르면 적용
/// 마우스 포인터 이미 적용되어 있으면 다른 거로 안 바뀌게
/// </summary>
public class InsideWindow : MonoBehaviour, IDragHandler
{
    private ResolutionManagement resolutionManager;
    private RectTransform insideRect;
    private RectTransform outsideRect;
    
    [Header("Inside 내부 UI")]
    
    [SerializeField]
    [Tooltip("드래그 가능한 영역")]
    private ResizeBounds resizeBounds;
    
    [SerializeField]
    [Tooltip("상단 바")]
    private GameObject navigationBar;
    
    [Header("기타 설정")]
    [SerializeField] private Material previewMaskMaterial;
    [SerializeField] private InsideWindowZoom insideWindowZoom;
    
    private Dictionary<ResizeType, ResizeEvent> resizeEvents = new Dictionary<ResizeType, ResizeEvent>();
    private ResizeType currentResizeType;
    
    private Vector2 insideOffsetMin, insideOffsetMax, outsideOffsetMin, outsideOffsetMax;
    private Vector2 prevOffsetMin, prevOffsetMax;

    private Vector2 insideLowestSize;

    private readonly float aspectRatio = 1.777778f;

    public UnityEvent<RectTransform> OnRectTransformReSize;

    private void OnEnable()
    {
        resolutionManager = ResolutionManagement.Instance;
        resolutionManager.OnFullScreenSwitched.AddListener(FullScreenSwitchHandler);
        insideRect = resolutionManager.InsideRectTransform;
        outsideRect = resolutionManager.OutsideRectTransform;
        
        Vector2Int insideLowestResolution = resolutionManager.GetLowestResolution();
        insideLowestSize = resolutionManager.ConvertResolutionToSize(insideLowestResolution);
        
        FullScreenSwitchHandler(resolutionManager.IsFullScreen);
    }

    private void OnDisable()
    {
        resolutionManager.OnFullScreenSwitched.RemoveListener(FullScreenSwitchHandler);
    }

    private void OnRectTransformDimensionsChange()
    {
        OnRectTransformReSize?.Invoke(insideRect);
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

        return resizeEvent;
    }

    private void SubscribeAllResizeEvent()
    {
        foreach (ResizeType type in Enum.GetValues(typeof(ResizeType)))
        {
            if (resizeEvents.TryGetValue(type, out var resizeEvent))
            {
                resizeEvent.OnResizeStart += OnResizeStart;
                resizeEvent.OnResizeEnd += OnResizeEnd;
                resizeEvent.OnResizeStay += OnResizeStay;
                resizeEvent.OnPointerEnterEvent += OnPointerEnterEvent;
                resizeEvent.OnPointerExitEvent += OnPointerExitEvent;
            }
        }
    }
    
    private void UnsubscribeAllResizeEvent()
    {
        foreach (ResizeType type in Enum.GetValues(typeof(ResizeType)))
        {
            if (resizeEvents.TryGetValue(type, out var resizeEvent))
            {
                resizeEvent.OnResizeStart -= OnResizeStart;
                resizeEvent.OnResizeEnd -= OnResizeEnd;
                resizeEvent.OnResizeStay -= OnResizeStay;
                resizeEvent.OnPointerEnterEvent -= OnPointerEnterEvent;
                resizeEvent.OnPointerExitEvent -= OnPointerExitEvent;
            }
        }
    }
    
    private void OnResizeStart(ResizeType type, PointerEventData eventData)
    {
        currentResizeType = type;
        insideWindowZoom.SetZoomState(ZoomState.ZoomIn);
        prevOffsetMin = resolutionManager.InsideOffsetMin;
        prevOffsetMax = resolutionManager.InsideOffsetMax;
    }
    
    private void OnResizeStay(PointerEventData eventData)
    {
        ChangeCursor(currentResizeType);
        insideOffsetMin = resolutionManager.InsideOffsetMin;
        insideOffsetMax = resolutionManager.InsideOffsetMax;
        outsideOffsetMax = resolutionManager.OutsideOffsetMax;
        outsideOffsetMin = resolutionManager.OutsideOffsetMin;
        
        Vector2 localPoint = outsideRect.InverseTransformPoint(eventData.position);
        
        switch (currentResizeType)
        {
            case ResizeType.Left:
            case ResizeType.LeftUp:
                float leftOffsetMinX = Mathf.Clamp(localPoint.x, outsideOffsetMin.x, insideOffsetMax.x - insideLowestSize.x);
                float leftOffsetMaxY = Mathf.Clamp(
                    prevOffsetMax.y - (leftOffsetMinX - prevOffsetMin.x) / aspectRatio,
                    outsideOffsetMin.y,
                    outsideOffsetMax.y
                );
                
                // Y축이 최대값을 넘어가면 X축 늘어나는 거 방지
                if (leftOffsetMaxY >= outsideOffsetMax.y)
                {
                    leftOffsetMaxY = outsideOffsetMax.y;
                    leftOffsetMinX = prevOffsetMin.x + (prevOffsetMax.y - leftOffsetMaxY) * aspectRatio;
                }
                
                insideOffsetMin.x = leftOffsetMinX;
                insideOffsetMax.y = leftOffsetMaxY;
                break;
            
            case ResizeType.Right:
            case ResizeType.RightUp:
                float rightOffsetMaxX = Mathf.Clamp(localPoint.x, insideOffsetMin.x + insideLowestSize.x, outsideOffsetMax.x);
                float rightOffsetMaxY = Mathf.Clamp(
                    prevOffsetMax.y + (rightOffsetMaxX - prevOffsetMax.x) / aspectRatio,
                    outsideOffsetMin.y,
                    outsideOffsetMax.y
                );
                
                if (rightOffsetMaxY >= outsideOffsetMax.y)
                {
                    rightOffsetMaxY = outsideOffsetMax.y;
                    rightOffsetMaxX = prevOffsetMax.x + (rightOffsetMaxY - prevOffsetMax.y) * aspectRatio;
                }
                
                insideOffsetMax.x = rightOffsetMaxX;
                insideOffsetMax.y = rightOffsetMaxY;
                break;
            
            case ResizeType.Up:
                float upOffsetMaxY = Mathf.Clamp(localPoint.y, insideOffsetMin.y + insideLowestSize.y, outsideOffsetMax.y);
                float upOffsetMaxX = Mathf.Clamp(
                    prevOffsetMax.x + (upOffsetMaxY - prevOffsetMax.y) * aspectRatio,
                    insideOffsetMin.x + insideLowestSize.x,
                    outsideOffsetMax.x
                );
                
                if (upOffsetMaxX >= outsideOffsetMax.x)
                {
                    upOffsetMaxX = outsideOffsetMax.x;
                    upOffsetMaxY = prevOffsetMax.y + (upOffsetMaxX - prevOffsetMax.x) / aspectRatio;
                }
                
                insideOffsetMax.y = upOffsetMaxY;
                insideOffsetMax.x = upOffsetMaxX;
                break;
            
            case ResizeType.Down:
            case ResizeType.LeftDown:
                float downOffsetMinY = Mathf.Clamp(localPoint.y, outsideOffsetMin.y, insideOffsetMax.y - insideLowestSize.y);
                float downOffsetMinX = Mathf.Clamp(
                    prevOffsetMin.x + (downOffsetMinY - prevOffsetMin.y) * aspectRatio,
                    outsideOffsetMin.x,
                    insideOffsetMax.x - insideLowestSize.x
                );
                
                if (downOffsetMinX <= outsideOffsetMin.x)
                {
                    downOffsetMinX = outsideOffsetMin.x;
                    downOffsetMinY = prevOffsetMin.y + (downOffsetMinX - prevOffsetMin.x) / aspectRatio;
                }
                
                insideOffsetMin.y = downOffsetMinY;
                insideOffsetMin.x = downOffsetMinX;
                break;
            
            case ResizeType.RightDown:
                float rightDownOffsetMinY = Mathf.Clamp(localPoint.y, outsideOffsetMin.y, insideOffsetMax.y - insideLowestSize.y);
                float rightDownOffsetMaxX = Mathf.Clamp(
                    prevOffsetMax.x - (rightDownOffsetMinY - prevOffsetMin.y) * aspectRatio,
                    insideOffsetMin.x + insideLowestSize.x,
                    outsideOffsetMax.x
                );
                
                if (rightDownOffsetMaxX >= outsideOffsetMax.x)
                {
                    rightDownOffsetMaxX = outsideOffsetMax.x;
                    rightDownOffsetMinY = prevOffsetMin.y + (prevOffsetMax.x - rightDownOffsetMaxX) / aspectRatio;
                }

                insideOffsetMin.y = rightDownOffsetMinY;
                insideOffsetMax.x = rightDownOffsetMaxX;
                break;
        }

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
    
    /// <summary>
    /// Preview 창을 드래그할 때 창을 움직일 수 있게 하는 함수
    /// </summary>
    /// <param name="eventData"></param>
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

    private void FullScreenSwitchHandler(bool isFullScreen)
    {
        if (isFullScreen == false)
        {
            SubscribeAllResizeEvent();
        }
        else
        {
            UnsubscribeAllResizeEvent();
        }
        navigationBar.SetActive(!isFullScreen);
    }
}
