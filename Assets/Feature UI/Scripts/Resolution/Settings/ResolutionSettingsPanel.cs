
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
    [SerializeField]
    private ResolutionDropDowncontroller resolutionDropDowncontroller;    
    [Header("Frame Rate")]
    
    [SerializeField] 
    private TMP_Dropdown frameRateDropdown;
    
    [SerializeField]
    private FrameRateController frameRateController;
    
    [Header("Window Mode")]
    
    [SerializeField]
    private WindowModeController windowModeController;
    
    [SerializeField]
    private Color offColor;

    [SerializeField]
    private Color onColor;

    [SerializeField, Min(0.1f)]
    private float speed = 1f;
    
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
    /// Awake에서 Resolution Data를 초기화
    /// </summary>
    /// <param name="preivewData"></param>
    public void Initialize(ResolutionSettingsData preivewData, ResolutionSettingsDTO backupData)
    {
        this.previewData = preivewData;
        this.backupData = backupData;
        
        Assert.IsNotNull(postProcessing, $"{path}Post Processing is null");
        Assert.IsNotNull(brightnessCheckImage, $"{path}Brightness Check Image is null");
        Assert.IsNotNull(brightnessHandleImage, $"{path}Brightness Handle Image is null");

        resolutionDropDowncontroller.Initialize(previewData);
        frameRateController.Initialize(previewData, backupData, frameRateDropdown);
        windowModeController.Initialize(previewData, backupData, ref offColor, ref onColor, speed);
        displayBrightnessController?.Initialize(previewData, backupData, postProcessing, brightnessCheckImage, brightnessHandleImage);
    }
    
    public void ApplyBrightness(float brightness)
    {
        displayBrightnessController.ApplyBrightness(brightness);
    }
}