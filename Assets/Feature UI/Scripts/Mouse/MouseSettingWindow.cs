using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

// �ش� Ŭ������ ��� �ִ� ������Ʈ�� ������ �� Ȱ��ȭ �Ǿ� ������ ���� ��.
public class MouseSettingWindow : MonoBehaviour, IWindowUIBase
{
    [Header("Player")]
    [SerializeField] 
    private PlayerBase player;
    [SerializeField]
    private PlayerBase previewPlayer;
    
    [SerializeField]
    private Transform playerPos, previewPlayerPos, meshesPos;
    
    private void OnEnable()
    {
        if (previewPlayer != null)
        {
            previewPlayer.TogglePlayer(true);
            previewPlayerPos.position = meshesPos.position;                 // �÷��̾��� �θ� ������Ʈ Global Position�� �ǹ� Mesh�� ��ġ�� �̵�
            MouseSettings.Instance.InitCameraSettings(previewPlayer);
        }
        
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
}
