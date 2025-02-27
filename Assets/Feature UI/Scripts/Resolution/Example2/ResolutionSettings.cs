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
    private ResolutionSettingsData resolutionData;
    
    [SerializeField]
    private ResolutionSettingsPanel resolutionSettingsPanel;
    
    [SerializeField]
    private ResolutionPreviewPanel resolutionPreviewPanel;

    private void Awake()
    {
        SaveManager saveMnaager = SaveManager.Instance;
        saveMnaager.LoadResolution(out var resolutionX, out var resolutionY);
        
        resolutionData = new ResolutionSettingsData()
        {
            ResolutionWidth = resolutionX,
            ResolutionHeight = resolutionY,
            FrameRate = saveMnaager.LoadFrameRate(),
            IsWindowed = saveMnaager.LoadIsWindowedScreen()
        };
    }

    private void OnEnable()
    {
        resolutionSettingsPanel.Initialize(resolutionData);
        resolutionPreviewPanel.Initialize(resolutionData);
    }
    
}
