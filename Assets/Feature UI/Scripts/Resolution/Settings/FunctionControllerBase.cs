
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class FunctionControllerBase : MonoBehaviour
{
    protected ResolutionSettingsData previewData;
    protected ResolutionSettingsDTO backupData;
    
    protected readonly string path = "Menu UI/Resolution Settings Screen/Settings Panel/";
    
    protected void Initialize(ResolutionSettingsData previewData, ResolutionSettingsDTO backupData)
    {
        this.previewData = previewData;
        this.backupData = backupData;
        previewData.PropertyChanged += OnPropertyChanged;
    }
    
    protected abstract void OnPropertyChanged(object sender, PropertyChangedEventArgs e);
}
