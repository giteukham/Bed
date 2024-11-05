using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// playerPrefs�� �̿��� ���� �ý���
/// </summary>
public class SaveManager : MonoBehaviour
{
    /// <summary>
    /// �÷��̾��� ����� ī�޶� ����
    /// </summary>
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    //���� ���۰� ���ÿ� ����Ǿ� �ִ� ������ ���� ������
    private void Awake()
    {
        MouseManagement.mouseSensitivity = LoadMouseSensitivity();
        virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InvertInput = LoadMouseVerticalReverse();
        virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InvertInput = LoadMouseHorizontalReverse();
        InputSystem.xBodyReverse = LoadXBodyReverse();
    }

    //���콺 ���� ����
    public void SaveMouseSensitivity(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();
    }
    public float LoadMouseSensitivity()
    {
        return PlayerPrefs.GetFloat("MouseSensitivity", 1f);
    }

    //���콺 ���Ϲ��� ����
    public void SaveMouseVerticalReverse(bool value)
    {
        PlayerPrefs.SetInt("MouseVerticalReverse", Convert.ToInt32(value));
        PlayerPrefs.Save();
    }
    public bool LoadMouseVerticalReverse()
    {
        return Convert.ToBoolean(PlayerPrefs.GetInt("MouseVerticalReverse", 1));
    }

    //���콺 �¿���� ����
    public void SaveMouseHorizontalReverse(bool value)
    {
        PlayerPrefs.SetInt("MouseHorizontalReverse", Convert.ToInt32(value));
        PlayerPrefs.Save();
    }
    public bool LoadMouseHorizontalReverse()
    {
        return Convert.ToBoolean(PlayerPrefs.GetInt("MouseHorizontalReverse", 0));
    }
    public void SaveXBodyReverse(int value)
    {
        InputSystem.xBodyReverse = value;
        PlayerPrefs.SetInt("XBodyReverse", value);
        PlayerPrefs.Save();
    }
    public int LoadXBodyReverse()
    {
        return PlayerPrefs.GetInt("XBodyReverse", 1);
    }
}
