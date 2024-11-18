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
/// 수정 날짜 : 2024-11-14 최무령
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
    [SerializeField] private TMP_Text sensitivityText;
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
    
    private static bool isVerticalReverse = true, isHorizontalReverse = false;

    private const float mouseSpeedMultiplier = 500f;        // 마우스 감도 상수

    public static void InitMouseSetting(float sensitivity, bool verticalReverse, bool horizontalReverse)
    {
        mouseSensitivity = sensitivity;
        mouseSpeed = mouseSensitivity * mouseSpeedMultiplier;
        isVerticalReverse = verticalReverse;
        isHorizontalReverse = horizontalReverse;
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
        //마우스 반전에 따른 버튼 이미지 변경
        verticalSwitch.sprite = isVerticalReverse ? offImage : onImage;
        horizontalSwitch.sprite = isHorizontalReverse ? onImage : offImage;
    }

    /// <summary>
    /// 슬라이더 조절바 사용시 자동 호출
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
    /// 마우스 상하반전
    /// </summary>
    public void MouseVerticalReverse()
    {
        isVerticalReverse = !isVerticalReverse;
        playerPOVCamera.m_VerticalAxis.m_InvertInput = isVerticalReverse;
        previewPOVCamera.m_VerticalAxis.m_InvertInput = isVerticalReverse;
        
        SaveManager.Instance.SaveMouseVerticalReverse(isVerticalReverse);
        //버튼 이미지 변경(true일 경우 빨강, false일 경우 초록)
        verticalSwitch.sprite = isVerticalReverse ? offImage : onImage;
    }

    /// <summary>
    /// 마우스 좌우 반전
    /// </summary>
    public void MouseHorizontalReverse()
    {
        isHorizontalReverse = !isHorizontalReverse;
        playerPOVCamera.m_HorizontalAxis.m_InvertInput = isHorizontalReverse;
        previewPOVCamera.m_HorizontalAxis.m_InvertInput = isHorizontalReverse;
        
        SaveManager.Instance.SaveMouseHorizontalReverse(isHorizontalReverse);

        //InputSystem.xBodyReverse = virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InvertInput ? -1 : 1;
        SaveManager.Instance.SaveXBodyReverse(isHorizontalReverse ? -1 : 1);
        //버튼 이미지 변경(false일 경우 빨강, true일 경우 초록)
        horizontalSwitch.sprite = isHorizontalReverse ? onImage : offImage;
    }
}