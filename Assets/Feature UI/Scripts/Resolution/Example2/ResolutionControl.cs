
using UnityEngine;
using UnityEngine.UI;

public class ResolutionControl : MonoBehaviour
{
    [SerializeField] private InputField resolutionXInput;
    [SerializeField] private InputField resolutionYInput;
    [SerializeField] private Dropdown frameRateDropdown;
    [SerializeField] private Toggle windowModeToggle;
    
    private ResolutionSettingsData resolutionData;
    
    public void Initialize(ResolutionSettingsData resolutionData)
    {
        this.resolutionData = resolutionData;
        resolutionXInput.onEndEdit.AddListener(OnResolutionXChanged);
        resolutionYInput.onEndEdit.AddListener(OnResolutionYChanged);
        frameRateDropdown.onValueChanged.AddListener(OnFrameRateChanged);
        windowModeToggle.onValueChanged.AddListener(OnWindowModeChanged);
        UpdateUI();
    }

    private void OnResolutionXChanged(string arg0)
    {
        resolutionData.ResolutionX = int.Parse(arg0);
    }
    
    private void OnResolutionYChanged(string arg0)
    {
        resolutionData.ResolutionY = int.Parse(arg0);
    }
    
    private void OnWindowModeChanged(bool arg0)
    {
        resolutionData.IsWindowed = arg0;
    }

    private void OnFrameRateChanged(int arg0)
    {
        resolutionData.FrameRate = arg0;
    }
    
    public void UpdateUI()
    {
        resolutionXInput.text = resolutionData.ResolutionX.ToString();
        resolutionYInput.text = resolutionData.ResolutionY.ToString();
        frameRateDropdown.value = resolutionData.FrameRate;
        windowModeToggle.isOn = resolutionData.IsWindowed;
    }
    
}
