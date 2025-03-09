
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Bed.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
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
    private RectTransform insideScreenRect;
    
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
    
    public void Initialize(ResolutionSettingsData previewData, ResolutionSettingsDTO backupData, DynamicUIData dynamicUIData)
    {
        this.previewData = previewData;
        this.previewData.PropertyChanged += OnPropertyChanged;
        this.backupData = backupData;
        this.dynamicUIData = dynamicUIData;
        insideScreenRect = GetComponent<RectTransform>();
        
        resolutionSelectController.OnSelectChanged += () =>
        {
            insideScreenRect.sizeDelta = dynamicUIData.InsideCurrentSize;
            insideScreenRect.anchoredPosition = Vector2.zero;
        };
        
        AddAllResizeEvent();
    }
    
    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
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
        
        if (e.PropertyName == nameof(ResolutionSettingsData.IsWindowed))
        {
            // 전체화면
            if (!previewData.IsWindowed)
            {
                ResizeInsideByOffsets(dynamicUIData.InsideMaxOffsets[0], dynamicUIData.InsideMaxOffsets[1]);
                insideScreenRect.anchoredPosition = Vector2.zero;
            }
            ToggleNavigationBar(previewData.IsWindowed);
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
        insideNavigationBar.onZoom.AddListener(DoZoom);
    }
    
    private void OnDisable()
    {
        isResizing = false;
        Cursor.SetCursor(CursorType.Normal);
        insideNavigationBar.onZoom.RemoveListener(DoZoom);
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
                    insideScreenRect.offsetMax.y - (Mathf.Clamp(localPoint.x, blankRect.offsetMin.x, blankRect.offsetMax.x) - insideScreenRect.offsetMin.x) / StaticUIData.BaseAspectRatio,
                    blankRect.offsetMax.y
                );

                float leftOffsetMinX = insideScreenRect.offsetMin.x + (insideScreenRect.offsetMax.y - leftOffsetMaxY) * StaticUIData.BaseAspectRatio;

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
                float rightOffsetMaxX = insideScreenRect.offsetMax.x - (insideScreenRect.offsetMax.y - rightOffsetMaxY) * StaticUIData.BaseAspectRatio;

                ResizeInsideByOffsets(
                    new Vector2(insideScreenRect.offsetMin.x, insideScreenRect.offsetMin.y), 
                    new Vector2(rightOffsetMaxX, rightOffsetMaxY));
                break;
            
            case ResizeType.Up:
                float upOffsetMaxX = Mathf.Min(
                    insideScreenRect.offsetMax.x + (Mathf.Clamp(localPoint.y, blankRect.offsetMin.y, blankRect.offsetMax.y) - insideScreenRect.offsetMax.y) * StaticUIData.BaseAspectRatio,
                    blankRect.offsetMax.x
                );
                
                float upOffsetMaxY = insideScreenRect.offsetMax.y + (upOffsetMaxX - insideScreenRect.offsetMax.x) / StaticUIData.BaseAspectRatio;

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
                
                float downOffsetMinY = insideScreenRect.offsetMin.y + (downOffsetMinX - insideScreenRect.offsetMin.x) / StaticUIData.BaseAspectRatio;

                ResizeInsideByOffsets(
                    new Vector2(downOffsetMinX, downOffsetMinY),
                    new Vector2(insideScreenRect.offsetMax.x, insideScreenRect.offsetMax.y));
                break;
            
            case ResizeType.RightDown:
// X축 최대값 계산 및 제한
                float rightDownOffsetMaxX = Mathf.Min(
                    insideScreenRect.offsetMax.x - (Mathf.Clamp(localPoint.y, blankRect.offsetMin.y, blankRect.offsetMax.y) - insideScreenRect.offsetMin.y) * StaticUIData.BaseAspectRatio,
                    blankRect.offsetMax.x
                );

// 최종 X값에 기반하여 Y값 계산
                float rightDownOffsetMinY = insideScreenRect.offsetMin.y + (insideScreenRect.offsetMax.x - rightDownOffsetMaxX) / StaticUIData.BaseAspectRatio;

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
        if (eventData.clickCount == 2) DoZoom();
    }
    
    /// <summary>
    /// Preview 창을 드래그할 때 창을 움직일 수 있게 하는 함수
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (!previewData.IsWindowed) return;
    
        Vector2 insideSize = insideScreenRect.sizeDelta;
        Vector2 parentSize = blankRect.sizeDelta;

        // 기본 스케일 계산
        float baseScaleX = parentSize.x / insideSize.x;
        float baseScaleY = parentSize.y / insideSize.y;
    
        // 조정된 스케일 - 작은 창일수록 낮은 스케일 적용
        float adjustedScaleX = Mathf.Lerp(1f, baseScaleX, insideSize.x / parentSize.x);
        float adjustedScaleY = Mathf.Lerp(1f, baseScaleY, insideSize.y / parentSize.y);
    
        float distanceX = (parentSize.x - insideSize.x) * 0.5f;
        float distanceY = (parentSize.y - insideSize.y) * 0.5f;
    
        insideScreenRect.anchoredPosition = new Vector2(
            Mathf.Clamp(insideScreenRect.anchoredPosition.x + (eventData.delta.x * adjustedScaleX), -distanceX, distanceX),
            Mathf.Clamp(insideScreenRect.anchoredPosition.y + (eventData.delta.y * adjustedScaleY), -distanceY, distanceY));
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
        // 먼저 offset 값을 적용해보고 체크
        Vector2 originalOffsetMin = insideScreenRect.offsetMin;
        Vector2 originalOffsetMax = insideScreenRect.offsetMax;
    
        // 새 offset 임시 적용
        insideScreenRect.offsetMin = offsetMin;
        insideScreenRect.offsetMax = offsetMax;
    
        // 적용 후 사이즈가 최소값보다 작은지 체크
        if (insideScreenRect.sizeDelta.x < dynamicUIData.InsideMinSize.x || 
            insideScreenRect.sizeDelta.y < dynamicUIData.InsideMinSize.y)
        {
            // 원래 값으로 복원하고 리턴
            insideScreenRect.offsetMin = originalOffsetMin;
            insideScreenRect.offsetMax = originalOffsetMax;
            return;
        }

        var size = insideScreenRect.sizeDelta;

        if (dynamicUIData.UserAspectRatio >= StaticUIData.BaseAspectRatio)
        {
            size.y *= (Display.main.systemHeight / (StaticUIData.BaseHeight / 3f));
            size.x = size.y * dynamicUIData.UserAspectRatio;
        }
        else
        {
            size.x *= (Display.main.systemWidth / (StaticUIData.BaseWidth / 3f));
            size.y = size.x * dynamicUIData.UserReverseAspectRatio;
        }

        previewData.ResolutionWidth = Mathf.CeilToInt(size.x);
        previewData.ResolutionHeight = Mathf.CeilToInt(size.y);
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
