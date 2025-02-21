using System;
using System.Collections.Generic;
using System.Globalization;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


// test test test test

public enum Arrow
{
    Up,
    Down,
    Left,
    Right
}

public class MouseSensitiveUI : MonoBehaviour
{
    private MouseSettings mouseSettings;
    
    [Header("Mouse UI")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_InputField sensitivityValue;
    [SerializeField] private Image sensitiveBar;
    //[SerializeField] private Image verticalSwitch;
    //[SerializeField] private Image horizontalSwitch;
    [SerializeField] private GameObject VerticalSwitch;
    [SerializeField] private GameObject HorizontalSwitch;
    
    [SerializeField] private Sprite onImage;                                                // 초록색 스위치 이미지
    [SerializeField] private Sprite offImage;                                               // 빨강 스위치 이미지
    
    [Header("Arrow")] 
    [SerializedDictionary("Arrow", "GameObject")]
    [SerializeField] private SerializedDictionary<Arrow, GameObject> arrows;

    private Dictionary<Arrow, Image> arrowImages;
    private readonly Color arrowMoveColor = Color.gray;

    private void OnEnable()
    {
        mouseSettings = MouseSettings.Instance;
        arrowImages = new Dictionary<Arrow, Image>
        {
            { Arrow.Up, arrows[Arrow.Up].GetComponent<Image>() },
            { Arrow.Down, arrows[Arrow.Down].GetComponent<Image>() },
            { Arrow.Left, arrows[Arrow.Left].GetComponent<Image>() },
            { Arrow.Right, arrows[Arrow.Right].GetComponent<Image>() }
        };

        mouseSettings.OnHorizontalReverse += ToggleHorizontalSwitch;
        mouseSettings.OnVerticalReverse += ToggleVerticalSwitch;

        sensitivitySlider.onValueChanged.AddListener(ChangeSensitive);
        sensitivityValue.onEndEdit.AddListener(ChangeSensitivityOnInputField);
        sensitiveBar.OnDoubleClick(() => ChangeSensitive(1f));
        ChangeSensitive(mouseSettings.MouseSensitivity);
        
        //verticalSwitch.sprite = mouseSettings.IsVerticalReverse ? onImage : offImage;
        //horizontalSwitch.sprite = mouseSettings.IsHorizontalReverse ? onImage : offImage;

        VerticalSwitch.GetComponent<SwitchButton_Mouse>().SwitchLoadDataApply(mouseSettings.IsVerticalReverse);
        HorizontalSwitch.GetComponent<SwitchButton_Mouse>().SwitchLoadDataApply(mouseSettings.IsHorizontalReverse);
    }
    
    private void OnDisable()
    {
        sensitivitySlider.onValueChanged.RemoveAllListeners();
        sensitivityValue.onEndEdit.RemoveAllListeners();
        mouseSettings.OnHorizontalReverse -= ToggleHorizontalSwitch;
        mouseSettings.OnVerticalReverse -= ToggleVerticalSwitch;
    }

    private void Update()
    {
        ChangeArrowColor();
    }
    
    private void ChangeArrowColor()
    {
        switch (mouseSettings.MouseVerticalSpeed)
        {
            case float y when y > 0f:
                arrowImages[Arrow.Up].color = arrowMoveColor;
                break;
            case float y when y < 0f:
                arrowImages[Arrow.Down].color = arrowMoveColor;
                break;
            default:
                arrowImages[Arrow.Up].color = Color.white;
                arrowImages[Arrow.Down].color = Color.white;
                break;
        }
        switch (mouseSettings.MouseHorizontalSpeed)
        {
            case float x when x > 0f:
                arrowImages[Arrow.Right].color = arrowMoveColor;
                break;
            case float x when x < 0f:
                arrowImages[Arrow.Left].color = arrowMoveColor;
                break;
            default:
                arrowImages[Arrow.Right].color = Color.white;
                arrowImages[Arrow.Left].color = Color.white;
                break;
        }
    }
    
    private void ChangeSensitive(float value)
    {
        sensitivitySlider.value = value;
        sensitivityValue.text = (Mathf.Floor(value * 100f) / 100f).ToString(CultureInfo.CurrentCulture);
        mouseSettings.ChangeSensitivity(value);
    }
    
    private void ChangeSensitivityOnInputField(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            sensitivityValue.text = "0.1";
            return;
        }
        
        if (float.TryParse(value, out var result))
        {
            if (result < sensitivitySlider.minValue)
            {
                sensitivityValue.text = sensitivitySlider.minValue.ToString(CultureInfo.CurrentCulture);
                return;
            }
            if (result > sensitivitySlider.maxValue)
            {
                sensitivityValue.text = sensitivitySlider.maxValue.ToString(CultureInfo.CurrentCulture);
                return;
            }
            ChangeSensitive(result);
        }
        else
        {
            sensitivityValue.text = "0.1";
        }
    }
    
    //버튼 이미지 변경(false일 경우 빨강, true일 경우 초록)
    //private void ToggleSwitch(bool isReverse, Image switchImage) => switchImage.sprite = isReverse ? onImage : offImage;
    private void ToggleSwitch(bool isReverse, GameObject switchObject) => switchObject.GetComponent<SwitchButton_Mouse>().OnSwitchButtonClicked(isReverse);

    private void ToggleHorizontalSwitch(bool isReverse) => HorizontalSwitch.GetComponent<SwitchButton_Mouse>().OnSwitchButtonClicked(isReverse);
    private void ToggleVerticalSwitch(bool isReverse) => VerticalSwitch.GetComponent<SwitchButton_Mouse>().OnSwitchButtonClicked(isReverse);
}