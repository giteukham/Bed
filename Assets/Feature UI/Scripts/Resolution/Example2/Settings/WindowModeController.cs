
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class WindowModeController : FunctionControllerBase
{
    private Toggle windowModeToggle;
    
    public void Initialize(
        ResolutionSettingsData previewData, 
        ResolutionSettingsDTO backupData, 
        Toggle windowModeToggle)
    {
        base.Initialize(previewData, backupData);
        
        this.windowModeToggle = windowModeToggle;
    }

    private void OnEnable()
    {
        Assert.IsNotNull(windowModeToggle, $"{path}Window Mode Toggle is null");
        windowModeToggle.onValueChanged.AddListener(OnWindowModeToggleChanged);
    }

    private void OnDisable()
    {
        windowModeToggle.onValueChanged.RemoveListener(OnWindowModeToggleChanged);
    }
    
    private void OnWindowModeToggleChanged(bool arg0)
    {
        previewData.IsWindowed = arg0;
    }

    protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ResolutionSettingsData.IsWindowed))
        {
            windowModeToggle.isOn = previewData.IsWindowed;
            Screen.fullScreen = previewData.IsWindowed;
        }
    }
}
