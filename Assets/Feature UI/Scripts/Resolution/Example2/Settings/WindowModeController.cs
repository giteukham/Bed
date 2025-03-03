
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class WindowModeController : FunctionControllerBase
{
    private Toggle windowModeToggle;
    private Animator switchAnimator;
    
    public void Initialize(
        ResolutionSettingsData previewData, 
        ResolutionSettingsDTO backupData)
    {
        base.Initialize(previewData, backupData);
        
        this.windowModeToggle = GetComponent<Toggle>();
        this.switchAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        Assert.IsNotNull(windowModeToggle, $"{path}Window Mode Toggle is null");
        
        windowModeToggle.isOn = previewData.IsWindowed;
        windowModeToggle.onValueChanged.AddListener(OnWindowModeToggleChanged);
        switchAnimator.SetBool("IsOn", previewData.IsWindowed);
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
            switchAnimator.SetBool("IsOn", previewData.IsWindowed);
            Screen.fullScreen = previewData.IsWindowed;
        }
    }
}
