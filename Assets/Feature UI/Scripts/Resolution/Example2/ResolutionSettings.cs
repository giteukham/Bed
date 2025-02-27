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
            resolutionWidth = value;
            OnPropertyChanged(nameof(ResolutionWidth));
        }
    }

    public int ResolutionHeight
    {
        get => resolutionHeight;
        set
        {
            resolutionHeight = value;
            OnPropertyChanged(nameof(ResolutionHeight));
        }
    }
    
    public int FrameRate
    {
        get => frameRate;
        set
        {
            frameRate = value;
            OnPropertyChanged(nameof(FrameRate));
        }
    }
    
    public bool IsWindowed
    {
        get => isWindowed;
        set
        {
            isWindowed = value;
            OnPropertyChanged(nameof(IsWindowed));
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
    private ResolutionSettingsData resolutionData;
    
    [SerializeField]
    private ResolutionSettingsPanel resolutionSettingsPanel;
    
    [SerializeField]
    private ResolutionPreviewPanel resolutionPreviewPanel;

    private void Awake()
    {
        SaveManager.Instance.LoadResolution(out var resolutionX, out var resolutionY);
        
        resolutionData = new ResolutionSettingsData()
        {
            ResolutionWidth = resolutionX,
            ResolutionHeight = resolutionY,
            FrameRate = SaveManager.Instance.LoadFrameRate(),
            IsWindowed = SaveManager.Instance.LoadIsWindowedScreen()
        };
    }

    private void OnEnable()
    {
        resolutionSettingsPanel.Initialize(resolutionData);
        resolutionPreviewPanel.Initialize(resolutionData);
    }
    
}
