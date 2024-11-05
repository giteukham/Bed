using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// playerPrefs를 이용한 저장 시스템
/// </summary>
public class SaveManager : MonoBehaviour
{
    //게임 시작과 동시에 설정 가져옴
    private void Awake()
    {
        MouseManagement.mouseSensitivity = LoadMouseSensitivity();
    }

    //마우스 감도 관련
    public void SaveMouseSensitivity(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();
    }
    public float LoadMouseSensitivity()
    {
        return PlayerPrefs.GetFloat("MouseSensitivity", 1f);
    }

    //마우스 상하반전 관련
    public void SaveMouseVerticalReverse(bool value)
    {
        PlayerPrefs.SetInt("MouseVerticalReverse", Convert.ToInt32(value));
        PlayerPrefs.Save();
    }
    public bool LoadMouseVerticalReverse()
    {
        return Convert.ToBoolean(PlayerPrefs.GetInt("MouseVerticalReverse", 1));
    }

    //마우스 좌우반전 관련
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
