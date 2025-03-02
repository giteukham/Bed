using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
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
    object Validate(object value);
}

public class ResolutionWidthRule : IResolutionRule
{
    int minWidth = Display.main.systemWidth / 4;
    int maxWidth = Display.main.systemWidth;
    public object Validate(object value)
    {
        int width = (int)value;
        return Mathf.Clamp(width, minWidth, maxWidth);
    }
}

public class ResolutionHeightRule : IResolutionRule
{
    int minHeight = Display.main.systemHeight / 4;
    int maxHeight = Display.main.systemHeight;
    public object Validate(object value)
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

    private float ratio = (float)Display.main.systemWidth / Display.main.systemHeight;

    public ResolutionSettingsData()
    { 
        ResolutionSettingType[] keys = (ResolutionSettingType[])Enum.GetValues(typeof(ResolutionSettingType));
        foreach (ResolutionSettingType key in keys)
        {
            settings[key] = null;
        }

        validationRules[ResolutionSettingType.ResolutionWidth] = new ResolutionWidthRule();
        validationRules[ResolutionSettingType.ResolutionHeight] = new ResolutionHeightRule();
    }

    private T GetSetting<T>(ResolutionSettingType type) => (T)settings[type];
 
    private T SetSetting<T>(ResolutionSettingType type, T value)
    {
        if (validationRules.ContainsKey(type)) // 해상도 높이와 넓이만 규칙 적용됨
        {
            value = (T)validationRules[type].Validate(value);
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

            int targetHieght = (int)Mathf.Round(validationValue / ratio);
            if (ResolutionHeight != targetHieght)
                ResolutionHeight = targetHieght;
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
                ResolutionWidth = targetWidth;
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
        set => SetSetting(ResolutionSettingType.IsWindowed, value);
    }

    public float ScreenBrightness
    {
        get => GetSetting<float>(ResolutionSettingType.ScreenBrightness);
        set => SetSetting(ResolutionSettingType.ScreenBrightness, value);
    }
    
    public void ChangeData(ResolutionSettingsDTO data)
    {
        SetSetting(ResolutionSettingType.ResolutionWidth, data.ResolutionWidth);
        SetSetting(ResolutionSettingType.ResolutionHeight, data.ResolutionHeight);
        SetSetting(ResolutionSettingType.FrameRate, data.FrameRate);
        SetSetting(ResolutionSettingType.IsWindowed, data.IsWindowed);
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
    
    private readonly string path = "Menu UI/Resolution Settings Screen/";

    public void InitResolutionSettings() // UIManager.Awake()에서 호출
    {
        Assert.IsNotNull(resolutionSettingsPanel, $"{path}Resolution Settings Panel is null");
        Assert.IsNotNull(resolutionPreviewPanel, $"{path}Resolution Preview Panel is null");
        
        previewData =  new ResolutionSettingsData();
        backupData = SaveManager.Instance.LoadResolutionSettings();
        
        resolutionSettingsPanel?.Initialize(previewData, backupData);
        resolutionPreviewPanel?.Initialize(previewData, backupData);

        previewData.ChangeData(backupData);
        
        Screen.SetResolution(backupData.ResolutionWidth, backupData.ResolutionHeight, backupData.IsWindowed);
        Application.targetFrameRate = backupData.FrameRate;
        if (QualitySettings.vSyncCount != 1) QualitySettings.vSyncCount = 1;
        PlayerConstant.pixelationFactor = 0.25f / (backupData.ResolutionWidth / 1920f); //픽셀레이션 값 조절
    }

    // private void OnEnable()
    // {
    //     previewData.ChangeData(backupData);
    // }

    public void ApplyResolutionSettings() // ApplyButton 오브젝트에 할당
    {
        backupData.ChangeData(previewData);
        Screen.SetResolution(backupData.ResolutionWidth, backupData.ResolutionHeight, backupData.IsWindowed);
        Application.targetFrameRate = backupData.FrameRate;
        PlayerConstant.pixelationFactor = 0.25f / (backupData.ResolutionWidth / 1920f); //픽셀레이션 값 조절
        resolutionSettingsPanel.ApplyBrightness(backupData.ScreenBrightness);
        SaveManager.Instance.SaveResolutionSettings(backupData); // 해상도 설정 저장
    }
}