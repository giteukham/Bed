using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// playerPrefs�� �̿��� ���� �ý���
/// </summary>
public class SaveManager : MonoBehaviour
{
    //���� ���۰� ���ÿ� ���� ������
    private void Awake()
    {
        MouseManagement.mouseSensitivity = LoadMouseSensitivity();
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
}
