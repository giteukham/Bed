
using System;
using System.ComponentModel;
using Cinemachine.PostFX;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class DisplayBrightnessController : MonoBehaviour
{
    private Slider          brightnessSlider;
    private ColorGrading    colorGrading;
    private Material        brightnessImageMaterial;
    
    private ResolutionSettingsData previewData;
    private ResolutionSettingsDTO backupData;
    
    private float imageMinAlpha = 0.2f, imageMaxAlpha = 1.0f;
    
    private readonly string path = "Menu UI/Resolution Settings Screen/Settings Panel/";
    
    public void Initialize(
        ResolutionSettingsData previewData,
        ResolutionSettingsDTO backupData,
        CinemachinePostProcessing postProcessing,
        Image brightnessCheckImage,
        Image brightnessBackgroundImage)
    {
        this.previewData = previewData;
        this.backupData = backupData;
        
        brightnessSlider = GetComponent<Slider>();
        colorGrading = postProcessing.m_Profile.GetSetting<ColorGrading>();
        
        brightnessBackgroundImage.OnDoubleClick(() => brightnessSlider.value = 0f);
        brightnessImageMaterial = brightnessCheckImage.material;
    }

    private void OnEnable()
    {
        previewData.PropertyChanged += OnPropertyChanged;
        brightnessSlider.value = backupData.ScreenBrightness;
        brightnessSlider.onValueChanged.AddListener(OnBrightnessSliderChanged);
    }

    private void OnDisable()
    {
        previewData.PropertyChanged -= OnPropertyChanged;
        brightnessSlider.onValueChanged.RemoveListener(OnBrightnessSliderChanged);
    }

    private void OnBrightnessSliderChanged(float arg0)
    {
        previewData.ScreenBrightness = arg0;
        ChangeBrightnessImageAlpha(arg0);
    }
    
    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ResolutionSettingsData.ScreenBrightness))
        {
            brightnessSlider.value = previewData.ScreenBrightness;
        }
    }

    private void ChangeBrightnessImageAlpha(float brightness)
    {
        var reverseLerp = Mathf.InverseLerp(brightnessSlider.minValue, brightnessSlider.maxValue, brightness);
        var alpha = Mathf.Lerp(imageMinAlpha, imageMaxAlpha, reverseLerp);
        brightnessImageMaterial.color = new Color(1f, 1f, 1f, alpha);   
    }
    
    public void ApplyBrightness(float brightness)
    {
        colorGrading.gamma.Override(new Vector4(1f, 1f, 1f, brightness));
    }

    private void OnDestroy()
    {
        colorGrading.gamma.Override(new Vector4(1f, 1f, 1f, 0f));
    }
}
