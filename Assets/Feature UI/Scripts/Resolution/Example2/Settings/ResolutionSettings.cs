using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

public class ResolutionSettingsData : INotifyPropertyChanged
{
    private int resolutionWidth = 1920;
    private int resolutionHeight = 1080;
    private int frameRate = 60;
    private bool isWindowed = false;
    private float screenBrightness = 0f;
    
    public int ResolutionWidth
    {
        get => resolutionWidth;
        set
        {
            if (resolutionWidth == value) return;
            resolutionWidth = value;
            OnPropertyChanged();
        }
    }

    public int ResolutionHeight
    {
        get => resolutionHeight;
        set
        {
            if (resolutionHeight == value) return;
            resolutionHeight = value;
            OnPropertyChanged();
        }
    }
    
    public int FrameRate
    {
        get => frameRate;
        set
        {
            if (frameRate == value) return;
            frameRate = value;
            OnPropertyChanged();
        }
    }
    
    public bool IsWindowed
    {
        get => isWindowed;
        set
        {
            if (isWindowed == value) return;
            isWindowed = value;
            OnPropertyChanged();
        }
    }
    
    public float ScreenBrightness
    {
        get => screenBrightness;
        set
        {
            if (Mathf.Approximately(screenBrightness, value)) return;
            screenBrightness = value;
            OnPropertyChanged();
        }
    }
    
    public void ChangeData(ResolutionSettingsData data)
    {
        ResolutionWidth = data.ResolutionWidth;
        ResolutionHeight = data.ResolutionHeight;
        FrameRate = data.FrameRate;
        IsWindowed = data.IsWindowed;
        ScreenBrightness = data.ScreenBrightness;
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
    
    private ResolutionSettingsData backupData, previewData;
    
    public ResolutionSettingsData PreviewData => previewData;
    public ResolutionSettingsData BackupData => backupData;
    
    private readonly string path = "Menu UI/Resolution Settings Screen/";

    public void InitResolutionSettings() // UIManager.Awake()에서 호출
    {
        previewData = SaveManager.Instance.LoadResolutionSettings();
        backupData = SaveManager.Instance.LoadResolutionSettings();
        Debug.Log(backupData.ScreenBrightness);
        
        resolutionSettingsPanel?.Initialize(previewData);
        resolutionPreviewPanel?.Initialize(previewData);
        
        Screen.SetResolution(backupData.ResolutionWidth, backupData.ResolutionHeight, backupData.IsWindowed);
        Application.targetFrameRate = backupData.FrameRate; 
        if (QualitySettings.vSyncCount != 1) QualitySettings.vSyncCount = 1;
        PlayerConstant.pixelationFactor = 0.25f / (backupData.ResolutionWidth / 1920f); //픽셀레이션 값 조절
        resolutionSettingsPanel?.ApplyBrightness(backupData.ScreenBrightness);
    }

    private void OnEnable()
    {
        previewData.ChangeData(backupData);
    }

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
