
using System;
using System.ComponentModel;
using Cinemachine.PostFX;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class ScreenBrightness : MonoBehaviour
{
    private Slider          brightnessSlider;
    private ColorGrading    colorGrading;
    private Material        brightnessImageMaterial;
    
    private ResolutionSettingsData previewData;
    
    private float imageMinAlpha = 0.2f, imageMaxAlpha = 1.0f;
    
    public void Initialize(ResolutionSettingsData data, CinemachinePostProcessing postProcessing, Image brightnessImage)
    {
        previewData = data;
        
        brightnessSlider = GetComponent<Slider>();
        brightnessSlider.value = previewData.ScreenBrightness;
        colorGrading = postProcessing.m_Profile.GetSetting<ColorGrading>();
        
        brightnessImageMaterial = brightnessImage.material;
        ChangeBrightnessImageAlpha(previewData.ScreenBrightness);
    }

    private void OnEnable()
    {
        previewData.PropertyChanged += OnPropertyChanged;
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
