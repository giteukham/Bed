using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// playerPrefs를 이용한 저장 시스템
/// </summary>
public class SaveManager : MonoSingleton<SaveManager>
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerPrefs.DeleteAll();
        }
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

    //화면 해상도 관련
    public void SaveResolution(int value1, int value2)
    {
        //PlayerPrefs.SetString("Resolution", value1 + " " + value2);

        PlayerPrefs.SetInt("ResolutionWidth", value1);
        PlayerPrefs.SetInt("ResolutionHeight", value2);

        PlayerPrefs.Save();
    }
    public void LoadResolution(out int value1, out int value2)
    {
        value1 = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        value2 = PlayerPrefs.GetInt("ResolutionHeight", 1080);
        //return PlayerPrefs.GetString("Resolution", "1920 1080");
    }

    //화면 풀스크린 여부 관련
    public void SaveIsFullScreen(bool value)
    {
        PlayerPrefs.SetInt("IsFullScreen", Convert.ToInt32(value));
        PlayerPrefs.Save();
    }
    public bool LoadIsFullScreen()
    {
        return Convert.ToBoolean(PlayerPrefs.GetInt("IsFullScreen", 1));
    }
    
    public void SaveDeadZoneValue(float value)
    {
        PlayerPrefs.SetFloat("DeadZoneValue", value);
        PlayerPrefs.Save();
    }
    
    public float LoadDeadZoneValue()
    {
        return PlayerPrefs.GetFloat("DeadZoneValue", 0.1f);
    }
}
