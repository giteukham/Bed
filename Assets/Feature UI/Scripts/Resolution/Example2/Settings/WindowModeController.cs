
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
        
        windowModeToggle.onValueChanged.AddListener(OnWindowModeToggleChanged);
        windowModeToggle.isOn = backupData.IsWindowed;
        switchAnimator.SetBool("IsOn", backupData.IsWindowed);
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
            switchAnimator.SetBool("IsOn", previewData.IsWindowed);
        }
    }
}
