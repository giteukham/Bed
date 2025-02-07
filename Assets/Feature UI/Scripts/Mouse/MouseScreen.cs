using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

// �ش� Ŭ������ ��� �ִ� ������Ʈ�� ������ �� Ȱ��ȭ �Ǿ� ������ ���� ��.
public class MouseScreen : MonoBehaviour, IWindowUIBase, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] 
    private Image transparentPanel;

    [SerializeField] 
    private PlayerBase previewPlayer;
    
    public static event Action OnScreenActivate, OnScreenDeactivate;

    private void OnEnable()
    {
        OnScreenActivate?.Invoke();
        transparentPanel.DOFade(0.5f, 0f);
        previewPlayer.StopPlayer(true);
    }
    
    private void OnDisable()
    {
        OnScreenDeactivate?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transparentPanel.DOFade(0f, 0.2f);
        previewPlayer.StopPlayer(false);
        Cursor.visible = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transparentPanel.DOFade(0.5f, 0.2f);
        previewPlayer.StopPlayer(true);
        Cursor.visible = true;
    }
}
