using System;
using System.Globalization;
using Cinemachine;
using FMOD.Studio;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MouseSettings : MonoSingleton<MouseSettings>
{
    [Header("Preview Settings")] 
    [SerializeField] private Transform previewPlayerPos;                                           
    [SerializeField] private Transform meshesPos;                         // �÷��̾��� �θ� ������Ʈ Global Position, �ǹ� Mesh�� ��ġ                             
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
    }

    private void Start()
    {
        InitCameraSettings(player);
    }
    
    private void Update()
    {
        if (previewPlayer.gameObject.activeSelf)
        {
            CalculateMouseSpeed(previewPlayer);
        }
        else if (player.gameObject.activeSelf)
        {
            CalculateMouseSpeed(player); 
        }
    }

    private void CalculateMouseSpeed(PlayerBase player)
    {
        int verticalReverseConstant = isVerticalReverse ? -1 : 1;
        int horizontalReverseConstant = isHorizontalReverse ? -1 : 1;
        
        if ( PlayerConstant.isParalysis )
        {
            mouseHorizontalSpeed = player.povCamera.m_HorizontalAxis.m_InputAxisValue * 0.02f * horizontalReverseConstant;
            mouseVerticalSpeed = player.povCamera.m_VerticalAxis.m_InputAxisValue * 0.02f * verticalReverseConstant;
        }
        else
        {
            mouseHorizontalSpeed = player.povCamera.m_HorizontalAxis.m_InputAxisValue * horizontalReverseConstant;
            mouseVerticalSpeed = player.povCamera.m_VerticalAxis.m_InputAxisValue * verticalReverseConstant;
        }
    }
    
    private void InitMouseSetting()
    {
        mouseSensitivity = SaveManager.Instance.LoadMouseSensitivity();
        mouseMaxSpeed = mouseSensitivity * mouseSpeedMultiplier;
        isVerticalReverse = SaveManager.Instance.LoadMouseVerticalReverse();
        isHorizontalReverse = SaveManager.Instance.LoadMouseHorizontalReverse();
        deadZoneSliderValue = SaveManager.Instance.LoadDeadZoneValue() > deadZoneLimit ? deadZoneLimit : SaveManager.Instance.LoadDeadZoneValue();
        ChangeTurnAxisSpeed(deadZoneSliderValue);
    }

    public void InitCameraSettings(PlayerBase player)
    {
        player.povCamera.m_VerticalAxis.Value = 0f;
        player.povCamera.m_HorizontalAxis.Value = 0f;
        player.povCamera.m_VerticalAxis.m_MaxSpeed = mouseMaxSpeed;
        player.povCamera.m_HorizontalAxis.m_MaxSpeed = mouseMaxSpeed;
        player.povCamera.m_VerticalAxis.m_InvertInput = !isVerticalReverse;
        player.povCamera.m_HorizontalAxis.m_InvertInput = isHorizontalReverse;
    }
    
    /// <summary>
    /// ���콺 ���Ϲ���
    /// </summary>
    public void MouseVerticalReverse()
    {
        ToggleVerticalReverse();
        player.povCamera.m_VerticalAxis.m_InvertInput = !isVerticalReverse;
        previewPlayer.povCamera.m_VerticalAxis.m_InvertInput = !isVerticalReverse;
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
        player.povCamera.m_HorizontalAxis.m_InvertInput = isHorizontalReverse;
        previewPlayer.povCamera.m_HorizontalAxis.m_InvertInput = isHorizontalReverse;
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
        
        player.povCamera.m_VerticalAxis.m_MaxSpeed = mouseMaxSpeed;
        player.povCamera.m_HorizontalAxis.m_MaxSpeed = mouseMaxSpeed;
        previewPlayer.povCamera.m_VerticalAxis.m_MaxSpeed = mouseMaxSpeed;
        previewPlayer.povCamera.m_HorizontalAxis.m_MaxSpeed = mouseMaxSpeed;
        
        SaveManager.Instance.SaveMouseSensitivity(mouseSensitivity);
    }

    public void ChangeTurnAxisSpeed(float newSliderValue)
    {
        float normalValue = Mathf.InverseLerp(0f, deadZoneLimit, newSliderValue);
        float value = Mathf.Lerp(mouseAxisLimit, mouseAxisLimit - (mouseAxisLimit * deadZoneLimit), normalValue);
        turnRightSpeed = value;
        turnLeftSpeed = -value;
    }
    
    public void ChangeDeadZoneSliderValue(float value)
    {
        deadZoneSliderValue = value;
        SaveManager.Instance.SaveDeadZoneValue(deadZoneSliderValue);
    }

    public void ResetMouseSpeed()
    {
        mouseHorizontalSpeed = 0;
        mouseVerticalSpeed = 0;
    }
}