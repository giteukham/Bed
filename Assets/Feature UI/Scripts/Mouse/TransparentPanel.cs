
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TransparentPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Image transparentPanel;
    
    [SerializeField]
    private PlayerBase previewPlayer;

    private void OnEnable()
    {
        transparentPanel = GetComponent<Image>();
        transparentPanel.DOFade(0.5f, 0f);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        transparentPanel.DOFade(0f, 0.2f);
        previewPlayer.StopPlayer(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transparentPanel.DOFade(0.5f, 0.2f);
        previewPlayer.StopPlayer(true);
        Cursor.lockState = CursorLockMode.None;
    }
}
