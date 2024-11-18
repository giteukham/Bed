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
/// ���� ��¥ : 2024-11-14 �ֹ���
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
    [SerializeField] private TMP_Text sensitivityText;
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
    
    private static bool isVerticalReverse = true, isHorizontalReverse = false;

    private const float mouseSpeedMultiplier = 500f;        // ���콺 ���� ���

    public static void InitMouseSetting(float sensitivity, bool verticalReverse, bool horizontalReverse)
    {
        mouseSensitivity = sensitivity;
        mouseSpeed = mouseSensitivity * mouseSpeedMultiplier;
        isVerticalReverse = verticalReverse;
        isHorizontalReverse = horizontalReverse;
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
        playerPOVCamera = player.playerPOVCamera;
        
        previewPOVCamera.m_VerticalAxis.Value = 0f;
        previewPOVCamera.m_HorizontalAxis.Value = 0f;
        previewPOVCamera.m_VerticalAxis.m_InvertInput = isVerticalReverse;
        previewPOVCamera.m_HorizontalAxis.m_InvertInput = isHorizontalReverse;
        
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
        int invertVerticalConstant = isVerticalReverse ? 1 : -1;
        int invertHorizontalConstant = isHorizontalReverse ? -1 : 1;

        switch (InputSystem.MouseDeltaY * invertVerticalConstant)
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
        switch (InputSystem.MouseDeltaX * invertHorizontalConstant)
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
        sensitivityText.text = mouseSensitivity.ToString(CultureInfo.CurrentCulture);
        //���콺 ������ ���� ��ư �̹��� ����
        verticalSwitch.sprite = isVerticalReverse ? offImage : onImage;
        horizontalSwitch.sprite = isHorizontalReverse ? onImage : offImage;
    }

    /// <summary>
    /// �����̴� ������ ���� �ڵ� ȣ��
    /// </summary>
    public void ChangeSensitivity()
    {
        mouseSensitivity = slider.value;
        mouseSpeed = mouseSensitivity * mouseSpeedMultiplier;
        sensitivityText.text = mouseSensitivity.ToString(CultureInfo.CurrentCulture);
        
        playerPOVCamera.m_VerticalAxis.m_MaxSpeed = mouseSpeed;
        playerPOVCamera.m_HorizontalAxis.m_MaxSpeed = mouseSpeed;
        previewPOVCamera.m_VerticalAxis.m_MaxSpeed = mouseSpeed;
        previewPOVCamera.m_HorizontalAxis.m_MaxSpeed = mouseSpeed;
        
        SaveManager.Instance.SaveMouseSensitivity(mouseSensitivity);
    }

    /// <summary>
    /// ���콺 ���Ϲ���
    /// </summary>
    public void MouseVerticalReverse()
    {
        isVerticalReverse = !isVerticalReverse;
        playerPOVCamera.m_VerticalAxis.m_InvertInput = isVerticalReverse;
        previewPOVCamera.m_VerticalAxis.m_InvertInput = isVerticalReverse;
        
        SaveManager.Instance.SaveMouseVerticalReverse(isVerticalReverse);
        //��ư �̹��� ����(true�� ��� ����, false�� ��� �ʷ�)
        verticalSwitch.sprite = isVerticalReverse ? offImage : onImage;
    }

    /// <summary>
    /// ���콺 �¿� ����
    /// </summary>
    public void MouseHorizontalReverse()
    {
        isHorizontalReverse = !isHorizontalReverse;
        playerPOVCamera.m_HorizontalAxis.m_InvertInput = isHorizontalReverse;
        previewPOVCamera.m_HorizontalAxis.m_InvertInput = isHorizontalReverse;
        
        SaveManager.Instance.SaveMouseHorizontalReverse(isHorizontalReverse);

        //InputSystem.xBodyReverse = virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InvertInput ? -1 : 1;
        SaveManager.Instance.SaveXBodyReverse(isHorizontalReverse ? -1 : 1);
        //��ư �̹��� ����(false�� ��� ����, true�� ��� �ʷ�)
        horizontalSwitch.sprite = isHorizontalReverse ? onImage : offImage;
    }
}