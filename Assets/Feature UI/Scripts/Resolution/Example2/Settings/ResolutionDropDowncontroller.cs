using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResolutionDropDowncontroller : FunctionControllerBase
{
    [SerializeField] private TMP_InputField inputWidth, inputHeight;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    List<Vector2Int> fullScreenResolutions = new(), windowedResolutions = new();

    public Action OnSelectChanged;

    public void Initialize(ResolutionSettingsData previewData)
    {
        base.Initialize(previewData, backupData);
        resolutionDropdown.ClearOptions();

        float minusStandardWidth = (previewData.fullScreenMaxWidth - previewData.fullScreenMaxWidth / 4f) / 9f;
        float minusStandardHeight = (previewData.fullScreenMaxHeight - previewData.fullScreenMaxHeight / 4f) / 9f;  

        if (previewData.fullScreenRatio == previewData.windowedRatio) // 16:9일때
        {
            for (int i = 9; i >= 0; i--)
            {
                Vector2Int item = new Vector2Int((int)Mathf.Round(previewData.fullScreenMaxWidth - minusStandardWidth * i), (int)Mathf.Round(previewData.fullScreenMaxHeight - minusStandardHeight * i));
                fullScreenResolutions.Add(item);
                windowedResolutions.Add(item);
            }
        }
        else if (previewData.fullScreenRatio > previewData.windowedRatio) // 16:9보다 width가 더 넒을때
        {
            float minusConversionWidth = (previewData.windowedMaxWidth - previewData.windowedMaxWidth / 4f) / 9f;
            for (int i = 9; i >= 0; i--)
            {
                fullScreenResolutions.Add(new Vector2Int((int)Mathf.Round(previewData.fullScreenMaxWidth - minusStandardWidth * i), (int)Mathf.Round(previewData.fullScreenMaxHeight - minusStandardHeight * i)));
                windowedResolutions.Add(new Vector2Int((int)Mathf.Round(previewData.windowedMaxWidth - minusConversionWidth * i), (int)Mathf.Round(previewData.fullScreenMaxHeight - minusStandardHeight * i)));
            }
        }
        else if (previewData.fullScreenRatio < previewData.windowedRatio) // 16:9보다 width가 더 좁을때
        {
            float minusConversionHeight = (previewData.windowedMaxHeight - previewData.windowedMaxHeight / 4f) / 9f;
            for (int i = 9; i >= 0; i--)
            {
                fullScreenResolutions.Add(new Vector2Int((int)Mathf.Round(previewData.fullScreenMaxWidth - minusStandardWidth * i), (int)Mathf.Round(previewData.fullScreenMaxHeight - minusStandardHeight * i)));
                windowedResolutions.Add(new Vector2Int((int)Mathf.Round(previewData.fullScreenMaxWidth - minusStandardWidth * i), (int)Mathf.Round(previewData.windowedMaxHeight - minusConversionHeight * i)));
            }
        }

        resolutionDropdown.AddOptions
        (
            fullScreenResolutions.ConvertAll<string>
            (
                value => 
                value.x < 1000 ? "\u200A\u200A\u200A\u200A\u200A\u200A" + value.x + " X " + value.y 
                : value.x + " X " + value.y
            )
        );
    }

    private void OnEnable()
    {
        inputWidth.onEndEdit.AddListener(OnWidthChanged);
        inputHeight.onEndEdit.AddListener(OnHeightChanged);
    }

    private void OnDisable()
    {
        inputWidth.onEndEdit.RemoveListener(OnWidthChanged);
        inputHeight.onEndEdit.RemoveListener(OnHeightChanged);
    }

    public void OnWidthChanged(string value)
    {
        if (int.TryParse(value, out int x))
        {
            previewData.ResolutionWidth = x;
            
            foreach (Vector2Int resolution in fullScreenResolutions)
            {
                if (resolution.x == previewData.ResolutionWidth)
                {
                    string searchText = resolution.x < 1000 ? "\u200A\u200A\u200A\u200A\u200A\u200A" + resolution.x + " X " + resolution.y : resolution.x + " X " + resolution.y;
                    resolutionDropdown.value = resolutionDropdown.options.FindIndex(option => option.text == searchText);
                    break;
                }
            }
        }
    }

    public void OnHeightChanged(string value)
    {
        if (int.TryParse(value, out int y))
        {
            previewData.ResolutionHeight = y;

            foreach (Vector2Int resolution in fullScreenResolutions)
            {
                if (resolution.y == previewData.ResolutionHeight)
                {
                    string searchText = resolution.x < 1000 ? "\u200A\u200A\u200A\u200A\u200A\u200A" + resolution.x + " X " + resolution.y : resolution.x + " X " + resolution.y;
                    resolutionDropdown.value = resolutionDropdown.options.FindIndex(option => option.text == searchText);
                    break;
                }
            }
        }
    }
    public void OnSelection()
    {
        string[] values = resolutionDropdown.options[resolutionDropdown.value].text.Split('X');
        if (values.Length == 2 && int.TryParse(values[0].Trim(), out int x) && int.TryParse(values[1].Trim(), out int y))
        {
            previewData.ResolutionWidth = x;
            previewData.ResolutionHeight = y;
        }
        OnSelectChanged?.Invoke();
    }

    protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ResolutionSettingsData.ResolutionWidth))
        {
            inputWidth.text = previewData.ResolutionWidth.ToString();
        }
        else if (e.PropertyName == nameof(ResolutionSettingsData.ResolutionHeight))
        {
            inputHeight.text = previewData.ResolutionHeight.ToString();
        }
        if (e.PropertyName == nameof(ResolutionSettingsData.IsWindowed))
        {
            resolutionDropdown.ClearOptions();
            if (previewData.IsWindowed)
            {
                resolutionDropdown.AddOptions
                (
                    windowedResolutions.ConvertAll<string>
                    (
                        value => 
                        value.x < 1000 ? "\u200A\u200A\u200A\u200A\u200A\u200A" + value.x + " X " + value.y 
                        : value.x + " X " + value.y
                    )
                );

                if(previewData.ResolutionWidth > previewData.windowedMaxWidth || previewData.ResolutionHeight > previewData.windowedMaxHeight)
                {
                    previewData.ResolutionWidth = previewData.windowedMaxWidth;
                    previewData.ResolutionHeight = previewData.windowedMaxHeight;
                }
            }
            else
            {
                resolutionDropdown.AddOptions
                (
                    fullScreenResolutions.ConvertAll<string>
                    (
                        value => 
                        value.x < 1000 ? "\u200A\u200A\u200A\u200A\u200A\u200A" + value.x + " X " + value.y 
                        : value.x + " X " + value.y
                    )
                );
            }
            resolutionDropdown.RefreshShownValue();
        }
    }
}
