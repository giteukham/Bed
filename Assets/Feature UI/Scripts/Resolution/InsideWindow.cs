
using System;
using System.Collections.Generic;
using Bed.UI;
using Cysharp.Threading.Tasks.Triggers;
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

public enum ZoomState
{
    Minimize,
    Maximize
}

[Serializable]
public class ResizeBounds
{
    public GameObject left, right, up, down;
    public GameObject leftUp, leftDown, rightUp, rightDown;
}

/// <summary>
/// TODO:
/// 마우스 포인터 이미 적용되어 있으면 다른 거로 안 바뀌게
/// </summary>
public class InsideWindow : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    private ResolutionManagement resolutionManager;
    private RectTransform insideRect;

    [Header("Inside 내부 UI")]
    
    [SerializeField]
    private Transform previewPanel;
    
    [SerializeField]
    [Tooltip("드래그 가능한 영역")]
    private ResizeBounds resizeBounds;
    
    [SerializeField]
    [Tooltip("Inside 창 경계")]
    private RectTransform insideBoundary;
    
    [Header("기타 설정")]
    [SerializeField] private Material previewMaskMaterial;
    [SerializeField] private InsideNavigationBar insideNavigationBar;
    [FormerlySerializedAs("insideWindowZoom")] [SerializeField] private InsideWindowZoomButton insideWindowZoomButton;
    
    private Dictionary<ResizeType, ResizeEvent> resizeEvents = new Dictionary<ResizeType, ResizeEvent>();
    private ResizeType currentResizeType;
    private bool isResizing;
    
    private static ZoomState zoomState = ZoomState.Minimize;
    private static Vector2 savedOffsetMin, savedOffsetMax;
    
    private Vector2 insideOffsetMin, insideOffsetMax;
    private Vector2 prevOffsetMin, prevOffsetMax;
    
    private Vector2 insideLowestSize;
    private float previewPanelScale;

    private readonly float aspectRatio = 1.777778f;

    public UnityEvent<RectTransform> OnRectTransformReSize;

    public ZoomState ZoomState
    {
        get => zoomState;
        set => zoomState = value;
    }
    
    public Vector2 SavedOffsetMin => savedOffsetMin;
    public Vector2 SavedOffsetMax => savedOffsetMax;
    
    private void Awake()
    {
        resolutionManager = ResolutionManagement.Instance;
        insideRect = resolutionManager.InsideRectTransform;
        
        Vector2[] offsets = resolutionManager.ConvertResolutionToOffsets(new Vector2Int(Display.main.systemWidth, Display.main.systemHeight));
        insideBoundary.offsetMin = offsets[0];
        insideBoundary.offsetMax = offsets[1];
        
        previewPanelScale = previewPanel.transform.localScale.x;
        AddAllResizeEvent();
    }

    private void OnEnable()
    {
        ToggleResizeEvent(resolutionManager.IsWindowedScreenReady);
        insideNavigationBar.SetNavigationBarActive(resolutionManager.IsWindowedScreenReady);
        resolutionManager.OnFullScreenSwitched.AddListener(FullScreenSwitchHandler);
        
        Vector2Int insideLowestResolution = resolutionManager.GetLowestResolution();
        insideLowestSize = resolutionManager.ConvertResolutionToSize(insideLowestResolution);
    }

    private void OnDisable()
    {
        isResizing = false;
        Cursor.SetCursor(CursorType.Normal);
        resolutionManager.OnFullScreenSwitched.RemoveListener(FullScreenSwitchHandler);
    }

    private void OnRectTransformDimensionsChange()
    {
        if (insideRect == null) return;
        OnRectTransformReSize?.Invoke(insideRect);
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
                resizeEvent.OnResizeStart = OnResizeStart;
                resizeEvent.OnResizeEnd = OnResizeEnd;
                resizeEvent.OnResizeStay = OnResizeStay;
                resizeEvent.OnPointerEnterEvent = OnPointerEnterEvent;
                resizeEvent.OnPointerExitEvent = OnPointerExitEvent;
            }
        }
    }
    
    private void UnsubscribeAllResizeEvent()
    {
        foreach (ResizeType type in Enum.GetValues(typeof(ResizeType)))
        {
            if (resizeEvents.TryGetValue(type, out var resizeEvent))
            {
                resizeEvent.OnResizeStart = null;
                resizeEvent.OnResizeEnd = null;
                resizeEvent.OnResizeStay = null;
                resizeEvent.OnPointerEnterEvent = null;
                resizeEvent.OnPointerExitEvent = null;
            }
        }
    }
    
    private void OnResizeStart(ResizeType type, PointerEventData eventData)
    {
        currentResizeType = type;
        prevOffsetMin = resolutionManager.InsideOffsetMin;
        prevOffsetMax = resolutionManager.InsideOffsetMax;
        ZoomState = ZoomState.Minimize;
        isResizing = true;
    }
    
    private void OnResizeStay(PointerEventData eventData)
    {
        ChangeCursorByType(currentResizeType);
        insideOffsetMin = resolutionManager.InsideOffsetMin;
        insideOffsetMax = resolutionManager.InsideOffsetMax;
        
        Vector2 localPoint = insideBoundary.InverseTransformPoint(eventData.position);
        
        switch (currentResizeType)
        {
            case ResizeType.Left:
            case ResizeType.LeftUp:
                float leftOffsetMinX = Mathf.Clamp(localPoint.x, insideBoundary.offsetMin.x, insideOffsetMax.x - insideLowestSize.x);
                float leftOffsetMaxY = Mathf.Clamp(
                    prevOffsetMax.y - (leftOffsetMinX - prevOffsetMin.x) / aspectRatio,
                    insideBoundary.offsetMin.y,
                    insideBoundary.offsetMax.y
                );
                
                // Y축이 최대값을 넘어가면 X축 늘어나는 거 방지
                if (leftOffsetMaxY >= insideBoundary.offsetMax.y)
                {
                    leftOffsetMaxY = insideBoundary.offsetMax.y;
                    leftOffsetMinX = prevOffsetMin.x + (prevOffsetMax.y - leftOffsetMaxY) * aspectRatio;
                }
                
                insideOffsetMin.x = leftOffsetMinX;
                insideOffsetMax.y = leftOffsetMaxY;
                break;
            
            case ResizeType.Right:
            case ResizeType.RightUp:
                float rightOffsetMaxX = Mathf.Clamp(localPoint.x, insideOffsetMin.x + insideLowestSize.x, insideBoundary.offsetMax.x);
                float rightOffsetMaxY = Mathf.Clamp(
                    prevOffsetMax.y + (rightOffsetMaxX - prevOffsetMax.x) / aspectRatio,
                    insideBoundary.offsetMin.y,
                    insideBoundary.offsetMax.y
                );
                
                if (rightOffsetMaxY >= insideBoundary.offsetMax.y)
                {
                    rightOffsetMaxY = insideBoundary.offsetMax.y;
                    rightOffsetMaxX = prevOffsetMax.x + (rightOffsetMaxY - prevOffsetMax.y) * aspectRatio;
                }
                
                insideOffsetMax.x = rightOffsetMaxX;
                insideOffsetMax.y = rightOffsetMaxY;
                break;
            
            case ResizeType.Up:
                float upOffsetMaxY = Mathf.Clamp(localPoint.y, insideOffsetMin.y + insideLowestSize.y, insideBoundary.offsetMax.y);
                float upOffsetMaxX = Mathf.Clamp(
                    prevOffsetMax.x + (upOffsetMaxY - prevOffsetMax.y) * aspectRatio,
                    insideOffsetMin.x + insideLowestSize.x,
                    insideBoundary.offsetMax.x
                );
                
                if (upOffsetMaxX >= insideBoundary.offsetMax.x)
                {
                    upOffsetMaxX = insideBoundary.offsetMax.x;
                    upOffsetMaxY = prevOffsetMax.y + (upOffsetMaxX - prevOffsetMax.x) / aspectRatio;
                }
                
                insideOffsetMax.y = upOffsetMaxY;
                insideOffsetMax.x = upOffsetMaxX;
                break;
            
            case ResizeType.Down:
            case ResizeType.LeftDown:
                float downOffsetMinY = Mathf.Clamp(localPoint.y, insideBoundary.offsetMin.y, insideOffsetMax.y - insideLowestSize.y);
                float downOffsetMinX = Mathf.Clamp(
                    prevOffsetMin.x + (downOffsetMinY - prevOffsetMin.y) * aspectRatio,
                    insideBoundary.offsetMin.x,
                    insideOffsetMax.x - insideLowestSize.x
                );
                
                if (downOffsetMinX <= insideBoundary.offsetMin.x)
                {
                    downOffsetMinX = insideBoundary.offsetMin.x;
                    downOffsetMinY = prevOffsetMin.y + (downOffsetMinX - prevOffsetMin.x) / aspectRatio;
                }
                
                insideOffsetMin.y = downOffsetMinY;
                insideOffsetMin.x = downOffsetMinX;
                break;
            
            case ResizeType.RightDown:
                float rightDownOffsetMinY = Mathf.Clamp(localPoint.y, insideBoundary.offsetMin.y, insideOffsetMax.y - insideLowestSize.y);
                float rightDownOffsetMaxX = Mathf.Clamp(
                    prevOffsetMax.x - (rightDownOffsetMinY - prevOffsetMin.y) * aspectRatio,
                    insideOffsetMin.x + insideLowestSize.x,
                    insideBoundary.offsetMax.x
                );
                
                if (rightDownOffsetMaxX >= insideBoundary.offsetMax.x)
                {
                    rightDownOffsetMaxX = insideBoundary.offsetMax.x;
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
        isResizing = false;
    }
    
    private void OnPointerEnterEvent(ResizeType type)
    {
        if (isResizing) return;
        ChangeCursorByType(type);
    }
    
    private void OnPointerExitEvent()
    {
        if (isResizing) return;
        Cursor.SetCursor(CursorType.Normal);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        resolutionManager.DoZoom(eventData);
    }
    
    /// <summary>
    /// Preview 창을 드래그할 때 창을 움직일 수 있게 하는 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (!resolutionManager.IsWindowedScreenReady) return;
        
        Vector2 insideAnchoredPosition = resolutionManager.InsideAnchoredPosition;
        Vector2 insideSize = resolutionManager.InsideSize;
        
        float distanceX = (insideBoundary.sizeDelta.x - insideSize.x) * 0.5f;
        float distanceY = (insideBoundary.sizeDelta.y - insideSize.y) * 0.5f;

        insideAnchoredPosition = new Vector2(
            Mathf.Clamp(insideAnchoredPosition.x + (eventData.delta.x / previewPanelScale), -distanceX, distanceX),
            Mathf.Clamp(insideAnchoredPosition.y + (eventData.delta.y / previewPanelScale), -distanceY, distanceY));

        resolutionManager.InsideAnchoredPosition = insideAnchoredPosition;
    }
    
    
    private void ChangeCursorByType(ResizeType type)
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
    
    private void FullScreenSwitchHandler(bool isWindowChecked)
    {
        insideNavigationBar.SetNavigationBarActive(isWindowChecked);
        ToggleResizeEvent(isWindowChecked);
    }
    
    private void ToggleResizeEvent(bool isWindowChecked)
    {
        if (isWindowChecked == true)
        {
            SubscribeAllResizeEvent();
        }
        else
        {
            UnsubscribeAllResizeEvent();
        }
    }
    
    public void SaveOffsets(Vector2 offsetMin, Vector2 offsetMax)
    {
        savedOffsetMin = offsetMin;
        savedOffsetMax = offsetMax;
    }
}
