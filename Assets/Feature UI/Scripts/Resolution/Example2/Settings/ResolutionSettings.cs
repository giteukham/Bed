using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.Serialization;


public enum ResolutionSettingType
{
    ResolutionWidth,
    ResolutionHeight,
    FrameRate,
    IsWindowed,
    ScreenBrightness
}

public interface IResolutionRule
{
    object Validate(object value, int maxWidth, int maxHeight, int minHeight, int minWidth);
}

public class ResolutionWidthRule : IResolutionRule
{    public object Validate(object value, int maxWidth, int maxHeight, int minHeight, int minWidth)
    {
        int width = (int)value;
        return Mathf.Clamp(width, minWidth, maxWidth);
    }
}

public class ResolutionHeightRule : IResolutionRule
{
    public object Validate(object value, int maxWidth, int maxHeight, int minHeight, int minWidth)
    {
        int height = (int)value;
        return Mathf.Clamp(height, minHeight, maxHeight);
    }
}

public class ResolutionSettingsDTO // Data Transfer Object
{
    public int ResolutionWidth;
    public int ResolutionHeight;
    public int FrameRate;
    public bool IsWindowed;
    public float ScreenBrightness;

    public ResolutionSettingsDTO() {}

    public ResolutionSettingsDTO(ResolutionSettingsData source)
    {
        ResolutionWidth = source.ResolutionWidth;
        ResolutionHeight = source.ResolutionHeight;
        FrameRate = source.FrameRate;
        IsWindowed = source.IsWindowed;
        ScreenBrightness = source.ScreenBrightness;
    }

    public void ChangeData(ResolutionSettingsData data)
    {
        ResolutionWidth = data.ResolutionWidth;
        ResolutionHeight = data.ResolutionHeight;
        FrameRate = data.FrameRate;
        IsWindowed = data.IsWindowed;
        ScreenBrightness = data.ScreenBrightness;
    }
}

public class ResolutionSettingsData : INotifyPropertyChanged
{
    // 설정 값 저장 Dictionary
    private readonly Dictionary<ResolutionSettingType, object> settings = new();
    // 규칙 저장 Dictionary
    private readonly Dictionary<ResolutionSettingType, IResolutionRule> validationRules = new();

    public int fullScreenMaxWidth, fullScreenMaxHeight, fullScreenMinWidth, fullScreenMinHeight, windowedMaxWidth, windowedMaxHeight, windowedMinWidth, windowedMinHeight;

    public int maxWidth, maxHeight, minWidth, minHeight;
    private float fullScreenRatio, windowedRatio; // 창모드 비율은 무조건 16:9
    public float ratio;

    public ResolutionSettingsData()
    { 
        ResolutionSettingType[] keys = (ResolutionSettingType[])Enum.GetValues(typeof(ResolutionSettingType));
        foreach (ResolutionSettingType key in keys)
        {
            settings[key] = null;
        }

        validationRules[ResolutionSettingType.ResolutionWidth] = new ResolutionWidthRule();
        validationRules[ResolutionSettingType.ResolutionHeight] = new ResolutionHeightRule();

        fullScreenMaxWidth = Display.main.systemWidth;
        fullScreenMaxHeight = Display.main.systemHeight;
        fullScreenMinWidth = fullScreenMaxWidth / 4;
        fullScreenMinHeight = fullScreenMaxHeight / 4;

        fullScreenRatio = (float)Math.Round((float)fullScreenMaxWidth / fullScreenMaxHeight, 3);
        windowedRatio = (float)Math.Round(16f / 9, 3);

        if (fullScreenRatio == windowedRatio) // 16:9일때
        {
            windowedMaxWidth = fullScreenMaxWidth;
            windowedMaxHeight = fullScreenMaxHeight;
            windowedMinWidth = fullScreenMinWidth;
            windowedMinHeight = fullScreenMinHeight;
        }
        else if (fullScreenRatio > windowedRatio) // 16:9보다 width가 더 넒을때
        {
            int conversionWidth = fullScreenMaxHeight / 9 * 16;
            windowedMaxWidth = conversionWidth;
            windowedMaxHeight = fullScreenMaxHeight;
            windowedMinWidth = conversionWidth / 4;
            windowedMinHeight = fullScreenMinHeight;
        }
        else if (fullScreenRatio < windowedRatio) // 16:9보다 height가 더 높을때
        {
            int conversionHeight = fullScreenMaxWidth / 16 * 9;
            windowedMaxWidth = fullScreenMaxWidth;
            windowedMaxHeight = conversionHeight;
            windowedMinWidth = fullScreenMinWidth;
            windowedMinHeight = conversionHeight / 4;
        }

        // Debug.Log("모니터 해상도 : " + fullScreenMaxWidth + " x " + fullScreenMaxHeight);
        // Debug.Log("전체 화면 일때 최대 : " + fullScreenMaxWidth + " x " + fullScreenMaxHeight);
        // Debug.Log("전체 화면 일때 최소 : " + fullScreenMinWidth + " x " + fullScreenMinHeight);
        // Debug.Log("창모드 일때 최대 : " + windowedMaxWidth + " x " + windowedMaxHeight);
        // Debug.Log("창모드 일때 최소 : " + windowedMinWidth + " x " + windowedMinHeight);
    }

    private T GetSetting<T>(ResolutionSettingType type) => (T)settings[type];
 
    private T SetSetting<T>(ResolutionSettingType type, T value)
    {
        if (validationRules.ContainsKey(type)) // 해상도 높이와 넓이만 규칙 적용됨
        {
            value = (T)validationRules[type].Validate(value, maxWidth, maxHeight, minHeight, minWidth);
        }
        settings[type] = value;
        OnPropertyChanged(type.ToString());
        if (settings[type] != null) return value; // 규칙 적용 후 값 반환
        else return default;
    }

    public int ResolutionWidth
    {
        get => GetSetting<int>(ResolutionSettingType.ResolutionWidth);
        set
        {
            int validationValue = SetSetting(ResolutionSettingType.ResolutionWidth, value);
            validationValue = validationValue == default ? value : validationValue; // 반환 값이 없으면 그대로 : 있으면 그 값으로 설정
            //Debug.Log(validationValue);
            int targetHieght = (int)Mathf.Round(validationValue / ratio);
            if (ResolutionHeight != targetHieght)
            {
                ResolutionHeight = targetHieght;
            }
                
        }
    }

    public int ResolutionHeight
    {
        get => GetSetting<int>(ResolutionSettingType.ResolutionHeight);
        set
        {
            int validationValue = SetSetting(ResolutionSettingType.ResolutionHeight, value);
            validationValue = validationValue == default ? value : validationValue; // 반환 값이 없으면 그대로 : 있으면 그 값으로 설정

            int targetWidth = (int)Mathf.Round(validationValue * ratio);
            if (ResolutionWidth != targetWidth)
            {
                ResolutionWidth = targetWidth;
            }
                
        }
    }

    public int FrameRate
    {
        get => GetSetting<int>(ResolutionSettingType.FrameRate);
        set => SetSetting(ResolutionSettingType.FrameRate, value);
    }

    public bool IsWindowed
    {
        get => GetSetting<bool>(ResolutionSettingType.IsWindowed);
        set
        {
            if (value) 
            {
                ratio = windowedRatio;
                maxWidth = windowedMaxWidth;
                maxHeight = windowedMaxHeight;
                minWidth = windowedMinWidth;
                minHeight = windowedMinHeight;
            }
            else
            {
                ratio = fullScreenRatio;
                maxWidth = fullScreenMaxWidth;
                maxHeight = fullScreenMaxHeight;
                minWidth = fullScreenMinWidth;
                minHeight = fullScreenMinHeight;
            }
            SetSetting(ResolutionSettingType.IsWindowed, value);
        }
    }

    public float ScreenBrightness
    {
        get => GetSetting<float>(ResolutionSettingType.ScreenBrightness);
        set => SetSetting(ResolutionSettingType.ScreenBrightness, value);
    }
    
    public void ChangeData(ResolutionSettingsDTO data)
    {
        IsWindowed = data.IsWindowed;
        SetSetting(ResolutionSettingType.ResolutionWidth, data.ResolutionWidth);
        ResolutionHeight = data.ResolutionHeight;
        SetSetting(ResolutionSettingType.FrameRate, data.FrameRate);
        SetSetting(ResolutionSettingType.ScreenBrightness, data.ScreenBrightness);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class ResolutionSettings : MonoBehaviour
{
    [SerializeField]
    private ResolutionSettingsPanel resolutionSettingsPanel;
    
    [SerializeField]
    private ResolutionPreviewPanel resolutionPreviewPanel;
    
    private ResolutionSettingsDTO backupData;
    private ResolutionSettingsData previewData;
    
    public ResolutionSettingsData PreviewData => previewData;
    public ResolutionSettingsDTO BackupData => backupData;
    [SerializeField] private Camera cam;
    private readonly string path = "Menu UI/Resolution Settings Screen/";

    public void InitResolutionSettings() // UIManager.Awake()에서 호출
    {
        Assert.IsNotNull(resolutionSettingsPanel, $"{path}Resolution Settings Panel is null");
        Assert.IsNotNull(resolutionPreviewPanel, $"{path}Resolution Preview Panel is null");
        previewData =  new ResolutionSettingsData();
        backupData = SaveManager.Instance.LoadResolutionSettings();
        
        previewData.ChangeData(backupData);
        
        resolutionSettingsPanel?.Initialize(previewData, backupData);
        resolutionPreviewPanel?.Initialize(previewData, backupData);
        
        Screen.SetResolution(backupData.ResolutionWidth, backupData.ResolutionHeight, !backupData.IsWindowed);
        Application.targetFrameRate = backupData.FrameRate;
        if (QualitySettings.vSyncCount != 1) QualitySettings.vSyncCount = 1;

        Rect rect = new Rect(0, 0, 1, 1);
        float ratio = (float)Math.Round((float)backupData.ResolutionWidth / backupData.ResolutionHeight, 3);
        float standardRatio = (float)Math.Round(16f / 9, 3);
        if (ratio == standardRatio)
        {
            BlinkEffect.StartPoint = BlinkEffect.BLINK_START_POINT_INIT;
        }
        else if (ratio > standardRatio)
        {
            float temp1 = 9 / (float)backupData.ResolutionHeight;
            float temp2 = temp1 * backupData.ResolutionWidth;
            rect.width = 16 / temp2;
            rect.x = (1 - rect.width) / 2;
            BlinkEffect.StartPoint = BlinkEffect.BLINK_START_POINT_INIT;
        }
        else if (ratio < standardRatio)
        {
            float temp1 = 9 / (float)backupData.ResolutionHeight;
            float temp2 = temp1 * backupData.ResolutionWidth;
            rect.height = temp2 / 16;
            rect.y = (1 - rect.height) / 2;
            float startPointConversionValue = Mathf.Lerp(BlinkEffect.BLINK_START_POINT_INIT + 0.13f, BlinkEffect.BLINK_START_POINT_INIT, (ratio - 1f) / (1.77f - 1f));
            BlinkEffect.StartPoint = startPointConversionValue;
        }
        cam.rect = rect;
        PlayerConstant.pixelationFactor = 0.25f / (backupData.ResolutionWidth / 1920f); //픽셀레이션 값 조절
        resolutionSettingsPanel.ApplyBrightness(backupData.ScreenBrightness);
        resolutionPreviewPanel.SetResolutionText(backupData.ResolutionWidth, backupData.ResolutionHeight, backupData.FrameRate, false);
    }

    public void ApplyResolutionSettings() // ApplyButton 오브젝트에 할당
    {
        backupData.ChangeData(previewData);
        Screen.SetResolution(backupData.ResolutionWidth, backupData.ResolutionHeight, !backupData.IsWindowed);
        Application.targetFrameRate = backupData.FrameRate;

        Rect rect = new Rect(0, 0, 1, 1);
        float ratio = (float)Math.Round((float)backupData.ResolutionWidth / backupData.ResolutionHeight, 3);
        float standardRatio = (float)Math.Round(16f / 9, 3);
        if (ratio == standardRatio)
        {
            BlinkEffect.StartPoint = BlinkEffect.BLINK_START_POINT_INIT;
        }
        else if (ratio > standardRatio)
        {
            float temp1 = 9 / (float)backupData.ResolutionHeight;
            float temp2 = temp1 * backupData.ResolutionWidth;
            rect.width = 16 / temp2;
            rect.x = (1 - rect.width) / 2;
            BlinkEffect.StartPoint = BlinkEffect.BLINK_START_POINT_INIT;
        }
        else if (ratio < standardRatio)
        {
            float temp1 = 9 / (float)backupData.ResolutionHeight;
            float temp2 = temp1 * backupData.ResolutionWidth;
            rect.height = temp2 / 16;
            rect.y = (1 - rect.height) / 2;
            float startPointConversionValue = Mathf.Lerp(BlinkEffect.BLINK_START_POINT_INIT + 0.13f, BlinkEffect.BLINK_START_POINT_INIT, (ratio - 1f) / (1.77f - 1f));
            BlinkEffect.StartPoint = startPointConversionValue;
        }
        cam.rect = rect;
        PlayerConstant.pixelationFactor = 0.25f / (backupData.ResolutionWidth / 1920f); //픽셀레이션 값 조절
        resolutionSettingsPanel.ApplyBrightness(backupData.ScreenBrightness);
        resolutionPreviewPanel.SetResolutionText(backupData.ResolutionWidth, backupData.ResolutionHeight, backupData.FrameRate, false);
        SaveManager.Instance.SaveResolutionSettings(backupData); // 해상도 설정 저장
    }
}