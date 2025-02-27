using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;

public class ResolutionSettingsData : INotifyPropertyChanged
{
    private int resolutionWidth;
    private int resolutionHeight;
    private int frameRate;
    private bool isWindowed;

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

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class ResolutionSettings : MonoBehaviour
{
    public static ResolutionSettingsData backupData;
    private ResolutionSettingsData resolutionData;
    
    [SerializeField]
    private ResolutionSettingsPanel resolutionSettingsPanel;
    
    [SerializeField]
    private ResolutionPreviewPanel resolutionPreviewPanel;

    public void InitResolutionSettings() // UIManager.Awake()에서 호출
    {
        backupData = SaveManager.Instance.LoadResolutionSettings();
        Screen.SetResolution(backupData.ResolutionWidth, backupData.ResolutionHeight, backupData.IsWindowed);
        Application.targetFrameRate = backupData.FrameRate; 
        if (QualitySettings.vSyncCount != 1) QualitySettings.vSyncCount = 1;
        PlayerConstant.pixelationFactor = 0.25f / (backupData.ResolutionWidth / 1920f); //픽셀레이션 값 조절
    }

    private void OnEnable()
    {
        resolutionData = new ResolutionSettingsData()
        {
            ResolutionWidth = backupData.ResolutionWidth,
            ResolutionHeight = backupData.ResolutionHeight,
            FrameRate = backupData.FrameRate,
            IsWindowed = backupData.IsWindowed
        };

        backupData.ResolutionWidth = resolutionData.ResolutionWidth;
        backupData.ResolutionHeight = resolutionData.ResolutionHeight;
        backupData.FrameRate = resolutionData.FrameRate;
        backupData.IsWindowed = resolutionData.IsWindowed;

        resolutionSettingsPanel.Initialize(resolutionData);
        resolutionPreviewPanel.Initialize(resolutionData);
    }

    private void OnDisable()
    {
        resolutionData = null;
    }
}
