
using System;
using System.ComponentModel;
using System.Windows.Forms;
using Cinemachine.PostFX;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ResolutionSettingsPanel : MonoBehaviour
{
    [Header("Resolution")]
    
    [SerializeField] 
    private TMP_InputField resolutionWidthInput;
    
    [SerializeField] 
    private TMP_InputField resolutionHeightInput;
    
    [Header("Frame Rate")]
    
    [SerializeField] 
    private TMP_Dropdown frameRateDropdown;
    
    [Header("Window Mode")]
    
    [SerializeField] 
    private Toggle windowModeToggle;
    
    [Header("Screen Brightness")]
    
    [SerializeField]
    private DisplayBrightnessController displayBrightnessController;
    
    [SerializeField]
    private CinemachinePostProcessing postProcessing;

    [SerializeField]
    private Image brightnessCheckImage;
    
    [SerializeField]
    private Image brightnessHandleImage;
    
    private ResolutionSettingsData previewData;
    private ResolutionSettingsDTO backupData;
    
    private readonly string path = "Menu UI/Resolution Settings Screen/Settings Panel/";

    /// <summary>
    /// OnEnable에서 Resolution Data를 초기화
    /// </summary>
    /// <param name="preivewData"></param>
    public void Initialize(ResolutionSettingsData preivewData, ResolutionSettingsDTO backupData)
    {
        this.previewData = preivewData;
        this.backupData = backupData;
        
        Assert.IsNotNull(postProcessing, $"{path}Post Processing is null");
        Assert.IsNotNull(brightnessCheckImage, $"{path}Brightness Check Image is null");
        Assert.IsNotNull(brightnessHandleImage, $"{path}Brightness Handle Image is null");
        displayBrightnessController.Initialize(previewData, backupData, postProcessing, brightnessCheckImage, brightnessHandleImage);
    }

    private void OnEnable()
    {
        previewData.PropertyChanged += OnPropertyChanged;
        
        Assert.IsNotNull(resolutionWidthInput, $"{path}Resolution Width InputField is null");
        resolutionWidthInput.onEndEdit.AddListener(OnResolutionWidthInputChanged);
        
        Assert.IsNotNull(resolutionHeightInput, $"{path}Resolution Height InputField is null");
        resolutionHeightInput.onEndEdit.AddListener(OnResolutionHeightInputChanged);
        
        Assert.IsNotNull(windowModeToggle, $"{path}Window Mode Toggle is null");
        windowModeToggle.onValueChanged.AddListener(OnWindowModeToggleChanged);
        
        Assert.IsNotNull(frameRateDropdown, $"{path}Frame Rate Dropdown is null");
        frameRateDropdown.onValueChanged.AddListener(OnFrameRateDropDownChanged);
    }

    private void OnDisable()
    {
        previewData.PropertyChanged -= OnPropertyChanged;
        resolutionWidthInput.onEndEdit.RemoveListener(OnResolutionWidthInputChanged);
        resolutionHeightInput.onEndEdit.RemoveListener(OnResolutionHeightInputChanged);
        windowModeToggle.onValueChanged.RemoveListener(OnWindowModeToggleChanged);
        frameRateDropdown.onValueChanged.RemoveListener(OnFrameRateDropDownChanged);
    }

    /// <summary>
    /// 인게임에서 Width InputField 값이 변경될 때 호출되는 이벤트
    /// </summary>
    /// <param name="arg0"></param>
    private void OnResolutionWidthInputChanged(string arg0)
    {
        previewData.ResolutionWidth = Convert.ToInt32(arg0);
    }
    
    private void OnResolutionHeightInputChanged(string arg0)
    {
        previewData.ResolutionHeight = Convert.ToInt32(arg0);
    }
    
    private void OnWindowModeToggleChanged(bool arg0)
    {
        previewData.IsWindowed = arg0;
    }

    private void OnFrameRateDropDownChanged(int arg0)
    {
        previewData.FrameRate = arg0;
    }
    
    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ResolutionSettingsData.ResolutionWidth))
        {
            resolutionWidthInput.text = previewData.ResolutionWidth.ToString();
        }
        else if (e.PropertyName == nameof(ResolutionSettingsData.ResolutionHeight))
        {
            resolutionHeightInput.text = previewData.ResolutionHeight.ToString();
        }
        else if (e.PropertyName == nameof(ResolutionSettingsData.IsWindowed))
        {
            windowModeToggle.isOn = previewData.IsWindowed;
        }
        else if (e.PropertyName == nameof(ResolutionSettingsData.FrameRate))
        {
            frameRateDropdown.value = previewData.FrameRate;
        }
    }
    
    public void ApplyBrightness(float brightness)
    {
        displayBrightnessController.ApplyBrightness(brightness);
    }
}