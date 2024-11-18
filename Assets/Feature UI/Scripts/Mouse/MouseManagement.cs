using System;
using System.Collections.Generic;
using System.Globalization;
using AYellowpaper.SerializedCollections;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// ���� ��¥ : 2024-11-18 �ֹ���
/// </summary>
public enum Arrow
{
    Up,
    Down,
    Left,
    Right
}

//�⺻�� ����
//���� 1f, ���Ϲ��� true(1), �¿���� false(0)
public class MouseManagement : MonoBehaviour
{
    [Header("Preview Settings")] 
    [SerializeField] private Transform globalPlayersPos, meshesPos;
    [SerializeField] private MouseSettingsPreviewPlayer previewPlayer;
    [SerializeField] private Player player;

    private Vector3 tmpPos;
    private CinemachinePOV previewPOVCamera;
    private CinemachinePOV playerPOVCamera;
    
    [Header("Mouse UI")]
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_InputField sensitivityValue;
    [SerializeField] private Image verticalSwitch;
    [SerializeField] private Image horizontalSwitch;
    /// <summary>
    /// �ʷ� ����ġ �̹���
    /// </summary>
    [SerializeField] private Sprite onImage;
    /// <summary>
    /// ���� ����ġ �̹���
    /// </summary>
    [SerializeField] private Sprite offImage;
    
    [Header("Arrow")] 
    [SerializedDictionary("Arrow", "GameObject")]
    [SerializeField] private SerializedDictionary<Arrow, GameObject> arrows;

    private Dictionary<Arrow, Image> arrowImages;
    private readonly Color arrowMoveColor = Color.gray;
    /// <summary>
    /// ���콺 ���� ���� ����
    /// </summary>
    public static float mouseSensitivity = 1f;
    /// <summary>
    /// ���� ����Ǵ� ���콺 ����
    /// </summary>
    public static float mouseSpeed;

    private const float mouseSpeedMultiplier = 500f;        // ���콺 ���� ���

    public static void InitMouseSetting(float sensitivity)
    {
        mouseSensitivity = sensitivity;
        mouseSpeed = mouseSensitivity * mouseSpeedMultiplier;
    }

    /// <summary>
    /// �������� �ƴ� ui ��Ҹ� �ʱ�ȭ ��(������ �ʱ�ȭ�� SaveManager���� ������)
    /// </summary>
    private void OnEnable()
    {
        arrowImages = new Dictionary<Arrow, Image>
        {
            { Arrow.Up, arrows[Arrow.Up].GetComponent<Image>() },
            { Arrow.Down, arrows[Arrow.Down].GetComponent<Image>() },
            { Arrow.Left, arrows[Arrow.Left].GetComponent<Image>() },
            { Arrow.Right, arrows[Arrow.Right].GetComponent<Image>() }
        };
        tmpPos = globalPlayersPos.position;
        globalPlayersPos.position = meshesPos.position;
        
        previewPOVCamera = previewPlayer.POVCamera;
        playerPOVCamera = player?.POVCamera;
        
        previewPOVCamera.m_VerticalAxis.Value = 0f;
        previewPOVCamera.m_HorizontalAxis.Value = 0f;
        previewPOVCamera.m_VerticalAxis.m_InvertInput = !InputSystem.IsVerticalReverse;
        previewPOVCamera.m_HorizontalAxis.m_InvertInput = InputSystem.IsHorizontalReverse;
        
        player?.EnablePlayerObject(false);
        previewPlayer?.EnablePlayerObject(true);
        ChangeUI();
    }

    private void OnDisable()
    {
        if (globalPlayersPos != null)
        {
            globalPlayersPos.position = tmpPos;
        }
        
        previewPlayer?.EnablePlayerObject(false);
        player?.EnablePlayerObject(true);
    }

    private void Update()
    {
        ChangeArrowColor();
    }
    
    private void ChangeArrowColor()
    {
        switch (InputSystem.MouseDeltaVertical)
        {
            case float y when y > 0f:
                arrowImages[Arrow.Up].color = arrowMoveColor;
                break;
            case float y when y < 0f:
                arrowImages[Arrow.Down].color = arrowMoveColor;
                break;
            default:
                arrowImages[Arrow.Up].color = Color.white;
                arrowImages[Arrow.Down].color = Color.white;
                break;
        }
        switch (InputSystem.MouseDeltaHorizontal)
        {
            case float x when x > 0f:
                arrowImages[Arrow.Right].color = arrowMoveColor;
                break;
            case float x when x < 0f:
                arrowImages[Arrow.Left].color = arrowMoveColor;
                break;
            default:
                arrowImages[Arrow.Right].color = Color.white;
                arrowImages[Arrow.Left].color = Color.white;
                break;
        }
    }

    private void ChangeUI()
    {
        slider.value = mouseSensitivity;
        sensitivityValue.text = (Mathf.Floor(mouseSensitivity * 100f) / 100f).ToString(CultureInfo.CurrentCulture);
        //���콺 ������ ���� ��ư �̹��� ����
        verticalSwitch.sprite = InputSystem.IsVerticalReverse ? onImage : offImage;
        horizontalSwitch.sprite = InputSystem.IsHorizontalReverse ? onImage : offImage;
    }

    /// <summary>
    /// �����̴� ������ ���� �ڵ� ȣ��
    /// </summary>
    public void ChangeSensitivity()
    {
        mouseSensitivity = slider.value;
        mouseSpeed = mouseSensitivity * mouseSpeedMultiplier;
        sensitivityValue.text = (Mathf.Floor(mouseSensitivity * 100f) / 100f).ToString(CultureInfo.CurrentCulture);
        
        playerPOVCamera.m_VerticalAxis.m_MaxSpeed = mouseSpeed;
        playerPOVCamera.m_HorizontalAxis.m_MaxSpeed = mouseSpeed;
        previewPOVCamera.m_VerticalAxis.m_MaxSpeed = mouseSpeed;
        previewPOVCamera.m_HorizontalAxis.m_MaxSpeed = mouseSpeed;
        
        SaveManager.Instance.SaveMouseSensitivity(mouseSensitivity);
    }
    
    public void ChangeSensitivityOnSlider()
    {
        ChangeSensitivity();
    }
    
    public void ChangeSensitivityOnInputField()
    {
        if (float.TryParse(sensitivityValue.text, out float value))
        {
            if (value < slider.minValue)
            {
                value = slider.minValue;
            }
            else if (value > slider.maxValue)
            {
                value = slider.maxValue;
            }
            slider.value = value;
            ChangeSensitivity();
        }
    }

    /// <summary>
    /// ���콺 ���Ϲ���
    /// </summary>
    public void MouseVerticalReverse()
    {
        InputSystem.ToggleVerticalReverse();
        bool isVerticalReverse = InputSystem.IsVerticalReverse;
        playerPOVCamera.m_VerticalAxis.m_InvertInput = !isVerticalReverse;
        previewPOVCamera.m_VerticalAxis.m_InvertInput = !isVerticalReverse;
        
        //��ư �̹��� ����(false�� ��� ����, true�� ��� �ʷ�)
        verticalSwitch.sprite = isVerticalReverse ? onImage : offImage;
    }

    /// <summary>
    /// ���콺 �¿� ����
    /// </summary>
    public void MouseHorizontalReverse()
    {
        InputSystem.ToggleHorizontalReverse();
        bool isHorizontalReverse = InputSystem.IsHorizontalReverse;
        playerPOVCamera.m_HorizontalAxis.m_InvertInput = isHorizontalReverse;
        previewPOVCamera.m_HorizontalAxis.m_InvertInput = isHorizontalReverse;
        
        //��ư �̹��� ����(false�� ��� ����, true�� ��� �ʷ�)
        horizontalSwitch.sprite = isHorizontalReverse ? onImage : offImage;
    }
}