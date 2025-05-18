using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// playerPrefs를 이용한 저장 시스템
/// </summary>
public class SaveManager : MonoSingleton<SaveManager>
{

    //게임 시작과 동시에 저장되어 있던 실제값 설정 가져옴
    private void Awake()
    {
        DontDestroyOnLoad(this);
        //프레임 관련 변수
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = LoadFrameRate();

        //해상도와 프레임은 자동으로 적용됨
        //하지만 카메라의 viewport Rect는 적용되지 않으므로 수동으로 불러옴
        //cam.rect = LoadCamRect();
    }

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
        return Convert.ToBoolean(PlayerPrefs.GetInt("MouseVerticalReverse", 0));
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

    //화면 풀스크린 여부 관련
    public void SaveIsWindowedScreen(bool value)
    {
        PlayerPrefs.SetInt("IsWindowedScreen", Convert.ToInt32(value));
        PlayerPrefs.Save();
    }
    public bool LoadIsWindowedScreen()
    {
        return Convert.ToBoolean(PlayerPrefs.GetInt("IsWindowedScreen", 0));
    }

    //camRect 관련
    public void SaveCamRect(float value1, float value2, float value3, float value4)
    {
        PlayerPrefs.SetFloat("CamX", value1);
        PlayerPrefs.SetFloat("CamY", value2);
        PlayerPrefs.SetFloat("CamWidth", value3);
        PlayerPrefs.SetFloat("CamHeight", value4);
        PlayerPrefs.Save();
    }

    public Rect LoadCamRect()
    {
        Rect rect = new Rect
            (
            PlayerPrefs.GetFloat("CamX", 0), PlayerPrefs.GetFloat("CamY", 0),
            PlayerPrefs.GetFloat("CamWidth", 1), PlayerPrefs.GetFloat("CamHeight", 1)
            );
        return rect;
    }

    //BlinkEffect StartPoint 관련
    public void SaveStartPoint(float value)
    {
         PlayerPrefs.SetFloat("BlinkEffectStartPoint", value);
         PlayerPrefs.Save();
    }

    public float LoadStartPoint()
    {
        return PlayerPrefs.GetFloat("BlinkEffectStartPoint", BlinkEffect.BLINK_START_POINT_INIT);
    }

    //프레임 레이트 관련
    public void SaveFrameRate(int value)
    {
        PlayerPrefs.SetInt("FrameRate", value);
    }
    public int LoadFrameRate()
    {
        return PlayerPrefs.GetInt("FrameRate", 60);
    }

    //lastApplyObject 관련
    public void SaveLastApplyObject(int value)
    {
        //0 : 드롭다운
        //1 : 인풋필드Width
        //2 : 인풋필드Height
        //3 : 마우스 드래그
        PlayerPrefs.SetInt("SaveLastApplyObject", value);
    }

    public int LoadLastApplyObject()
    {
        return PlayerPrefs.GetInt("SaveLastApplyObject", 0);
    }
    
    public void SaveDeadZoneValue(float value)
    {
        PlayerPrefs.SetFloat("DeadZoneValue", value);
        PlayerPrefs.Save();
    }
    
    public float LoadDeadZoneValue()
    {
        return PlayerPrefs.GetFloat("DeadZoneValue", 0.5f);
    }

    public void SavePixelationFactor(float value)
    {
        PlayerPrefs.SetFloat("PixelationFactor", value);
    }

    public float LoadPixelationFactor()
    {
        return PlayerPrefs.GetFloat("PixelationFactor", 0.25f / (Display.main.systemWidth / 1920f));
    }
    //마지막 플레이 시간 저장 관련
    public void SaveLastPlayedTime(string value)
    {
        PlayerPrefs.SetString("LastPlayedTime", value);
        PlayerPrefs.Save();
    }

    public string LoadLastPlayedTime()
    {
        return PlayerPrefs.GetString("LastPlayedTime", "20000101000000");
    }

    public float LoadMasterVolume()
    {
        return PlayerPrefs.GetFloat("MasterVolume", 1);
    }

    public void SaveMasterVolume(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

    public float LoadGimmickVolume()
    {
        return PlayerPrefs.GetFloat("GimmickVolume", 1);
    }

    public void SaveGimmickVolume(float value)
    {
        PlayerPrefs.SetFloat("GimmickVolume", value);
        PlayerPrefs.Save();
    }

    public float LoadPlayerVolume()
    {
        return PlayerPrefs.GetFloat("PlayerVolume", 1);
    }

    public void SavePlayerVolume(float value)
    {
        PlayerPrefs.SetFloat("PlayerVolume", value);
        PlayerPrefs.Save();
    }

    public void SaveResolutionSettings(ResolutionSettingsDTO data)
    {
        PlayerPrefs.SetInt("ResolutionWidth", data.ResolutionWidth);
        PlayerPrefs.SetInt("ResolutionHeight", data.ResolutionHeight);
        PlayerPrefs.SetInt("FrameRate", data.FrameRate);
        PlayerPrefs.SetInt("IsWindowed", data.IsWindowed ? 1 : 0);
        PlayerPrefs.SetFloat("ScreenBrightness", data.ScreenBrightness);
        PlayerPrefs.Save();
    }

    public ResolutionSettingsDTO LoadResolutionSettings()
    {
        return new ResolutionSettingsDTO
        {
            ResolutionWidth = PlayerPrefs.GetInt("ResolutionWidth", Screen.currentResolution.width),
            ResolutionHeight = PlayerPrefs.GetInt("ResolutionHeight", Screen.currentResolution.height),
            FrameRate = PlayerPrefs.GetInt("FrameRate", 60),
            IsWindowed = PlayerPrefs.GetInt("IsWindowed", 0) == 1,
            ScreenBrightness = PlayerPrefs.GetFloat("ScreenBrightness", 0f)
        };
    }
}
