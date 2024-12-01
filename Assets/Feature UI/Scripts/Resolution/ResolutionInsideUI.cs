
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Cursor = Bed.UI.Cursor;

[RequireComponent(typeof(Resize))]
public class ResolutionInsideUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    
    [Header("화면 설정 (모든 X,Y를 하나의 값으로 통일하는 것을 권장)")] 
    [SerializeField, Tooltip("X는 왼쪽 최소 크기, Y는 아래 최소 크기")] private Vector2 offsetRightUpMin;
    [SerializeField, Tooltip("X는 오른쪽 최소 크기, Y는 위 최소 크기")] private Vector2 offsetLeftDownMin;
    
    private const float edgeDraggableThickness = 25f;                            // 화면 가장자리에서 드래그 가능한 영역의 두께
    
    private ResolutionManagement resolutionManagement;
    private Resize resize;
    private Vector2 staticOffsetMax, staticOffsetMin;
    
    private Action<Vector2> cardinalEvent, diagonalEvent;

    private bool isDragging = false;

    private void OnEnable()
    {
        resolutionManagement = ResolutionManagement.Instance;
        resize = GetComponent<Resize>();
        
        this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100);
        RegulateOffsetMinOverOutside();
    }
    
    private void Update()
    {
        // inside.sizeDelta = new Vector2(Mathf.Clamp(inside.sizeDelta.x, 10, outside.sizeDelta.x), Mathf.Clamp(inside.sizeDelta.y, 10, outside.sizeDelta.y));
        // inside.anchoredPosition = new Vector2(Mathf.Clamp(inside.anchoredPosition.x, -outside.sizeDelta.x * 0.5f + inside.sizeDelta.x * 0.5f, outside.sizeDelta.x * 0.5f - inside.sizeDelta.x * 0.5f),
        //     Mathf.Clamp(inside.anchoredPosition.y, -outside.sizeDelta.y * 0.5f + inside.sizeDelta.y * 0.5f, outside.sizeDelta.y * 0.5f - inside.sizeDelta.y * 0.5f));
        if (isDragging) ClampInside();
    }
    
    private void ClampInside()
    {
        resolutionManagement.InsideOffsetMin = Vector2.Max(resolutionManagement.InsideOffsetMin, -resolutionManagement.OutsideSize * 0.5f);
        resolutionManagement.InsideOffsetMin = Vector2.Min(resolutionManagement.InsideOffsetMin, staticOffsetMax - offsetLeftDownMin);
        resolutionManagement.InsideOffsetMax = Vector2.Max(resolutionManagement.InsideOffsetMax, staticOffsetMin + offsetRightUpMin);
        resolutionManagement.InsideOffsetMax = Vector2.Min(resolutionManagement.InsideOffsetMax, resolutionManagement.OutsideSize * 0.5f);
    }

    /// <summary>
    /// offsetRightUpMin과 offsetLeftDownMin을 outside의 범위를 넘어가지 않도록 조정
    /// </summary>
    private void RegulateOffsetMinOverOutside()
    {
        if (offsetRightUpMin.x < resolutionManagement.OutsideOffsetMin.x || offsetRightUpMin.y < resolutionManagement.OutsideOffsetMin.y)
        {
            offsetRightUpMin = resolutionManagement.OutsideOffsetMin;
        }
        
        if (offsetLeftDownMin.x > resolutionManagement.OutsideOffsetMax.x || offsetLeftDownMin.y > resolutionManagement.OutsideOffsetMax.y)
        {
            offsetLeftDownMin = resolutionManagement.OutsideOffsetMax;
        }
    }
    
        /// <summary>
    /// 마우스 드래그를 시작할 때
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        var localMousePoint = resolutionManagement.GetOutsideLocalMousePoint(eventData);
        staticOffsetMax = resolutionManagement.InsideOffsetMax;
        staticOffsetMin = resolutionManagement.InsideOffsetMin;
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

    /// <summary>
    /// 대각선 방향 드래그
    /// </summary>
    /// <param name="localMousePoint"></param>
    private void DragDiagnostic(Vector2 localMousePoint)
    {
        // InsideRect나 InsideOffsetMin, InsideOffsetMax를 Wrapper로 감싸서 참조 형식으로 만들던가
        // 포인터 써서 참조 형식으로 만들던가 하면 코드 길이를 줄일 수 있을 것 같음
        
        if (localMousePoint.x < -resolutionManagement.InsideRect.width * 0.5f + edgeDraggableThickness 
            && localMousePoint.y < -resolutionManagement.InsideRect.height * 0.5f + edgeDraggableThickness)         // 왼쪽 아래
        {
            diagonalEvent += (delta) => 
                resolutionManagement.InsideOffsetMin = new Vector2(resolutionManagement.InsideOffsetMin.x + delta.x, resolutionManagement.InsideOffsetMin.y + delta.y);
        }
        else if (localMousePoint.x > resolutionManagement.InsideRect.width * 0.5f - edgeDraggableThickness 
                 && localMousePoint.y > resolutionManagement.InsideRect.height * 0.5f - edgeDraggableThickness)     // 오른쪽 위
        {
            diagonalEvent += (delta) => 
                resolutionManagement.InsideOffsetMax = new Vector2(resolutionManagement.InsideOffsetMax.x + delta.x, resolutionManagement.InsideOffsetMax.y + delta.y);
        }
        
        if (localMousePoint.x > resolutionManagement.InsideRect.width * 0.5f - edgeDraggableThickness 
                 && localMousePoint.y < -resolutionManagement.InsideRect.height * 0.5f + edgeDraggableThickness)    // 오른쪽 아래
        {
            diagonalEvent += (delta) =>
            {
                resolutionManagement.InsideOffsetMax = new Vector2(resolutionManagement.InsideOffsetMax.x + delta.x, resolutionManagement.InsideOffsetMax.y);
                resolutionManagement.InsideOffsetMin = new Vector2(resolutionManagement.InsideOffsetMin.x, resolutionManagement.InsideOffsetMin.y + delta.y);
            };
        }
        else if (localMousePoint.x < -resolutionManagement.InsideRect.width * 0.5f + edgeDraggableThickness 
                 && localMousePoint.y > resolutionManagement.InsideRect.height * 0.5f - edgeDraggableThickness)     // 왼쪽 위
        {
            diagonalEvent += (delta) =>
            {
                resolutionManagement.InsideOffsetMin = new Vector2(resolutionManagement.InsideOffsetMin.x + delta.x, resolutionManagement.InsideOffsetMin.y);
                resolutionManagement.InsideOffsetMax = new Vector2(resolutionManagement.InsideOffsetMax.x, resolutionManagement.InsideOffsetMax.y + delta.y);
            };
        }
    }
    
    /// <summary>
    /// 동서남북 방향 드래그
    /// </summary>
    /// <param name="localMousePoint"></param>
    private void DragCardinal(Vector2 localMousePoint)
    {
        // InsideRect나 InsideOffsetMin, InsideOffsetMax를 Wrapper로 감싸서 참조 형식으로 만들던가
        // 포인터 써서 참조 형식으로 만들던가 하면 코드 길이를 줄일 수 있을 것 같음
        if (localMousePoint.x < -resolutionManagement.InsideRect.width * 0.5f + edgeDraggableThickness)         // 왼쪽
        {
            cardinalEvent += (delta) => 
                resolutionManagement.InsideOffsetMin = new Vector2(resolutionManagement.InsideOffsetMin.x + delta.x, resolutionManagement.InsideOffsetMin.y);
        }
        else if (localMousePoint.x > resolutionManagement.InsideRect.width * 0.5f - edgeDraggableThickness)     // 오른쪽
        {
            cardinalEvent += (delta) => 
                resolutionManagement.InsideOffsetMax = new Vector2(resolutionManagement.InsideOffsetMax.x + delta.x, resolutionManagement.InsideOffsetMax.y);
        }
        
        if (localMousePoint.y < -resolutionManagement.InsideRect.height * 0.5f + edgeDraggableThickness)        // 아래
        {
            cardinalEvent += (delta) => 
                resolutionManagement.InsideOffsetMin = new Vector2(resolutionManagement.InsideOffsetMin.x, resolutionManagement.InsideOffsetMin.y + delta.y);
        }
        else if (localMousePoint.y > resolutionManagement.InsideRect.height * 0.5f - edgeDraggableThickness)    // 위
        {
            cardinalEvent += (delta) => 
                resolutionManagement.InsideOffsetMax = new Vector2(resolutionManagement.InsideOffsetMax.x, resolutionManagement.InsideOffsetMax.y + delta.y);
        }
    }
}