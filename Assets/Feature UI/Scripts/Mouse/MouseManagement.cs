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
/// 수정 날짜 : 2024-11-18 최무령
/// </summary>
public enum Arrow
{
    Up,
    Down,
    Left,
    Right
}

//기본값 정리
//감도 1f, 상하반전 true(1), 좌우반전 false(0)
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
    /// 초록 스위치 이미지
    /// </summary>
    [SerializeField] private Sprite onImage;
    /// <summary>
    /// 빨강 스위치 이미지
    /// </summary>
    [SerializeField] private Sprite offImage;
    
    [Header("Arrow")] 
    [SerializedDictionary("Arrow", "GameObject")]
    [SerializeField] private SerializedDictionary<Arrow, GameObject> arrows;

    private Dictionary<Arrow, Image> arrowImages;
    private readonly Color arrowMoveColor = Color.gray;
    /// <summary>
    /// 마우스 감도 전역 변수
    /// </summary>
    public static float mouseSensitivity = 1f;
    /// <summary>
    /// 최종 적용되는 마우스 감도
    /// </summary>
    public static float mouseSpeed;

    private const float mouseSpeedMultiplier = 500f;        // 마우스 감도 상수

    private int verticalConstant = 1, horizontalConstant = 1;                    // 1은 정상, -1은 반전
    private static float mouseDeltaHorizontal, mouseDeltaVertical;
    public static float MouseDeltaHorizontal
    {
        get { return mouseDeltaHorizontal; }
        private set { mouseDeltaHorizontal = value; }
    }

    public static float MouseDeltaVertical
    {
        get { return mouseDeltaVertical; }
        private set { mouseDeltaVertical = value; }
    }
    private static bool isVerticalReverse = false, isHorizontalReverse = false;         // false는 정상, true는 반전

    public static void InitMouseSetting()
    {
        mouseSensitivity = SaveManager.Instance.LoadMouseSensitivity();
        mouseSpeed = mouseSensitivity * mouseSpeedMultiplier;
        isVerticalReverse = SaveManager.Instance.LoadMouseVerticalReverse();
        isHorizontalReverse = SaveManager.Instance.LoadMouseHorizontalReverse();

        previewPOVCamera = previewPlayer.POVCamera;
        playerPOVCamera = player?.POVCamera;

        previewPOVCamera.m_VerticalAxis.Value = 0f;
        previewPOVCamera.m_HorizontalAxis.Value = 0f;
        previewPOVCamera.m_VerticalAxis.m_InvertInput = !isVerticalReverse;
        previewPOVCamera.m_HorizontalAxis.m_InvertInput = isHorizontalReverse;
    }

    /// <summary>
    /// 실제값이 아닌 ui 요소만 초기화 함(실제값 초기화는 SaveManager에서 실행함)
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
        switch (MouseDeltaVertical)
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
        switch (MouseDeltaHorizontal)
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
        //마우스 반전에 따른 버튼 이미지 변경
        verticalSwitch.sprite = isVerticalReverse ? onImage : offImage;
        horizontalSwitch.sprite = isHorizontalReverse ? onImage : offImage;
    }

    /// <summary>
    /// 슬라이더 조절바 사용시 자동 호출
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
    /// 마우스 상하반전
    /// </summary>
    public void MouseVerticalReverse()
    {
        ToggleVerticalReverse();
        playerPOVCamera.m_VerticalAxis.m_InvertInput = !isVerticalReverse;
        previewPOVCamera.m_VerticalAxis.m_InvertInput = !isVerticalReverse;
        
        //버튼 이미지 변경(false일 경우 빨강, true일 경우 초록)
        verticalSwitch.sprite = isVerticalReverse ? onImage : offImage;
    }

    /// <summary>
    /// 마우스 좌우 반전
    /// </summary>
    public void MouseHorizontalReverse()
    {
        ToggleHorizontalReverse();
        playerPOVCamera.m_HorizontalAxis.m_InvertInput = isHorizontalReverse;
        previewPOVCamera.m_HorizontalAxis.m_InvertInput = isHorizontalReverse;
        
        //버튼 이미지 변경(false일 경우 빨강, true일 경우 초록)
        horizontalSwitch.sprite = isHorizontalReverse ? onImage : offImage;
    }

    private void ToggleVerticalReverse()
    {
        isVerticalReverse = !isVerticalReverse;
        verticalConstant = isVerticalReverse ? -1 : 1;
        SaveManager.Instance.SaveMouseVerticalReverse(isVerticalReverse);
    }

    private void ToggleHorizontalReverse()
    {
        isHorizontalReverse = !isHorizontalReverse;
        horizontalConstant = isHorizontalReverse ? -1 : 1;
        SaveManager.Instance.SaveMouseHorizontalReverse(isHorizontalReverse);
    }
}