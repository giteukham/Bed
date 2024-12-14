
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
/// </summary>
public class InsideWindow : MonoBehaviour, IDragHandler
{
    private ResolutionManagement resolutionManager;
    private RectTransform insideRectTransform;
    
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
    private Vector2 insideSize, outsideSize;

    private readonly float minSize = 300f;
    private readonly float aspectRatio = 1.777778f;

    public UnityEvent<RectTransform> OnRectTransformReSize;
    
    private void OnEnable()
    {
        insideRectTransform = GetComponent<RectTransform>();
        resolutionManager = ResolutionManagement.Instance;
        resolutionManager.OnFullScreenSwitched.AddListener(FullScreenSwitchHandler);
        
        FullScreenSwitchHandler(resolutionManager.IsFullScreen);
    }

    private void OnDisable()
    {
        resolutionManager.OnFullScreenSwitched.RemoveListener(FullScreenSwitchHandler);
    }

    private void OnRectTransformDimensionsChange()
    {
        OnRectTransformReSize?.Invoke(insideRectTransform);
    }

    private void Update()
    {
        //ConfineInsideInOutside();
    }
    
    private void ConfineInsideInOutside()
    {
        insideOffsetMax = resolutionManager.InsideOffsetMax;
        insideOffsetMin = resolutionManager.InsideOffsetMin;
        outsideOffsetMax = resolutionManager.OutsideOffsetMax;
        outsideOffsetMin = resolutionManager.OutsideOffsetMin;
        
        insideOffsetMax.x = Mathf.Clamp(insideOffsetMax.x, insideOffsetMin.x, outsideOffsetMax.x);
        insideOffsetMax.y = Mathf.Clamp(insideOffsetMax.y, insideOffsetMin.y, outsideOffsetMax.y);
        insideOffsetMin.x = Mathf.Clamp(insideOffsetMin.x, outsideOffsetMin.x, insideOffsetMax.x);
        insideOffsetMin.y = Mathf.Clamp(insideOffsetMin.y, outsideOffsetMin.y, insideOffsetMax.y);
        
        resolutionManager.InsideOffsetMax = insideOffsetMax;
        resolutionManager.InsideOffsetMin = insideOffsetMin;
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

    private void OnResizeStart(ResizeType type)
    {
        currentResizeType = type;
        insideWindowZoom.SetZoomState(ZoomState.ZoomIn);
    }
    
    private void OnResizeStay(PointerEventData eventData)
    {
        ChangeCursor(currentResizeType);
        insideSize = resolutionManager.InsideSize;
        insideOffsetMin = resolutionManager.InsideOffsetMin;
        insideOffsetMax = resolutionManager.InsideOffsetMax;
        outsideOffsetMax = resolutionManager.OutsideOffsetMax;
        outsideOffsetMin = resolutionManager.OutsideOffsetMin;
        
        Vector2 delta = eventData.delta;
        
        switch (currentResizeType)
        {
            case ResizeType.Left:
                // left에다가 delta.x를 더해줌에 따라 left가 줄어들면서 오른쪽으로 이동
                // bottom은 delta.x에다가 1.777778을 곱해줌으로써 aspect ratio를 유지
                float newLeft = Mathf.Clamp(insideRectTransform.GetLeft() + (delta.x * 0.5f), 0, 900);
                float newBottom = Mathf.Clamp(insideRectTransform.GetBottom() + (delta.x / aspectRatio * 0.5f), 0, 900);
                insideRectTransform.SetLeft(newLeft);
                insideRectTransform.SetBottom(newBottom);
                Debug.Log($"{insideRectTransform.sizeDelta}");
                // insideOffsetMin.x = Mathf.Clamp(insideOffsetMin.x + delta.x, outsideOffsetMin.x, insideOffsetMax.x);
                //
                // insideSize.x = insideOffsetMax.x - insideOffsetMin.x;
                // insideSize.y = insideSize.x / aspectRatio;
                //
                // insideOffsetMin.y = Mathf.Clamp(insideOffsetMax.y - insideSize.y, outsideOffsetMin.y, outsideOffsetMax.y);
                break;

            case ResizeType.Right:
                insideOffsetMax.x = Mathf.Clamp(insideOffsetMax.x + delta.x, insideOffsetMin.x, outsideOffsetMax.x);
                
                insideSize.x = insideOffsetMax.x - insideOffsetMin.x;
                insideSize.y = insideSize.x / aspectRatio;
                
                insideOffsetMax.y = insideOffsetMin.y + insideSize.y;
                break;
            
            case ResizeType.Up:
                insideOffsetMax.y = Mathf.Clamp(insideOffsetMax.y + delta.y, insideOffsetMin.y, outsideOffsetMax.y);
                
                insideSize.y = insideOffsetMax.y - insideOffsetMin.y;
                insideSize.x = insideSize.y * aspectRatio;
                
                insideOffsetMax.x = insideOffsetMin.x + insideSize.x;
                break;
            
            case ResizeType.Down:
                insideOffsetMin.y = Mathf.Clamp(insideOffsetMin.y + delta.y, outsideOffsetMin.y, insideOffsetMax.y);
                
                insideSize.y = insideOffsetMax.y - insideOffsetMin.y;
                insideSize.x = insideSize.y * aspectRatio;
                
                insideOffsetMin.x = insideOffsetMax.x - insideSize.x;
                break;
            
            case ResizeType.LeftUp:
                insideSize.x = Mathf.Clamp(insideSize.x - delta.x, minSize, outsideOffsetMax.x - outsideOffsetMin.x);
                insideSize.y = insideSize.x / aspectRatio;
                
                insideOffsetMin.x = insideOffsetMax.x - insideSize.x;
                insideOffsetMax.y = insideOffsetMin.y + insideSize.y;
                break;
            
            case ResizeType.LeftDown:
                insideSize.x = Mathf.Clamp(insideSize.x - delta.x, minSize, outsideOffsetMax.x - outsideOffsetMin.x);
                insideSize.y = insideSize.x / aspectRatio;
                
                insideOffsetMin.x = insideOffsetMax.x - insideSize.x;
                insideOffsetMin.y = insideOffsetMax.y - insideSize.y;
                break;
            
            case ResizeType.RightUp:
                insideSize.x = Mathf.Clamp(insideSize.x + delta.x, minSize, outsideOffsetMax.x - outsideOffsetMin.x);
                insideSize.y = insideSize.x / aspectRatio;
                
                insideOffsetMax.x = insideOffsetMin.x + insideSize.x;
                insideOffsetMax.y = insideOffsetMin.y + insideSize.y;
                break;
            
            case ResizeType.RightDown:
                insideSize.x = Mathf.Clamp(insideSize.x + delta.x, minSize, outsideOffsetMax.x - outsideOffsetMin.x);
                insideSize.y = insideSize.x / aspectRatio;
                
                insideOffsetMax.x = insideOffsetMin.x + insideSize.x;
                insideOffsetMax.y = insideOffsetMin.y + insideSize.y;
                break;
        }
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
        if (resolutionManager.IsFullScreen) return;

        Vector2 delta = eventData.delta;
        Vector2 insideAnchoredPosition = resolutionManager.InsideAnchoredPosition;
        
        insideAnchoredPosition.x = Mathf.Clamp(insideAnchoredPosition.x + delta.x, -(insideRectTransform.GetRight() * 0.5f), insideRectTransform.GetLeft() * 0.5f);
        insideAnchoredPosition.y = Mathf.Clamp(insideAnchoredPosition.y + delta.y, -(insideRectTransform.GetTop() * 0.5f), insideRectTransform.GetBottom() * 0.5f);
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
