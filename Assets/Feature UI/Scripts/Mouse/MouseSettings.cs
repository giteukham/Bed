using System;
using System.Globalization;
using Cinemachine;
using UnityEngine;

public class MouseSettings : MonoSingleton<MouseSettings>
{
    [Header("Preview Settings")] 
    [SerializeField] private Transform globalPlayersPos, meshesPos;
    private Vector3 mainPlayerPos;
    
    [SerializeField] private Player player;
    [SerializeField] private MouseSettingsPreviewPlayer previewPlayer;
    
    [Tooltip("�⺻ ���� ������ �������� 3, ���� �������� -3")]
    [SerializeField] private float turnRightSpeed = 3f, turnLeftSpeed = -3f;                // ���콺 Delta X���� 5�̻��̸� Right State��, -5�����̸� Left State�� ����
    
    private const float mouseSpeedMultiplier = 500f;                                // ���콺 ���� ���
    private float mouseSensitivity = 1f;
    private float mouseMaxSpeed;
    private float mouseHorizontalSpeed, mouseVerticalSpeed;
    private bool isVerticalReverse, isHorizontalReverse;                            // false�� ����, true�� ����
    
    public float MouseSensitivity => mouseSensitivity;
    public float MouseMaxSpeed => mouseMaxSpeed;
    public float MouseHorizontalSpeed => mouseHorizontalSpeed;
    public float MouseVerticalSpeed => mouseVerticalSpeed;
    public float TurnRightSpeed => turnRightSpeed;
    public float TurnLeftSpeed => turnLeftSpeed;
    public bool IsVerticalReverse => isVerticalReverse;
    public bool IsHorizontalReverse => isHorizontalReverse;
    
    public event Action<bool> OnVerticalReverse, OnHorizontalReverse;
    private void OnEnable()
    {
        MouseWindowUI.OnMouseSettingsScreenActive += () =>
        {
            player?.EnablePlayerObject(false);
            previewPlayer?.EnablePlayerObject(true);
            InitCamera(previewPlayer);
            mainPlayerPos = globalPlayersPos.position;
            globalPlayersPos.position = meshesPos.position;
        };
        MouseWindowUI.OnMouseSettingsScreenDeactive += () =>
        {
            player?.EnablePlayerObject(true);
            previewPlayer?.EnablePlayerObject(false);
            globalPlayersPos.position = mainPlayerPos;
        };
    }
    
    private void Start()
    {
        InitMouseSetting();
        InitCamera(player);
    }
    
    private void Update()
    {
        if (PlayerConstant.isPlayerStop)
        {
            mouseHorizontalSpeed = 0;
            mouseVerticalSpeed = 0;
        }
        
        if (player.isActiveAndEnabled)
            CalculateMouseSpeed(player);
        else if (previewPlayer.isActiveAndEnabled)
            CalculateMouseSpeed(previewPlayer);
    }

    private void CalculateMouseSpeed(PlayerBase player)
    {
        int verticalReverseConstant = isVerticalReverse ? -1 : 1;
        int horizontalReverseConstant = isHorizontalReverse ? -1 : 1;
        
        if ( PlayerConstant.isParalysis )
        {
            mouseHorizontalSpeed = player.POVCamera.m_HorizontalAxis.m_InputAxisValue * 0.02f * horizontalReverseConstant;
            mouseVerticalSpeed = player.POVCamera.m_VerticalAxis.m_InputAxisValue * 0.02f * verticalReverseConstant;
        }
        else
        {
            mouseHorizontalSpeed = player.POVCamera.m_HorizontalAxis.m_InputAxisValue * horizontalReverseConstant;
            mouseVerticalSpeed = player.POVCamera.m_VerticalAxis.m_InputAxisValue * verticalReverseConstant;
        }
    }
    
    private void InitMouseSetting()
    {
        mouseSensitivity = SaveManager.Instance.LoadMouseSensitivity();
        mouseMaxSpeed = mouseSensitivity * mouseSpeedMultiplier;
        isVerticalReverse = SaveManager.Instance.LoadMouseVerticalReverse();
        isHorizontalReverse = SaveManager.Instance.LoadMouseHorizontalReverse();
    }

    private void InitCamera(PlayerBase player)
    {
        player.POVCamera.m_VerticalAxis.Value = 0f;
        player.POVCamera.m_HorizontalAxis.Value = 0f;
        player.POVCamera.m_VerticalAxis.m_MaxSpeed = mouseMaxSpeed;
        player.POVCamera.m_HorizontalAxis.m_MaxSpeed = mouseMaxSpeed;
        player.POVCamera.m_VerticalAxis.m_InvertInput = !isVerticalReverse;
        player.POVCamera.m_HorizontalAxis.m_InvertInput = isHorizontalReverse;
    }
    
    /// <summary>
    /// ���콺 ���Ϲ���
    /// </summary>
    public void MouseVerticalReverse()
    {
        ToggleVerticalReverse();
        player.POVCamera.m_VerticalAxis.m_InvertInput = !isVerticalReverse;
        previewPlayer.POVCamera.m_VerticalAxis.m_InvertInput = !isVerticalReverse;
        OnVerticalReverse?.Invoke(isVerticalReverse);
    }
    private void ToggleVerticalReverse()
    {
        isVerticalReverse = !isVerticalReverse;
        SaveManager.Instance.SaveMouseVerticalReverse(isVerticalReverse);
    }

    /// <summary>
    /// ���콺 �¿� ����
    /// </summary>
    public void MouseHorizontalReverse()
    {
        ToggleHorizontalReverse();
        player.POVCamera.m_HorizontalAxis.m_InvertInput = isHorizontalReverse;
        previewPlayer.POVCamera.m_HorizontalAxis.m_InvertInput = isHorizontalReverse;
        OnHorizontalReverse?.Invoke(isHorizontalReverse);
    }

    private void ToggleHorizontalReverse()
    {
        isHorizontalReverse = !isHorizontalReverse;
        SaveManager.Instance.SaveMouseHorizontalReverse(isHorizontalReverse);
    }
    
    /// <summary>
    /// �����̴� ������ ���� �ڵ� ȣ��
    /// </summary>
    public void ChangeSensitivity(float value)
    {
        mouseSensitivity = value;
        mouseMaxSpeed = mouseSensitivity * mouseSpeedMultiplier;
        
        player.POVCamera.m_VerticalAxis.m_MaxSpeed = mouseMaxSpeed;
        player.POVCamera.m_HorizontalAxis.m_MaxSpeed = mouseMaxSpeed;
        previewPlayer.POVCamera.m_VerticalAxis.m_MaxSpeed = mouseMaxSpeed;
        previewPlayer.POVCamera.m_HorizontalAxis.m_MaxSpeed = mouseMaxSpeed;
        
        SaveManager.Instance.SaveMouseSensitivity(mouseSensitivity);
    }
}