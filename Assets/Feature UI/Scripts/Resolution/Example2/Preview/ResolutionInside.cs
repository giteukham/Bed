
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Bed.UI;
using Cysharp.Threading.Tasks.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
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


public class ResolutionInside : MonoBehaviour, IDragHandler, IPointerClickHandler, IPointerDownHandler
{
    [Header("내부 UI")]
    [SerializeField]
    private RectTransform blankRect;
    private RectTransform insideScreenRect;
    private Image insideImage;
    
    private float insideImageBasePixelMultiplier = 0.17f;
    
    [SerializeField]
    private TMP_Text resolutionText;
    
    [SerializeField]
    [Tooltip("드래그 가능한 영역")]
    private ResizeBounds resizeBounds;
    
    [Header("기타 설정")]
    [SerializeField] 
    private Material previewMaskMaterial;
    
    [SerializeField] 
    private InsideNavigationBar insideNavigationBar;
    
    [SerializeField]
    private ResolutionSelectController resolutionSelectController;
    
    private Dictionary<ResizeType, InsideResizer> resizeEvents = new Dictionary<ResizeType, InsideResizer>();
    private ResizeType resizeCurrentType;
    private bool isResizing;
    
    private ResolutionSettingsData previewData;
    private ResolutionSettingsDTO backupData;
    private DynamicUIData dynamicUIData;
    
    private ZoomState zoomCurrentState = ZoomState.Minimize;
    private Vector2Int zoomSavedResolution;
    private Vector2[] zoomSavedOffsets;

    private float lastClickTime = -1f;
    private bool isDrag = false;
    
    public void Initialize(ResolutionSettingsData previewData, ResolutionSettingsDTO backupData, DynamicUIData dynamicUIData)
    {
        this.previewData = previewData;
        this.previewData.PropertyChanged += OnPropertyChanged;
        this.backupData = backupData;
        this.dynamicUIData = dynamicUIData;
        insideScreenRect = GetComponent<RectTransform>();
        insideImage = GetComponent<Image>();
        
        insideNavigationBar?.Initialize();
        
        resolutionSelectController.OnSelectChanged += OnDropdownSelectChanged;
        
        AddAllResizeEvent();
    }
    
    private void OnDropdownSelectChanged()
    {
        insideScreenRect.sizeDelta = previewData.IsWindowed ? dynamicUIData.InsideCurrentSize : dynamicUIData.InsideMaxSize;
        insideScreenRect.anchoredPosition = Vector2.zero;
    }
    
    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        ChangeAsteriskOnModified();
        
        if (e.PropertyName == nameof(ResolutionSettingsData.IsWindowed))
        {
            if (previewData.IsWindowed)
            {
                ResizeInsideByOffsets(dynamicUIData.InsideCurrentOffsets[0], dynamicUIData.InsideCurrentOffsets[1]);
                ChangeInsidePixelMultiplier(insideImageBasePixelMultiplier);
            }
            else
            {
                ResizeInsideByOffsets(dynamicUIData.InsideMaxOffsets[0], dynamicUIData.InsideMaxOffsets[1]);
                ChangeInsidePixelMultiplier(Math.Round(dynamicUIData.UserAspectRatio, 2) == Math.Round(StaticUIData.BaseAspectRatio, 2) ? insideImageBasePixelMultiplier : 50f);
            }
            
            insideScreenRect.anchoredPosition = Vector2.zero;
            ToggleNavigationBar(previewData.IsWindowed);
        }
    }
    
    private void ChangeInsidePixelMultiplier(float pixelMultiplier)
    {
        insideImage.pixelsPerUnitMultiplier = pixelMultiplier;
        insideNavigationBar?.ChangeNavigationBarPixelMultiplier(insideImage.pixelsPerUnitMultiplier);
    }

    private void ChangeAsteriskOnModified()
    {
        if (previewData.ResolutionWidth != backupData.ResolutionWidth ||
            previewData.ResolutionHeight != backupData.ResolutionHeight ||
            previewData.IsWindowed != backupData.IsWindowed ||
            previewData.FrameRate != backupData.FrameRate ||
            !Mathf.Approximately(previewData.ScreenBrightness, backupData.ScreenBrightness))
        {
            SetResolutionText(previewData.ResolutionWidth, previewData.ResolutionHeight, previewData.FrameRate, true);
        }
        else
        {
            SetResolutionText(previewData.ResolutionWidth, previewData.ResolutionHeight, previewData.FrameRate, false);
        }
    }

    private void OnEnable()
    {
        previewData.ChangeData(backupData);
        zoomSavedResolution = new Vector2Int(previewData.ResolutionWidth, previewData.ResolutionHeight);
        SetResolutionText(Display.main.systemWidth, Display.main.systemHeight, previewData.FrameRate, false);
        ResizeInsideByOffsets(dynamicUIData.InsideMaxOffsets[0], dynamicUIData.InsideMaxOffsets[1]);
        insideScreenRect.anchoredPosition = Vector2.zero;
        
        ToggleNavigationBar(previewData.IsWindowed);
    }
    
    private void OnDisable()
    {
        isResizing = false;
        Cursor.SetCursor(CursorType.Normal);
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
    
    private static InsideResizer CreateResizeEvent(GameObject resizeBound, ResizeType type)
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
    
    float minWidth = dynamicUIData.InsideMinSize.x;
    float minHeight = dynamicUIData.InsideMinSize.y;
    
    switch (resizeCurrentType)
    {
        case ResizeType.Left:
        case ResizeType.LeftUp:
            float leftOffsetMaxY = Mathf.Min(
                insideScreenRect.offsetMax.y - (Mathf.Clamp(localPoint.x, blankRect.offsetMin.x, blankRect.offsetMax.x) - insideScreenRect.offsetMin.x) / StaticUIData.BaseAspectRatio,
                blankRect.offsetMax.y
            );

            leftOffsetMaxY = Mathf.Max(leftOffsetMaxY, insideScreenRect.offsetMin.y + minHeight);

            float leftOffsetMinX = insideScreenRect.offsetMin.x + (insideScreenRect.offsetMax.y - leftOffsetMaxY) * StaticUIData.BaseAspectRatio;
            
            leftOffsetMinX = Mathf.Min(leftOffsetMinX, insideScreenRect.offsetMax.x - minWidth);

            ResizeInsideByOffsets(
                new Vector2(leftOffsetMinX, insideScreenRect.offsetMin.y), 
                new Vector2(insideScreenRect.offsetMax.x, leftOffsetMaxY));
            break;
        
        case ResizeType.Right:
        case ResizeType.RightUp:
            float rightOffsetMaxY = Mathf.Min(
                insideScreenRect.offsetMax.y - (insideScreenRect.offsetMax.x - Mathf.Clamp(localPoint.x, blankRect.offsetMin.x, blankRect.offsetMax.x)) / StaticUIData.BaseAspectRatio,
                blankRect.offsetMax.y
            );
            
            rightOffsetMaxY = Mathf.Max(rightOffsetMaxY, insideScreenRect.offsetMin.y + minHeight);
            
            float rightOffsetMaxX = insideScreenRect.offsetMax.x - (insideScreenRect.offsetMax.y - rightOffsetMaxY) * StaticUIData.BaseAspectRatio;
            
            rightOffsetMaxX = Mathf.Max(rightOffsetMaxX, insideScreenRect.offsetMin.x + minWidth);

            ResizeInsideByOffsets(
                new Vector2(insideScreenRect.offsetMin.x, insideScreenRect.offsetMin.y), 
                new Vector2(rightOffsetMaxX, rightOffsetMaxY));
            break;
        
        case ResizeType.Up:
            float upOffsetMaxX = Mathf.Min(
                insideScreenRect.offsetMax.x + (Mathf.Clamp(localPoint.y, blankRect.offsetMin.y, blankRect.offsetMax.y) - insideScreenRect.offsetMax.y) * StaticUIData.BaseAspectRatio,
                blankRect.offsetMax.x
            );
            
            upOffsetMaxX = Mathf.Max(upOffsetMaxX, insideScreenRect.offsetMin.x + minWidth);
            
            float upOffsetMaxY = insideScreenRect.offsetMax.y + (upOffsetMaxX - insideScreenRect.offsetMax.x) / StaticUIData.BaseAspectRatio;
            
            upOffsetMaxY = Mathf.Max(upOffsetMaxY, insideScreenRect.offsetMin.y + minHeight);

            ResizeInsideByOffsets(
                new Vector2(insideScreenRect.offsetMin.x, insideScreenRect.offsetMin.y), 
                new Vector2(upOffsetMaxX, upOffsetMaxY));
            break;
        
        case ResizeType.Down:
        case ResizeType.LeftDown:
            float downOffsetMinX = Mathf.Max(
                insideScreenRect.offsetMin.x + (Mathf.Clamp(localPoint.y, blankRect.offsetMin.y, blankRect.offsetMax.y) - insideScreenRect.offsetMin.y) * StaticUIData.BaseAspectRatio,
                blankRect.offsetMin.x
            );
            
            downOffsetMinX = Mathf.Min(downOffsetMinX, insideScreenRect.offsetMax.x - minWidth);
            
            float downOffsetMinY = insideScreenRect.offsetMin.y + (downOffsetMinX - insideScreenRect.offsetMin.x) / StaticUIData.BaseAspectRatio;
            
            downOffsetMinY = Mathf.Min(downOffsetMinY, insideScreenRect.offsetMax.y - minHeight);

            ResizeInsideByOffsets(
                new Vector2(downOffsetMinX, downOffsetMinY),
                new Vector2(insideScreenRect.offsetMax.x, insideScreenRect.offsetMax.y));
            break;
        
        case ResizeType.RightDown:
            float rightDownOffsetMaxX = Mathf.Min(
                insideScreenRect.offsetMax.x - (Mathf.Clamp(localPoint.y, blankRect.offsetMin.y, blankRect.offsetMax.y) - insideScreenRect.offsetMin.y) * StaticUIData.BaseAspectRatio,
                blankRect.offsetMax.x
            );
            
            rightDownOffsetMaxX = Mathf.Max(rightDownOffsetMaxX, insideScreenRect.offsetMin.x + minWidth);

            float rightDownOffsetMinY = insideScreenRect.offsetMin.y + (insideScreenRect.offsetMax.x - rightDownOffsetMaxX) / StaticUIData.BaseAspectRatio;
            
            rightDownOffsetMinY = Mathf.Min(rightDownOffsetMinY, insideScreenRect.offsetMax.y - minHeight);

            ResizeInsideByOffsets(
                new Vector2(insideScreenRect.offsetMin.x, rightDownOffsetMinY), 
                new Vector2(rightDownOffsetMaxX, insideScreenRect.offsetMax.y));
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
        if (eventData.button != PointerEventData.InputButton.Left || eventData.dragging) return;

        if (eventData.clickTime - lastClickTime <= 0.3f) 
        {
            DoZoom();
            lastClickTime = -1f;
        }
        else lastClickTime = eventData.clickTime;
    }
    
    private Vector2 insideMoveBaseOffset;
    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(blankRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        insideMoveBaseOffset = insideScreenRect.anchoredPosition - localPoint;
    }
    
    /// <summary>
    /// Preview 창을 드래그할 때 창을 움직일 수 있게 하는 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (!previewData.IsWindowed) return;
        isDrag = true;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(blankRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        Vector2 targetPos = localPoint + insideMoveBaseOffset;
        
        targetPos.x = Mathf.Clamp(targetPos.x,
            (blankRect.rect.min - insideScreenRect.rect.min).x, (blankRect.rect.max - insideScreenRect.rect.max).x);
        targetPos.y = Mathf.Clamp(targetPos.y,
                (blankRect.rect.min - insideScreenRect.rect.min).y, (blankRect.rect.max - insideScreenRect.rect.max).y);
        
        insideScreenRect.anchoredPosition = targetPos;
        
        Cursor.SetCursor(CursorType.Normal);
    }

    public void DoZoom()
    {
        if (!previewData.IsWindowed) return;
        
        if (zoomCurrentState == ZoomState.Minimize)
        {
            // Maximize 시작 시, 현재 사이즈 직접 저장
            zoomSavedOffsets = new Vector2[] { insideScreenRect.offsetMin, insideScreenRect.offsetMax };
    
            zoomCurrentState = ZoomState.Maximize;
            zoomSavedResolution = new Vector2Int(previewData.ResolutionWidth, previewData.ResolutionHeight);
    
            previewData.ResolutionWidth = Screen.width;
            previewData.ResolutionHeight = Screen.height;
            ResizeInsideByOffsets(dynamicUIData.InsideMaxOffsets[0], dynamicUIData.InsideMaxOffsets[1]);
        }
        else if (zoomCurrentState == ZoomState.Maximize)
        {
            zoomCurrentState = ZoomState.Minimize;
    
            previewData.ResolutionWidth = zoomSavedResolution.x;
            previewData.ResolutionHeight = zoomSavedResolution.y;
    
            // 저장된 오프셋 사용
            ResizeInsideByOffsets(zoomSavedOffsets[0], zoomSavedOffsets[1]);
        }
    }
    
    public void SetResolutionText(int width, int height, int frame, bool isModified = false)
    {
        resolutionText.text = isModified ? $"{width} X {height} *\n{frame}Hz" : $"{width} X {height}\n{frame}Hz";
    }
    
    private void ResizeInsideByOffsets(Vector2 offsetMin, Vector2 offsetMax)
    {
        insideScreenRect.offsetMin = offsetMin;
        insideScreenRect.offsetMax = offsetMax;
    
        var size = insideScreenRect.sizeDelta;
    
        // 나의 해상도 비율이 16:9 보다 클 때
        if (dynamicUIData.UserAspectRatio >= StaticUIData.BaseAspectRatio)
        {
            size.y *= previewData.IsWindowed 
                ? (previewData.windowedMaxHeight / (StaticUIData.BaseHeight / 3f)) 
                : (previewData.fullScreenMaxHeight / (StaticUIData.BaseHeight / 3f));
            size.x = previewData.IsWindowed
                ? size.y * StaticUIData.BaseAspectRatio
                : size.y * dynamicUIData.UserAspectRatio;
        }
        else
        {
            size.x *= previewData.IsWindowed 
                ? (previewData.windowedMaxWidth / (StaticUIData.BaseWidth / 3f)) 
                : (previewData.fullScreenMaxWidth / (StaticUIData.BaseWidth / 3f));
            size.y = previewData.IsWindowed
                ? size.x * StaticUIData.BaseReverseAspectRatio
                : size.x * dynamicUIData.UserReverseAspectRatio;
        }
    
        previewData.ResolutionWidth = Convert.ToInt32(size.x);
        previewData.ResolutionHeight = Convert.ToInt32(size.y);
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
