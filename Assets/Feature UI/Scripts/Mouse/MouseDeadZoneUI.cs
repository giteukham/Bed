
using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class MouseDeadZoneUI : MonoBehaviour
{
    [SerializeField] private RectTransform valueBarTransform, backgroundTransform, deadZoneAreaTransform;
    [SerializeField] private TMP_InputField deadZoneInputField;
    [SerializeField] private Slider deadZoneSlider;
    
    [Header("Components")]
    [SerializeField] private MouseDeadZoneArrow mouseDeadZoneArrow;

    [Header("Settings")]
    [SerializeField]
    private Color idleColor;
    
    [SerializeField]
    private Color activeColor;

    private MouseSettings mouseSettings;
    private MouseSettingsPreviewPlayer previewPlayer;
    
    private Image deadZoneAreaImage;

    private void Awake()
    {
        deadZoneAreaImage = deadZoneAreaTransform.GetComponent<Image>();
        deadZoneAreaImage.color = idleColor;
    }

    private void OnEnable()
    {
        mouseSettings = MouseSettings.Instance;
        
        mouseDeadZoneArrow.Init(backgroundTransform, idleColor);
        mouseDeadZoneArrow.OnArrowDrag += ChangeDeadZoneArea;
        
        previewPlayer = (MouseSettingsPreviewPlayer) MouseSettings.Instance.PreviewPlayer;
        previewPlayer.OnDirectionStateChanged += types =>
        {
            if (types == PlayerDirectionStateTypes.Switching)
            {
                ChangeDeadZoneAreaColor(activeColor);
                mouseDeadZoneArrow.ChangeArrowColor(activeColor);
            }
            else if (deadZoneAreaImage.color != idleColor)
            {
                ChangeDeadZoneAreaColor(idleColor);
                mouseDeadZoneArrow.ChangeArrowColor(idleColor);
            }
        };
        
        deadZoneSlider.minValue = 0f;
        deadZoneSlider.maxValue = mouseSettings.DeadZoneLimit;
        deadZoneSlider.onValueChanged.AddListener(ChangeDeadZoneArea);
        deadZoneInputField.onEndEdit.AddListener(ChangeDeadZoneValueOnInputField);
        
        ChangeDeadZoneValueOnInputField(mouseSettings.DeadZoneSliderValue.ToString(CultureInfo.CurrentCulture));
        ChangeDeadZoneArea(mouseSettings.DeadZoneSliderValue);
        ChangeBarPosition();
        
        float normalValue = Mathf.InverseLerp(0f, mouseSettings.DeadZoneLimit, mouseSettings.DeadZoneSliderValue);
        mouseDeadZoneArrow.ChangeArrowPositionWithLerp(normalValue);
    }
    
    private void OnDisable()
    {
        mouseDeadZoneArrow.OnArrowDrag -= ChangeDeadZoneArea;
        deadZoneSlider.onValueChanged.RemoveAllListeners();
        deadZoneInputField.onEndEdit.RemoveAllListeners();
    }

    private void OnValidate()
    {
        if (mouseSettings != null)
        {
            ChangeDeadZoneArea(deadZoneSlider.value);
        }
    }

    /// <summary>
    /// DeadZone 이미지의 크기를 조절
    /// </summary>
    /// <param name="value">슬라이더 값 0 ~ DeadZoneLimit 까지</param>
    private void ChangeDeadZoneArea(float newSliderValue)
    {
        ChangeDeadZoneValue(newSliderValue);
        CalculateDeadZoneOffset(newSliderValue);
        mouseSettings.ChangeTurnAxisSpeed(newSliderValue);
    }

    private void ChangeDeadZoneAreaColor(Color color)
    {
        deadZoneAreaImage.DOColor(color, 0.5f);
    }

    /// <summary>
    /// 슬라이더의 값을 변경
    /// </summary>
    /// <param name="newSliderValue"></param>
    private void ChangeDeadZoneValue(float newSliderValue)
    {
        mouseSettings.ChangeDeadZoneSliderValue(newSliderValue);
        deadZoneSlider.value = newSliderValue;
        deadZoneInputField.text = (Mathf.Floor(newSliderValue * 100f) / 100f).ToString(CultureInfo.CurrentCulture);
    }
    
    /// <summary>
    /// DeadZone 이미지의 Left 값을 계산
    /// </summary>
    /// <param name="newSliderValue"></param>
    private void CalculateDeadZoneOffset(float newSliderValue)
    {
        float normalValue = Mathf.InverseLerp(0f, mouseSettings.DeadZoneLimit, newSliderValue);
        float deadZoneSpriteOffset = Mathf.Lerp(1f, -(backgroundTransform.rect.width - 5f) * mouseSettings.DeadZoneLimit, normalValue);
        ChangeDeadZoneOffset(deadZoneSpriteOffset);
    }

    private void ChangeDeadZoneOffset(float offset)
    {
        deadZoneAreaTransform.localScale = new Vector3(offset, deadZoneAreaTransform.localScale.y, deadZoneAreaTransform.localScale.z);;
        //deadZoneAreaTransform.offsetMin = new Vector2(offset, deadZoneAreaTransform.offsetMin.y);
    }
    
    private void ChangeDeadZoneValueOnInputField(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            deadZoneInputField.text = "0";
            return;
        }

        if (float.TryParse(value, out float result))
        {
            if (result < 0f)
            {
                deadZoneInputField.text = "0";
                return;
            }

            if (result > mouseSettings.MouseAxisLimit)
            {
                deadZoneInputField.text = mouseSettings.MouseAxisLimit.ToString(CultureInfo.CurrentCulture);
                return;
            }

            ChangeDeadZoneValue(result);
        }
        else
        {
            deadZoneInputField.text = "0";
        }
    }
    
    /// <summary>
    /// OnEnable에서 호출하면 자동으로 Update에서 실행
    /// </summary>
    private async void ChangeBarPosition()
    {
        Vector2 barPos = valueBarTransform.anchoredPosition;
        
        while (true)
        {
            var targetValue = Mathf.Abs(mouseSettings.MouseHorizontalSpeed) * ((backgroundTransform.rect.width - 5f) / mouseSettings.MouseAxisLimit);

            // 현재 슬라이더 값을 목표 값으로 부드럽게 보간합니다.
            barPos.x = Mathf.Lerp(valueBarTransform.anchoredPosition.x, targetValue, Time.deltaTime * 100f);
            barPos.x = Mathf.Floor(barPos.x * 1000f) * 0.001f;
            valueBarTransform.anchoredPosition = barPos;

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }
}
