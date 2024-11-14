using System;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// 수정 날짜 : 2024-11-14 최무령
/// </summary>
public class MouseManagement : MonoBehaviour
{
    [SerializeField] private Slider slider;
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


    //기본값 정리
    //감도 1f, 상하반전 true(1), 좌우반전 false(0)

    /// <summary>
    /// 마우스 감도 전역 변수
    /// </summary>
    public static float mouseSensitivity = 1f;
    /// <summary>
    /// 최종 적용되는 마우스 감도
    /// </summary>
    public static float mouseSpeed;

    private const float mouseSpeedMultiplier = 500f;        // 마우스 감도 상수

    public static void InitMouseSetting(float sensitivity, bool verticalReverse, bool horizontalReverse)
    {
        mouseSensitivity = sensitivity;
        mouseSpeed = mouseSensitivity * mouseSpeedMultiplier;
        Player.POVCamera.m_VerticalAxis.m_InvertInput = verticalReverse;
        Player.POVCamera.m_HorizontalAxis.m_InvertInput = horizontalReverse;
    }

    /// <summary>
    /// 실제값이 아닌 ui 요소만 초기화 함(실제값 초기화는 SaveManager에서 실행함)
    /// </summary>
    private void OnEnable()
    {
        Player.POVCamera.m_VerticalAxis.m_InputAxisName = "Mouse Y";
        Player.POVCamera.m_HorizontalAxis.m_InputAxisName = "Mouse X";
        ChangeUI();
    }

    private void OnDisable()
    {
        Player.POVCamera.m_VerticalAxis.m_InputAxisName = "";
        Player.POVCamera.m_HorizontalAxis.m_InputAxisName = "";
    }

    private void ChangeUI()
    {
        slider.value = mouseSensitivity;
        //마우스 반전에 따른 버튼 이미지 변경
        verticalSwitch.sprite = Player.POVCamera.m_VerticalAxis.m_InvertInput ? offImage : onImage;
        horizontalSwitch.sprite = Player.POVCamera.m_HorizontalAxis.m_InvertInput ? onImage : offImage;
    }

    /// <summary>
    /// 슬라이더 조절바 사용시 자동 호출
    /// </summary>
    public void ChangeSensitivity()
    {
        mouseSensitivity = slider.value;
        mouseSpeed = mouseSensitivity * mouseSpeedMultiplier;

        Player.POVCamera.m_VerticalAxis.m_MaxSpeed = mouseSpeed;
        Player.POVCamera.m_HorizontalAxis.m_MaxSpeed = mouseSpeed;
        SaveManager.Instance.SaveMouseSensitivity(mouseSensitivity);
    }

    /// <summary>
    /// 마우스 상하반전
    /// </summary>
    public void MouseVerticalReverse()
    {
        Player.POVCamera.m_VerticalAxis.m_InvertInput = !Player.POVCamera.m_VerticalAxis.m_InvertInput;
        SaveManager.Instance.SaveMouseVerticalReverse(Player.POVCamera.m_VerticalAxis.m_InvertInput);
        //버튼 이미지 변경(true일 경우 빨강, false일 경우 초록)
        verticalSwitch.sprite = Player.POVCamera.m_VerticalAxis.m_InvertInput ? offImage : onImage;
    }

    /// <summary>
    /// 마우스 좌우 반전
    /// </summary>
    public void MouseHorizontalReverse()
    {
        Player.POVCamera.m_HorizontalAxis.m_InvertInput = !Player.POVCamera.m_HorizontalAxis.m_InvertInput;
        SaveManager.Instance.SaveMouseHorizontalReverse(Player.POVCamera.m_HorizontalAxis.m_InvertInput);

        //InputSystem.xBodyReverse = virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InvertInput ? -1 : 1;
        SaveManager.Instance.SaveXBodyReverse(Player.POVCamera.m_HorizontalAxis.m_InvertInput ? -1 : 1);
        //버튼 이미지 변경(false일 경우 빨강, true일 경우 초록)
        horizontalSwitch.sprite = Player.POVCamera.m_HorizontalAxis.m_InvertInput ? onImage : offImage;
    }
}