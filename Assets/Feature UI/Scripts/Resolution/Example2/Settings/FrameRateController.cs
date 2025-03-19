
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class FrameRateController : FunctionControllerBase
{
    private TMP_Dropdown frameRateDropdown;
    List<int> frameRates = new();
    
    public void Initialize(
        ResolutionSettingsData previewData, 
        ResolutionSettingsDTO backupData, 
        TMP_Dropdown frameRateDropdown)
    {
        base.Initialize(previewData, backupData);
        this.frameRateDropdown = frameRateDropdown;

        this.frameRateDropdown.ClearOptions();
        frameRates.Add(30);
        frameRates.Add(60);
        this.frameRateDropdown.AddOptions(frameRates.ConvertAll(x => x.ToString()));

        frameRateDropdown.value = frameRateDropdown.options.FindIndex(option => option.text.Equals(previewData.FrameRate.ToString()));
    }

    private void OnEnable()
    {
        Assert.IsNotNull(frameRateDropdown, $"{path}Frame Rate Dropdown is null");
        frameRateDropdown.onValueChanged.AddListener(OnFrameRateDropdownChanged);
    }
    
    private void OnDisable()
    {
        frameRateDropdown.onValueChanged.RemoveListener(OnFrameRateDropdownChanged);
    }

    private void OnFrameRateDropdownChanged(int arg0)
    {
        previewData.FrameRate = Convert.ToInt32(frameRateDropdown.options[arg0].text);
    }

    protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ResolutionSettingsData.FrameRate))
        {
        }
    }
}
