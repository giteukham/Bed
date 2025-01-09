using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

// 해당 클래스가 들어 있는 오브젝트가 시작할 때 활성화 되어 있으면 오류 뜸.
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
