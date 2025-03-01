using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionSelectController : MonoBehaviour
{
    private ResolutionSettingsData previewData;
    [SerializeField] private TMP_InputField inputWidth, inputHeight;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    private int minWidth, minHeight, maxWidth, maxHeight;

    public void Initialize(ResolutionSettingsData previewData)
    {
        this.previewData = previewData;
        
        resolutionDropdown.ClearOptions();

        minWidth = Display.main.systemWidth / 4;
        minHeight = Display.main.systemHeight / 4;
        maxWidth = Display.main.systemWidth;
        maxHeight = Display.main.systemHeight;

        float widthNum = (Display.main.systemWidth - Display.main.systemWidth / 4f) / 9f;
        float heightNum = (Display.main.systemHeight - Display.main.systemHeight / 4f) / 9f;

        List<Vector2Int> resolutions = new();

        for (int i = 9; i >= 0; i--)
            resolutions.Add(new Vector2Int((int)Mathf.Round(Display.main.systemWidth - widthNum * i), (int)Mathf.Round(Display.main.systemHeight - heightNum * i)));

        resolutionDropdown.AddOptions(resolutions.ConvertAll<string>
        (
            value => 
            value.x < 1000 ? "\u200A\u200A\u200A\u200A\u200A\u200A" + value.x + " X " + value.y 
            : value.x + " X " + value.y
        ));
    }

    private void OnEnable()
    {
        inputWidth.onEndEdit.AddListener(OnWidthChanged);
        inputHeight.onEndEdit.AddListener(OnHeightChanged);
        resolutionDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    private void OnDisable()
    {
        inputWidth.onEndEdit.RemoveListener(OnWidthChanged);
        inputHeight.onEndEdit.RemoveListener(OnHeightChanged);
        resolutionDropdown.onValueChanged.RemoveListener(OnDropdownChanged);
    }

    public void OnWidthChanged(string value)
    {
        if (int.TryParse(value, out int x))
        {
            previewData.ResolutionWidth = x;
        }
    }

    public void OnHeightChanged(string value)
    {
        if (int.TryParse(value, out int y))
        {
            previewData.ResolutionHeight = y;
        }
    }

    private void OnDropdownChanged(int index)
    {
        string[] values = resolutionDropdown.options[index].text.Split('X');
        if (values.Length == 2 && int.TryParse(values[0].Trim(), out int x) && int.TryParse(values[1].Trim(), out int y))
        {
            previewData.ResolutionWidth = x;
            previewData.ResolutionHeight = y;
        }
    }
}
