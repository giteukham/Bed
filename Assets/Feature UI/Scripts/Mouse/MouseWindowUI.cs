using System;
using System.Collections.Generic;
using System.Globalization;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum Arrow
{
    Up,
    Down,
    Left,
    Right
}

public class MouseWindowUI : MonoBehaviour
{
    private MouseSettings mouseSettings;
    
    [Header("Mouse UI")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TMP_InputField sensitivityValue;
    [SerializeField] private Image verticalSwitch;
    [SerializeField] private Image horizontalSwitch;
    
    [SerializeField] private Sprite onImage;                                                // 초록색 스위치 이미지
    [SerializeField] private Sprite offImage;                                               // 빨강 스위치 이미지
    
    [Header("Arrow")] 
    [SerializedDictionary("Arrow", "GameObject")]
    [SerializeField] private SerializedDictionary<Arrow, GameObject> arrows;

    private Dictionary<Arrow, Image> arrowImages;
    private readonly Color arrowMoveColor = Color.gray;
    
    public static event Action OnMouseSettingsScreenActive, OnMouseSettingsScreenDeactive;
    
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
        OnMouseSettingsScreenActive?.Invoke();
        
        mouseSettings.OnHorizontalReverse += (isReverse) => ToggleSwitch(isReverse, horizontalSwitch);
        mouseSettings.OnVerticalReverse += (isReverse) => ToggleSwitch(isReverse, verticalSwitch);
        ChangeUI();
        
        sensitivitySlider.onValueChanged.AddListener((value) =>
        {
            sensitivityValue.text = (Mathf.Floor(value * 100f) / 100f).ToString(CultureInfo.CurrentCulture);
            mouseSettings.ChangeSensitivity(value);
        });
        
        sensitivityValue.onEndEdit.AddListener((value) =>
        {
            if (float.TryParse(value, out var result))
            {
                ChangeSensitivityOnInputField(result);
            }
        });
    }
    
    private void OnDisable()
    {
        OnMouseSettingsScreenDeactive?.Invoke();
        sensitivitySlider.onValueChanged.RemoveAllListeners();
        sensitivityValue.onEndEdit.RemoveAllListeners();
        OnMouseSettingsScreenActive = null;
        OnMouseSettingsScreenDeactive = null;
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

    private void ChangeUI()
    {
        sensitivitySlider.value = mouseSettings.MouseSensitivity;
        sensitivityValue.text = (Mathf.Floor(sensitivitySlider.value * 100f) / 100f).ToString(CultureInfo.CurrentCulture);
        //마우스 반전에 따른 버튼 이미지 변경
        verticalSwitch.sprite = mouseSettings.IsVerticalReverse ? onImage : offImage;
        horizontalSwitch.sprite = mouseSettings.IsHorizontalReverse ? onImage : offImage;
    }
    
    private void ChangeSensitivityOnInputField(float value)
    {
        if (value < sensitivitySlider.minValue)
        {
            value = sensitivitySlider.minValue;
        }
        else if (value > sensitivitySlider.maxValue)
        {
            value = sensitivitySlider.maxValue;
        }
        sensitivitySlider.value = value;
        mouseSettings.ChangeSensitivity(value);
    }
    
    //버튼 이미지 변경(false일 경우 빨강, true일 경우 초록)
    private void ToggleSwitch(bool isReverse, Image switchImage) => switchImage.sprite = isReverse ? onImage : offImage;
}