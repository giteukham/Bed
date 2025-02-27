
using System;
using System.ComponentModel;
using System.Windows.Forms;
using Cinemachine.PostFX;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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
    private ScreenBrightness screenBrightness;
    
    [SerializeField]
    private CinemachinePostProcessing postProcessing;
    
    [SerializeField]
    private Image brightnessImage;
    
    private ResolutionSettingsData previewData;
    
    private readonly string path = "Menu UI/Resolution Settings Screen/Settings Panel/";

    /// <summary>
    /// OnEnable에서 Resolution Data를 초기화
    /// </summary>
    /// <param name="data"></param>
    public void Initialize(ResolutionSettingsData data)
    {
        previewData = data;
        screenBrightness.Initialize(previewData, postProcessing, brightnessImage);
    }

    private void OnEnable()
    {
        previewData.PropertyChanged += OnPropertyChanged;
        resolutionWidthInput.onEndEdit.AddListener(OnResolutionWidthInputChanged);
        resolutionHeightInput.onEndEdit.AddListener(OnResolutionHeightInputChanged);
        windowModeToggle.onValueChanged.AddListener(OnWindowModeToggleChanged);
        frameRateDropdown.onValueChanged.AddListener(OnFrameRateDropDownChanged);
    }

    private void OnDisable()
    {
        resolutionWidthInput.onEndEdit.RemoveListener(OnResolutionWidthInputChanged);
        resolutionHeightInput.onEndEdit.RemoveListener(OnResolutionHeightInputChanged);
        windowModeToggle.onValueChanged.RemoveListener(OnWindowModeToggleChanged);
        frameRateDropdown.onValueChanged.RemoveListener(OnFrameRateDropDownChanged);
        previewData.PropertyChanged -= OnPropertyChanged;
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
        screenBrightness.ApplyBrightness(brightness);
    }
}