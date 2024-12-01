
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
    
    [Header("ȭ�� ���� (��� X,Y�� �ϳ��� ������ �����ϴ� ���� ����)")] 
    [SerializeField, Tooltip("X�� ���� �ּ� ũ��, Y�� �Ʒ� �ּ� ũ��")] private Vector2 offsetRightUpMin;
    [SerializeField, Tooltip("X�� ������ �ּ� ũ��, Y�� �� �ּ� ũ��")] private Vector2 offsetLeftDownMin;
    
    private const float edgeDraggableThickness = 25f;                            // ȭ�� �����ڸ����� �巡�� ������ ������ �β�
    
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
    /// offsetRightUpMin�� offsetLeftDownMin�� outside�� ������ �Ѿ�� �ʵ��� ����
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
    /// ���콺 �巡�׸� ������ ��
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
    /// ���콺 �巡�� ��
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.delta;
    
        cardinalEvent?.Invoke(delta);
        diagonalEvent?.Invoke(delta * 0.01f);
    }

    /// <summary>
    /// ���콺 �巡�׸� ���� ��
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        cardinalEvent = null;
        diagonalEvent = null;
    }

    /// <summary>
    /// �밢�� ���� �巡��
    /// </summary>
    /// <param name="localMousePoint"></param>
    private void DragDiagnostic(Vector2 localMousePoint)
    {
        // InsideRect�� InsideOffsetMin, InsideOffsetMax�� Wrapper�� ���μ� ���� �������� �������
        // ������ �Ἥ ���� �������� ������� �ϸ� �ڵ� ���̸� ���� �� ���� �� ����
        
        if (localMousePoint.x < -resolutionManagement.InsideRect.width * 0.5f + edgeDraggableThickness 
            && localMousePoint.y < -resolutionManagement.InsideRect.height * 0.5f + edgeDraggableThickness)         // ���� �Ʒ�
        {
            diagonalEvent += (delta) => 
                resolutionManagement.InsideOffsetMin = new Vector2(resolutionManagement.InsideOffsetMin.x + delta.x, resolutionManagement.InsideOffsetMin.y + delta.y);
        }
        else if (localMousePoint.x > resolutionManagement.InsideRect.width * 0.5f - edgeDraggableThickness 
                 && localMousePoint.y > resolutionManagement.InsideRect.height * 0.5f - edgeDraggableThickness)     // ������ ��
        {
            diagonalEvent += (delta) => 
                resolutionManagement.InsideOffsetMax = new Vector2(resolutionManagement.InsideOffsetMax.x + delta.x, resolutionManagement.InsideOffsetMax.y + delta.y);
        }
        
        if (localMousePoint.x > resolutionManagement.InsideRect.width * 0.5f - edgeDraggableThickness 
                 && localMousePoint.y < -resolutionManagement.InsideRect.height * 0.5f + edgeDraggableThickness)    // ������ �Ʒ�
        {
            diagonalEvent += (delta) =>
            {
                resolutionManagement.InsideOffsetMax = new Vector2(resolutionManagement.InsideOffsetMax.x + delta.x, resolutionManagement.InsideOffsetMax.y);
                resolutionManagement.InsideOffsetMin = new Vector2(resolutionManagement.InsideOffsetMin.x, resolutionManagement.InsideOffsetMin.y + delta.y);
            };
        }
        else if (localMousePoint.x < -resolutionManagement.InsideRect.width * 0.5f + edgeDraggableThickness 
                 && localMousePoint.y > resolutionManagement.InsideRect.height * 0.5f - edgeDraggableThickness)     // ���� ��
        {
            diagonalEvent += (delta) =>
            {
                resolutionManagement.InsideOffsetMin = new Vector2(resolutionManagement.InsideOffsetMin.x + delta.x, resolutionManagement.InsideOffsetMin.y);
                resolutionManagement.InsideOffsetMax = new Vector2(resolutionManagement.InsideOffsetMax.x, resolutionManagement.InsideOffsetMax.y + delta.y);
            };
        }
    }
    
    /// <summary>
    /// �������� ���� �巡��
    /// </summary>
    /// <param name="localMousePoint"></param>
    private void DragCardinal(Vector2 localMousePoint)
    {
        // InsideRect�� InsideOffsetMin, InsideOffsetMax�� Wrapper�� ���μ� ���� �������� �������
        // ������ �Ἥ ���� �������� ������� �ϸ� �ڵ� ���̸� ���� �� ���� �� ����
        if (localMousePoint.x < -resolutionManagement.InsideRect.width * 0.5f + edgeDraggableThickness)         // ����
        {
            cardinalEvent += (delta) => 
                resolutionManagement.InsideOffsetMin = new Vector2(resolutionManagement.InsideOffsetMin.x + delta.x, resolutionManagement.InsideOffsetMin.y);
        }
        else if (localMousePoint.x > resolutionManagement.InsideRect.width * 0.5f - edgeDraggableThickness)     // ������
        {
            cardinalEvent += (delta) => 
                resolutionManagement.InsideOffsetMax = new Vector2(resolutionManagement.InsideOffsetMax.x + delta.x, resolutionManagement.InsideOffsetMax.y);
        }
        
        if (localMousePoint.y < -resolutionManagement.InsideRect.height * 0.5f + edgeDraggableThickness)        // �Ʒ�
        {
            cardinalEvent += (delta) => 
                resolutionManagement.InsideOffsetMin = new Vector2(resolutionManagement.InsideOffsetMin.x, resolutionManagement.InsideOffsetMin.y + delta.y);
        }
        else if (localMousePoint.y > resolutionManagement.InsideRect.height * 0.5f - edgeDraggableThickness)    // ��
        {
            cardinalEvent += (delta) => 
                resolutionManagement.InsideOffsetMax = new Vector2(resolutionManagement.InsideOffsetMax.x, resolutionManagement.InsideOffsetMax.y + delta.y);
        }
    }
}