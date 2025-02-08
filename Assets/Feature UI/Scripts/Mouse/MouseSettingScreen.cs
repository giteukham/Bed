using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

// �ش� Ŭ������ ��� �ִ� ������Ʈ�� ������ �� Ȱ��ȭ �Ǿ� ������ ���� ��.
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
            previewPlayerPos.position = meshesPos.position;                 // �÷��̾��� �θ� ������Ʈ Global Position�� �ǹ� Mesh�� ��ġ�� �̵�
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
            previewPlayerPos.position = playerPos.position;                      // �÷��̾��� �θ� ������Ʈ Global Position�� ���� �÷��̾��� ��ġ�� �̵�
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
