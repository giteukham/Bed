
using System;
using System.Collections.Generic;
using System.ComponentModel;
using AYellowpaper.SerializedCollections.Editor;
using Bed.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
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
public class OnZoomEvent : UnityEvent { }

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


public class ResolutionInside : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    [Header("내부 UI")]
    [SerializeField] 
    private RectTransform blankRect;
    private RectTransform blackScreenRect;
    
    [SerializeField]
    [Tooltip("드래그 가능한 영역")]
    private ResizeBounds resizeBounds;
    
    [Header("기타 설정")]
    [SerializeField] 
    private Material previewMaskMaterial;
    
    [SerializeField] 
    private InsideNavigationBar insideNavigationBar;
    
    private Dictionary<ResizeType, InsideResizer> resizeEvents = new Dictionary<ResizeType, InsideResizer>();
    private ResizeType resizeCurrentType;
    private bool isResizing;
    
    private ResolutionSettingsData previewData;
    private ResolutionSettingsDTO backupData;
    private ResolutionPreviewPanel.UIPreviewData uiData;
    
    private ZoomState zoomCurrentState = ZoomState.Maximize;
    private Vector2 zoomSavedOffsetMin, zoomSavedOffsetMax;
    
    // 기준 해상도. 1920x1080 == 1
    private readonly float baseWidth = 1920;
    private readonly float baseHeight = 1080;
        
    // TODO: 최소 사이즈 지정해야 함
    public void Initialize(ResolutionSettingsData previewData, ResolutionSettingsDTO backupData, ResolutionPreviewPanel.UIPreviewData uiData)
    {
        this.previewData = previewData;
        this.previewData.PropertyChanged += OnPropertyChanged;
        this.backupData = backupData;
        this.uiData = uiData;
        blackScreenRect = GetComponent<RectTransform>();
        
        AddAllResizeEvent();
    }
    
    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ResolutionSettingsData.IsWindowed))
        {
            ResizePreviewByOffsets(blankRect.offsetMin, blankRect.offsetMax);
            ToggleNavigationBar(previewData.IsWindowed);
        }
    }

    private void OnEnable()
    {
        blackScreenRect.sizeDelta = new Vector2(uiData.InsideMaxSize.x, uiData.InsideMaxSize.y);
        blackScreenRect.anchoredPosition = Vector2.zero;
        zoomSavedOffsetMax = blackScreenRect.offsetMax;
        zoomSavedOffsetMin = blackScreenRect.offsetMin;
        
        ToggleResizeEvent(previewData.IsWindowed);
        insideNavigationBar.SetNavigationBarActive(previewData.IsWindowed);
        insideNavigationBar.onZoom.AddListener(DoZoom);
    }
    
    private void OnDisable()
    {
        isResizing = false;
        Cursor.SetCursor(CursorType.Normal);
        insideNavigationBar.onZoom.RemoveListener(DoZoom);
    }
    
    
    private void OnRectTransformDimensionsChange()
    {

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
            
            var insideBoundaryResize = CreateResizeEvent(resizeBound, type);
            
            if (resizeBound != null)
            {
                this.resizeEvents.TryAdd(type, insideBoundaryResize);
            }
        }
    }
    
    private InsideResizer CreateResizeEvent(GameObject resizeBound, ResizeType type)
    {
        InsideResizer insideResizer = resizeBound.AddComponent<InsideResizer>();
        insideResizer.ResizeType = type;
    
        return insideResizer;
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
        resizeCurrentType = type;
        zoomCurrentState = ZoomState.Minimize;
        isResizing = true;
    }
    
    private void OnResizeStay(PointerEventData eventData)
    {
        ChangeCursorByType(resizeCurrentType);
        
        var localPoint = blankRect.InverseTransformPoint(eventData.position);
        
        switch (resizeCurrentType)
        {
            case ResizeType.Left:
            case ResizeType.LeftUp:
                float leftOffsetMaxY = Mathf.Min(
                    blackScreenRect.offsetMax.y - (Mathf.Clamp(localPoint.x, blankRect.offsetMin.x, blankRect.offsetMax.x) - blackScreenRect.offsetMin.x) / uiData.AspectRatio,
                    blankRect.offsetMax.y
                );

                float leftOffsetMinX = blackScreenRect.offsetMin.x + (blackScreenRect.offsetMax.y - leftOffsetMaxY) * uiData.AspectRatio;

                ResizePreviewByOffsets(
                    new Vector2(leftOffsetMinX, blackScreenRect.offsetMin.y), 
                    new Vector2(blackScreenRect.offsetMax.x, leftOffsetMaxY));
                break;
            
            case ResizeType.Right:
            case ResizeType.RightUp:
                float rightOffsetMaxY = Mathf.Min(
                    blackScreenRect.offsetMax.y - (blackScreenRect.offsetMax.x - Mathf.Clamp(localPoint.x, blankRect.offsetMin.x, blankRect.offsetMax.x)) / uiData.AspectRatio,
                    blankRect.offsetMax.y
                );
                float rightOffsetMaxX = blackScreenRect.offsetMax.x - (blackScreenRect.offsetMax.y - rightOffsetMaxY) * uiData.AspectRatio;
                
                ResizePreviewByOffsets(
                    new Vector2(blackScreenRect.offsetMin.x, blackScreenRect.offsetMin.y), 
                    new Vector2(rightOffsetMaxX, rightOffsetMaxY));
                break;
            
            case ResizeType.Up:
                float upOffsetMaxX = Mathf.Min(
                    blackScreenRect.offsetMax.x + (Mathf.Clamp(localPoint.y, blankRect.offsetMin.y, blankRect.offsetMax.y) - blackScreenRect.offsetMax.y) * uiData.AspectRatio,
                    blankRect.offsetMax.x
                );
                
                float upOffsetMaxY = blackScreenRect.offsetMax.y + (upOffsetMaxX - blackScreenRect.offsetMax.x) / uiData.AspectRatio;

                ResizePreviewByOffsets(
                    new Vector2(blackScreenRect.offsetMin.x, blackScreenRect.offsetMin.y), 
                    new Vector2(upOffsetMaxX, upOffsetMaxY));
                break;
            
            case ResizeType.Down:
            case ResizeType.LeftDown:
                float downOffsetMinX = Mathf.Max(
                    blackScreenRect.offsetMin.x + (Mathf.Clamp(localPoint.y, blankRect.offsetMin.y, blankRect.offsetMax.y) - blackScreenRect.offsetMin.y) * uiData.AspectRatio,
                    blankRect.offsetMin.x
                );
                
                float downOffsetMinY = blackScreenRect.offsetMin.y + (downOffsetMinX - blackScreenRect.offsetMin.x) / uiData.AspectRatio;
                
                ResizePreviewByOffsets(
                    new Vector2(downOffsetMinX, downOffsetMinY),
                    new Vector2(blackScreenRect.offsetMax.x, blackScreenRect.offsetMax.y));
                break;
            
            case ResizeType.RightDown:
// X축 최대값 계산 및 제한
                float rightDownOffsetMaxX = Mathf.Min(
                    blackScreenRect.offsetMax.x - (Mathf.Clamp(localPoint.y, blankRect.offsetMin.y, blankRect.offsetMax.y) - blackScreenRect.offsetMin.y) * uiData.AspectRatio,
                    blankRect.offsetMax.x
                );

// 최종 X값에 기반하여 Y값 계산
                float rightDownOffsetMinY = blackScreenRect.offsetMin.y + (blackScreenRect.offsetMax.x - rightDownOffsetMaxX) / uiData.AspectRatio;

// 값 적용
                ResizePreviewByOffsets(
                    new Vector2(blackScreenRect.offsetMin.x, rightDownOffsetMinY), 
                    new Vector2(rightDownOffsetMaxX, blackScreenRect.offsetMax.y));
                break;
        }
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
        if (eventData.clickCount == 2) DoZoom();
    }
    
    /// <summary>
    /// Preview 창을 드래그할 때 창을 움직일 수 있게 하는 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (!previewData.IsWindowed) return;
        
        Vector2 insideSize = blackScreenRect.sizeDelta;

        float scaleX = baseWidth / Display.main.systemWidth;    // TODO: 나중에 Data 값으로 변경
        float scaleY = baseHeight / Display.main.systemHeight;
        float distanceX = (blankRect.sizeDelta.x - insideSize.x) * 0.5f;
        float distanceY = (blankRect.sizeDelta.y - insideSize.y) * 0.5f;

        blackScreenRect.anchoredPosition = new Vector2(
        Mathf.Clamp(blackScreenRect.anchoredPosition.x + (eventData.delta.x * scaleX), -distanceX, distanceX),
        Mathf.Clamp(blackScreenRect.anchoredPosition.y + (eventData.delta.y * scaleY), -distanceY, distanceY));
    }
    
    public void DoZoom()
    {
        if (!previewData.IsWindowed) return;
        
        if (zoomCurrentState == ZoomState.Minimize)
        {
            zoomCurrentState = ZoomState.Maximize;
            zoomSavedOffsetMin = blackScreenRect.offsetMin;
            zoomSavedOffsetMax = blackScreenRect.offsetMax;
            
            ResizePreviewByOffsets(blankRect.offsetMin, blankRect.offsetMax);
        }
        else if (zoomCurrentState == ZoomState.Maximize)
        {
            zoomCurrentState = ZoomState.Minimize;
            ResizePreviewByOffsets(zoomSavedOffsetMin, zoomSavedOffsetMax);
        }
    }
    
    private void ResizePreviewByOffsets(Vector2 offsetMin, Vector2 offsetMax)
    {
        blackScreenRect.offsetMin = offsetMin;
        blackScreenRect.offsetMax = offsetMax;
    }
    
    private static void ChangeCursorByType(ResizeType type)
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
    
    private void ToggleNavigationBar(bool isWindowChecked)
    {
        insideNavigationBar.SetNavigationBarActive(isWindowChecked);
        ToggleResizeEvent(isWindowChecked);
    }
    
    private void ToggleResizeEvent(bool isWindowChecked)
    {
        if (isWindowChecked)
        {
            SubscribeAllResizeEvent();
        }
        else
        {
            UnsubscribeAllResizeEvent();
        }
    }
}
