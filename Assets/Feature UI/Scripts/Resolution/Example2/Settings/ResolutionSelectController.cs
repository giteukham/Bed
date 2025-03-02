using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResolutionSelectController : MonoBehaviour
{
    private ResolutionSettingsData previewData;
    [SerializeField] private TMP_InputField inputWidth, inputHeight;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    private int minWidth, minHeight, maxWidth, maxHeight;
    //private float ratio;
    List<Vector2Int> resolutions = new();

    public void Initialize(ResolutionSettingsData previewData)
    {
        this.previewData = previewData;
        
        resolutionDropdown.ClearOptions();

        //ratio = (float)Display.main.systemWidth / Display.main.systemHeight;
        minWidth = Display.main.systemWidth / 4;
        minHeight = Display.main.systemHeight / 4;
        maxWidth = Display.main.systemWidth;
        maxHeight = Display.main.systemHeight;

        float widthNum = (Display.main.systemWidth - Display.main.systemWidth / 4f) / 9f;
        float heightNum = (Display.main.systemHeight - Display.main.systemHeight / 4f) / 9f;        

        for (int i = 9; i >= 0; i--)
            resolutions.Add(new Vector2Int((int)Mathf.Round(Display.main.systemWidth - widthNum * i), (int)Mathf.Round(Display.main.systemHeight - heightNum * i)));

        resolutionDropdown.AddOptions(resolutions.ConvertAll<string>
        (
            value => 
            value.x < 1000 ? "\u200A\u200A\u200A\u200A\u200A\u200A" + value.x + " X " + value.y 
            : value.x + " X " + value.y
        ));
        // resolutionDropdown.options.Insert(0, new TMP_Dropdown.OptionData("------------------------"));
    }

    private void OnEnable()
    {
        inputWidth.onEndEdit.AddListener(OnWidthChanged);
        inputHeight.onEndEdit.AddListener(OnHeightChanged);
        // resolutionDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    private void OnDisable()
    {
        inputWidth.onEndEdit.RemoveListener(OnWidthChanged);
        inputHeight.onEndEdit.RemoveListener(OnHeightChanged);
        // resolutionDropdown.onValueChanged.RemoveListener(OnDropdownChanged);
    }

    public void OnWidthChanged(string value)
    {
        if (int.TryParse(value, out int x))
        {
            previewData.ResolutionWidth = x;
            // previewData.ResolutionHeight = (int)Mathf.Round(x / ratio);
            
            foreach (Vector2Int resolution in resolutions)
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
            //previewData.ResolutionWidth = (int)Mathf.Round(y * ratio);

            foreach (Vector2Int resolution in resolutions)
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
    }
}
