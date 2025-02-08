using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

// 해당 클래스가 들어 있는 오브젝트가 시작할 때 활성화 되어 있으면 오류 뜸.
public class MouseSettingScreen : MonoBehaviour, IWindowUIBase, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] 
    private Image transparentPanel;

    [Header("Player")]
    [SerializeField] 
    private PlayerBase player;
    [SerializeField]
    private PlayerBase previewPlayer;
    
    [SerializeField]
    private Transform playerPos, previewPlayerPos, meshesPos;
    
    private void OnEnable()
    {
        if (player != null)
        {
            player.TogglePlayer(false);
            playerPos.position = previewPlayerPos.position;
        }
        
        if (previewPlayer != null)
        {
            previewPlayer.TogglePlayer(true);
            previewPlayerPos.position = meshesPos.position;                 // 플레이어의 부모 오브젝트 Global Position을 건물 Mesh의 위치로 이동
            MouseSettings.Instance.InitCameraSettings(previewPlayer);
        }
        
        transparentPanel.DOFade(0.5f, 0f);
        previewPlayer.StopPlayer(true);
    }

    private void OnDisable()
    {
        if (player != null)
        {
            player.TogglePlayer(true);
        }
        
        if (previewPlayer != null)
        {
            previewPlayer.TogglePlayer(false);
            previewPlayerPos.position = playerPos.position;                      // 플레이어의 부모 오브젝트 Global Position을 원래 플레이어의 위치로 이동
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transparentPanel.DOFade(0f, 0.2f);
        previewPlayer.StopPlayer(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transparentPanel.DOFade(0.5f, 0.2f);
        previewPlayer.StopPlayer(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
