using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionControlPanel : MonoBehaviour
{
    [SerializeField] private InputField resolutionXInput;
    [SerializeField] private InputField resolutionYInput;
    [SerializeField] private Dropdown frameRateDropdown;
    [SerializeField] private Toggle windowModeToggle;

    private ResolutionSettingsManegment settingsManagement;

    private void Awake()
    {
        settingsManagement = FindObjectOfType<ResolutionSettingsManegment>();
        resolutionXInput.onEndEdit.AddListener(OnResolutionChanged);
        resolutionYInput.onEndEdit.AddListener(OnResolutionChanged);
        frameRateDropdown.onValueChanged.AddListener(value => settingsManagement.SetFrameRate(value));
        windowModeToggle.onValueChanged.AddListener(value => settingsManagement.SetWindowMode(value));

        settingsManagement.OnResolutionChanged.AddListener(UpdateResolutionUI);
        settingsManagement.OnFrameRateChanged.AddListener(UpdateFrameRateUI);
        settingsManagement.OnWindowModeChanged.AddListener(UpdateWindowModeUI);
    }

    // 인풋필드로 값 바꿀때 호출
    private void OnResolutionChanged(string _)
    {
        int x = int.Parse(resolutionXInput.text);
        int y = int.Parse(resolutionYInput.text);
        settingsManagement.SetResolution(x, y);
    }

    private void UpdateResolutionUI(int x, int y)
    {
        resolutionXInput.text = x.ToString();
        resolutionYInput.text = y.ToString();
    }

    private void UpdateFrameRateUI(int frame)
    {
        frameRateDropdown.value = frame;
    }

    private void UpdateWindowModeUI(bool windowed)
    {
        windowModeToggle.isOn = windowed;
    }
}
