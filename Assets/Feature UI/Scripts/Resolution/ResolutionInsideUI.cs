
using System;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

public class ResolutionInsideUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private ResolutionManagement resolutionManagement;
    
    [Header("화면 설정 (모든 X,Y를 하나의 값으로 통일하는 것을 권장)")] 
    [SerializeField, Tooltip("X는 왼쪽 최소 크기, Y는 아래 최소 크기")] private Vector2 offsetRightUpMin;
    [SerializeField, Tooltip("X는 오른쪽 최소 크기, Y는 위 최소 크기")] private Vector2 offsetLeftDownMin;

    private RectTransform inside, outside;
    private Image navBar;
    private RawImage zoomIcon;
    
    private Action<Vector2> cardinalEvent, diagonalEvent;
    
    private const float edgeDraggableThickness = 50;                            // 화면 가장자리에서 드래그 가능한 영역의 두께
    private Vector2 staticOffsetMax, staticOffsetMin;

    private bool isDragging = false;

    private void OnEnable()
    {
        inside = resolutionManagement.InSide;
        outside = resolutionManagement.OutSide;
        
        RegulateOffsetMinOverOutside();
    }

    private void Update()
    {
        // inside.sizeDelta = new Vector2(Mathf.Clamp(inside.sizeDelta.x, 10, outside.sizeDelta.x), Mathf.Clamp(inside.sizeDelta.y, 10, outside.sizeDelta.y));
        // inside.anchoredPosition = new Vector2(Mathf.Clamp(inside.anchoredPosition.x, -outside.sizeDelta.x * 0.5f + inside.sizeDelta.x * 0.5f, outside.sizeDelta.x * 0.5f - inside.sizeDelta.x * 0.5f),
        //     Mathf.Clamp(inside.anchoredPosition.y, -outside.sizeDelta.y * 0.5f + inside.sizeDelta.y * 0.5f, outside.sizeDelta.y * 0.5f - inside.sizeDelta.y * 0.5f));
        
        if (isDragging) ClampInside();
    }
    
    /// <summary>
    /// 마우스 드래그를 시작할 때
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        var localMousePoint = GetLocalMousePoint(eventData);
        staticOffsetMax = inside.offsetMax;
        staticOffsetMin = inside.offsetMin;
        isDragging = true;
        
        DragDiagnostic(localMousePoint);
        DragCardinal(localMousePoint);
    }
    
    /// <summary>
    /// 마우스 드래그 중
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.delta;
        
        cardinalEvent?.Invoke(delta);
        diagonalEvent?.Invoke(delta * 0.01f);
    }

    /// <summary>
    /// 마우스 드래그를 끝낼 때
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        cardinalEvent = null;
        diagonalEvent = null;
    }
    
    private void ClampInside()
    {
        inside.offsetMin = Vector2.Max(inside.offsetMin, -outside.sizeDelta * 0.5f);
        inside.offsetMin = Vector2.Min(inside.offsetMin, staticOffsetMax - offsetLeftDownMin);
        inside.offsetMax = Vector2.Max(inside.offsetMax, staticOffsetMin + offsetRightUpMin);
        inside.offsetMax = Vector2.Min(inside.offsetMax, outside.sizeDelta * 0.5f);
    }

    /// <summary>
    /// UI 내 로컬 마우스 위치
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    private Vector2 GetLocalMousePoint(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inside, eventData.position, eventData.pressEventCamera, out Vector2 localMousePoint);
        return localMousePoint;
    }
    
    /// <summary>
    /// 대각선 방향 드래그
    /// </summary>
    /// <param name="localMousePoint"></param>
    private void DragDiagnostic(Vector2 localMousePoint)
    {
        if (localMousePoint.x < -inside.rect.width * 0.5f + edgeDraggableThickness 
            && localMousePoint.y < -inside.rect.height * 0.5f + edgeDraggableThickness)         // 왼쪽 아래
        {
            diagonalEvent += (delta) => inside.offsetMin = new Vector2(inside.offsetMin.x + delta.x, inside.offsetMin.y + delta.y);
        }
        else if (localMousePoint.x > inside.rect.width * 0.5f - edgeDraggableThickness 
                 && localMousePoint.y > inside.rect.height * 0.5f - edgeDraggableThickness)     // 오른쪽 위
        {
            diagonalEvent += (delta) => inside.offsetMax = new Vector2(inside.offsetMax.x + delta.x, inside.offsetMax.y + delta.y);
        }
        
        if (localMousePoint.x > inside.rect.width * 0.5f - edgeDraggableThickness 
                 && localMousePoint.y < -inside.rect.height * 0.5f + edgeDraggableThickness)    // 오른쪽 아래
        {
            diagonalEvent += (delta) =>
            {
                inside.offsetMax = new Vector2(inside.offsetMax.x + delta.x, inside.offsetMax.y);
                inside.offsetMin = new Vector2(inside.offsetMin.x, inside.offsetMin.y + delta.y);
            };
        }
        else if (localMousePoint.x < -inside.rect.width * 0.5f + edgeDraggableThickness 
                 && localMousePoint.y > inside.rect.height * 0.5f - edgeDraggableThickness)     // 왼쪽 위
        {
            diagonalEvent += (delta) =>
            {
                inside.offsetMin = new Vector2(inside.offsetMin.x + delta.x, inside.offsetMin.y);
                inside.offsetMax = new Vector2(inside.offsetMax.x, inside.offsetMax.y + delta.y);
            };
        }
    }
    
    /// <summary>
    /// 동서남북 방향 드래그
    /// </summary>
    /// <param name="localMousePoint"></param>
    private void DragCardinal(Vector2 localMousePoint)
    {
        if (localMousePoint.x < -inside.rect.width * 0.5f + edgeDraggableThickness)
        {
            cardinalEvent += (delta) => inside.offsetMin = new Vector2(inside.offsetMin.x + delta.x, inside.offsetMin.y);
        }
        else if (localMousePoint.x > inside.rect.width * 0.5f - edgeDraggableThickness)
        {
            cardinalEvent += (delta) => inside.offsetMax = new Vector2(inside.offsetMax.x + delta.x, inside.offsetMax.y);
        }
        
        if (localMousePoint.y < -inside.rect.height * 0.5f + edgeDraggableThickness)
        {
            cardinalEvent += (delta) => inside.offsetMin = new Vector2(inside.offsetMin.x, inside.offsetMin.y + delta.y);
        }
        else if (localMousePoint.y > inside.rect.height * 0.5f - edgeDraggableThickness)
        {
            cardinalEvent += (delta) => inside.offsetMax = new Vector2(inside.offsetMax.x, inside.offsetMax.y + delta.y);
        }
    }

    /// <summary>
    /// offsetRightUpMin과 offsetLeftDownMin을 outside의 범위를 넘어가지 않도록 조정
    /// </summary>
    private void RegulateOffsetMinOverOutside()
    {
        if (offsetRightUpMin.x < outside.offsetMin.x || offsetRightUpMin.y < outside.offsetMin.y)
        {
            offsetRightUpMin = outside.offsetMin;
        }
        
        if (offsetLeftDownMin.x > outside.offsetMax.x || offsetLeftDownMin.y > outside.offsetMax.y)
        {
            offsetLeftDownMin = outside.offsetMax;
        }
    }
}