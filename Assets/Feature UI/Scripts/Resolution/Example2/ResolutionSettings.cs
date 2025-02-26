using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ResolutionSettingsData : INotifyPropertyChanged
{
    private int resolutionX;
    private int resolutionY;
    private int frameRate;
    private bool isWindowed;

    public int ResolutionX
    {
        get => resolutionX;
        set
        {
            resolutionX = value;
            OnPropertyChanged(nameof(ResolutionX));
        }
    }

    public int ResolutionY
    {
        get => resolutionY;
        set
        {
            resolutionY = value;
            OnPropertyChanged(nameof(ResolutionY));
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
    private ResolutionControl resolutionControl;
    
    [SerializeField]
    private ResolutionPreview resolutionPreview;

    private void Awake()
    {
        SaveManager.Instance.LoadResolution(out var resolutionX, out var resolutionY);
        
        resolutionData = new ResolutionSettingsData()
        {
            ResolutionX = resolutionX,
            ResolutionY = resolutionY,
            FrameRate = SaveManager.Instance.LoadFrameRate(),
            IsWindowed = SaveManager.Instance.LoadIsWindowedScreen()
        };
    }

    private void OnEnable()
    {
        resolutionData.PropertyChanged += OnDataPropertyChanged;
        resolutionControl.Initialize(resolutionData);
        resolutionPreview.Initialize(resolutionData);
    }
    
    private void OnDisable()
    {
        resolutionData.PropertyChanged -= OnDataPropertyChanged;
    }

    private void OnDataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "ResolutionX")
        {
            Debug.Log("ResolutionX Changed");
        }
    }
}
