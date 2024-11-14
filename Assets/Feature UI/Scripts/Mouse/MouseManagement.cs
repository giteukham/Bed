using System;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// ���� ��¥ : 2024-11-14 �ֹ���
/// </summary>
public class MouseManagement : MonoBehaviour
{
    [SerializeField] private Slider slider;
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


    //�⺻�� ����
    //���� 1f, ���Ϲ��� true(1), �¿���� false(0)

    /// <summary>
    /// ���콺 ���� ���� ����
    /// </summary>
    public static float mouseSensitivity = 1f;
    /// <summary>
    /// ���� ����Ǵ� ���콺 ����
    /// </summary>
    public static float mouseSpeed;

    private const float mouseSpeedMultiplier = 500f;        // ���콺 ���� ���

    public static void InitMouseSetting(float sensitivity, bool verticalReverse, bool horizontalReverse)
    {
        mouseSensitivity = sensitivity;
        mouseSpeed = mouseSensitivity * mouseSpeedMultiplier;
        Player.POVCamera.m_VerticalAxis.m_InvertInput = verticalReverse;
        Player.POVCamera.m_HorizontalAxis.m_InvertInput = horizontalReverse;
    }

    /// <summary>
    /// �������� �ƴ� ui ��Ҹ� �ʱ�ȭ ��(������ �ʱ�ȭ�� SaveManager���� ������)
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
        //���콺 ������ ���� ��ư �̹��� ����
        verticalSwitch.sprite = Player.POVCamera.m_VerticalAxis.m_InvertInput ? offImage : onImage;
        horizontalSwitch.sprite = Player.POVCamera.m_HorizontalAxis.m_InvertInput ? onImage : offImage;
    }

    /// <summary>
    /// �����̴� ������ ���� �ڵ� ȣ��
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
    /// ���콺 ���Ϲ���
    /// </summary>
    public void MouseVerticalReverse()
    {
        Player.POVCamera.m_VerticalAxis.m_InvertInput = !Player.POVCamera.m_VerticalAxis.m_InvertInput;
        SaveManager.Instance.SaveMouseVerticalReverse(Player.POVCamera.m_VerticalAxis.m_InvertInput);
        //��ư �̹��� ����(true�� ��� ����, false�� ��� �ʷ�)
        verticalSwitch.sprite = Player.POVCamera.m_VerticalAxis.m_InvertInput ? offImage : onImage;
    }

    /// <summary>
    /// ���콺 �¿� ����
    /// </summary>
    public void MouseHorizontalReverse()
    {
        Player.POVCamera.m_HorizontalAxis.m_InvertInput = !Player.POVCamera.m_HorizontalAxis.m_InvertInput;
        SaveManager.Instance.SaveMouseHorizontalReverse(Player.POVCamera.m_HorizontalAxis.m_InvertInput);

        //InputSystem.xBodyReverse = virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InvertInput ? -1 : 1;
        SaveManager.Instance.SaveXBodyReverse(Player.POVCamera.m_HorizontalAxis.m_InvertInput ? -1 : 1);
        //��ư �̹��� ����(false�� ��� ����, true�� ��� �ʷ�)
        horizontalSwitch.sprite = Player.POVCamera.m_HorizontalAxis.m_InvertInput ? onImage : offImage;
    }
}