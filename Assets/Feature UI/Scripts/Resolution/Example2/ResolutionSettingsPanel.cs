
using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ResolutionSettingsPanel : MonoBehaviour
{
    [SerializeField] 
    private TMP_InputField resolutionWidthInput;
    
    [SerializeField] 
    private TMP_InputField resolutionHeightInput;
    
    [SerializeField] 
    private TMP_Dropdown frameRateDropdown;
    
    [SerializeField] 
    private Toggle windowModeToggle;
    
    private ResolutionSettingsData resolutionData;
    
    public void Initialize(ResolutionSettingsData resolutionData)
    {
        this.resolutionData = resolutionData;
        resolutionData.PropertyChanged += OnPropertyChanged;
        resolutionWidthInput.onEndEdit.AddListener(OnResolutionWidthInputChanged);
        resolutionHeightInput.onEndEdit.AddListener(OnResolutionHeightInputChanged);
        frameRateDropdown.onValueChanged.AddListener(OnFrameRateDropDownChanged);
        windowModeToggle.onValueChanged.AddListener(OnWindowModeToggleChanged);
    }

    private void OnDisable()
    {
        resolutionData.PropertyChanged -= OnPropertyChanged;
    }

    /// <summary>
    /// �ΰ��ӿ��� Width InputField ���� ����� �� ȣ��Ǵ� �̺�Ʈ
    /// </summary>
    /// <param name="arg0"></param>
    private void OnResolutionWidthInputChanged(string arg0)
    {
        resolutionData.ResolutionWidth = int.Parse(arg0);
    }
    
    private void OnResolutionHeightInputChanged(string arg0)
    {
        resolutionData.ResolutionHeight = int.Parse(arg0);
    }
    
    private void OnWindowModeToggleChanged(bool arg0)
    {
        resolutionData.IsWindowed = arg0;
    }

    private void OnFrameRateDropDownChanged(int arg0)
    {
        resolutionData.FrameRate = arg0;
    }
    
    
    /// <summary>
    /// Resolution Data ���� ����� �� ȣ��Ǵ� �̺�Ʈ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ResolutionSettingsData.ResolutionWidth))
        {
            resolutionWidthInput.text = resolutionData.ResolutionWidth.ToString();
        }
        else if (e.PropertyName == nameof(ResolutionSettingsData.ResolutionHeight))
        {
            resolutionHeightInput.text = resolutionData.ResolutionHeight.ToString();
        }
        else if (e.PropertyName == nameof(ResolutionSettingsData.FrameRate))
        {
            frameRateDropdown.value = resolutionData.FrameRate;
        }
        else if (e.PropertyName == nameof(ResolutionSettingsData.IsWindowed))
        {
            windowModeToggle.isOn = resolutionData.IsWindowed;
            Debug.Log("IsWindowed: " + resolutionData.IsWindowed);
        }
    }
    
    private void OnDestroy()
    {
        resolutionData.PropertyChanged -= OnPropertyChanged;
    }
    
}
