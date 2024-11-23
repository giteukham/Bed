using System;
using System.Globalization;
using Cinemachine;
using UnityEngine;

public class MouseSettings : MonoSingleton<MouseSettings>
{
    [Header("Preview Settings")] 
    [SerializeField] private Transform globalPlayersPos, meshesPos;                         // �÷��̾��� �θ� ������Ʈ Global Position, �ǹ� Mesh�� ��ġ                             
    private Vector3 mainPlayerPos;
    
    [SerializeField] private Player player;
    [SerializeField] private MouseSettingsPreviewPlayer previewPlayer;
    
    [Tooltip("�⺻ ���� ������ �������� 3, ���� �������� -3")]
    [SerializeField] private float turnRightSpeed, turnLeftSpeed;

    [Tooltip("���콺 �ӵ� �ִ밪. �⺻ ���� max = 10f")]
    [SerializeField] private float mouseAxisLimit;                                          // ���콺 �ӵ� �ִ밪. Cinemachine�� Axis value�� �������µ�
                                                                                            // axis ���� �ִ� ������ ��� ������ ������ ��
    
    [Tooltip("Deadzone ������ �ִ�"), Range(0f, 1f)]
    [SerializeField] private float deadZoneLimit;                                           // Deadzone ������ �ִ�. ��, 0�̸� Deadzone�� ������ �ø� �� �ִµ� 1�̸� Deadzone�� ����
    
    private const float mouseSpeedMultiplier = 500f;                                        // ���콺 ���� ���
    private float mouseSensitivity = 1f;
    private float mouseMaxSpeed;                                                            // ���콺 �ִ� �ӵ�
    private float mouseHorizontalSpeed, mouseVerticalSpeed;                                     
    private bool isVerticalReverse, isHorizontalReverse;                                    // false�� ����, true�� ����
    private float deadZoneSliderValue;                                                      // ���� Deadzone ������ ��
    
    #region Properties
    public float MouseSensitivity => mouseSensitivity;
    public float MouseMaxSpeed => mouseMaxSpeed;
    public float MouseAxisLimit => mouseAxisLimit;
    public float MouseHorizontalSpeed => Mathf.Clamp(mouseHorizontalSpeed, -mouseAxisLimit, mouseAxisLimit);
    public float MouseVerticalSpeed => Mathf.Clamp(mouseVerticalSpeed, -mouseAxisLimit, mouseAxisLimit);
    public float TurnRightSpeed => turnRightSpeed;
    public float TurnLeftSpeed => turnLeftSpeed;
    public bool IsVerticalReverse => isVerticalReverse;
    public bool IsHorizontalReverse => isHorizontalReverse;
    public float DeadZoneLimit => deadZoneLimit;
    public float DeadZoneSliderValue => deadZoneSliderValue;
    #endregion
    
    public event Action<bool> OnVerticalReverse, OnHorizontalReverse;

    private void Awake()
    {
        InitMouseSetting();
        
        MouseWindowUI.OnScreenActive += () =>
        {
            player?.EnablePlayerObject(false);
            previewPlayer?.EnablePlayerObject(true);
            
            mainPlayerPos = globalPlayersPos.position;
            globalPlayersPos.position = meshesPos.position;                 // �÷��̾��� �θ� ������Ʈ Global Position�� �ǹ� Mesh�� ��ġ�� �̵�
            InitCameraSettings(previewPlayer);
        };
        MouseWindowUI.OnScreenDeactive += () =>
        {
            if (player != null) player?.EnablePlayerObject(true);
            if (previewPlayer != null) previewPlayer?.EnablePlayerObject(false);
            
            if (globalPlayersPos != null) globalPlayersPos.position = mainPlayerPos;                      // �÷��̾��� �θ� ������Ʈ Global Position�� ���� �÷��̾��� ��ġ�� �̵�
        };
    }

    private void Start()
    {
        InitCameraSettings(player);
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
        deadZoneSliderValue = SaveManager.Instance.LoadDeadZoneValue();
        SaveManager.Instance.LoadTurnSpeed(out turnRightSpeed, out turnLeftSpeed);
    }

    private void InitCameraSettings(PlayerBase player)
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

    public void ChangeTurnAxisSpeed(float value)
    {
        turnRightSpeed = value;
        turnLeftSpeed = -value;
        SaveManager.Instance.SaveTurnSpeed(turnRightSpeed, turnLeftSpeed);
    }
    
    public void ChangeDeadZoneSliderValue(float value)
    {
        deadZoneSliderValue = value;
        SaveManager.Instance.SaveDeadZoneValue(deadZoneSliderValue);
    }
}