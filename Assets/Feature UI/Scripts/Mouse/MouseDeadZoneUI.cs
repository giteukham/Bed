
using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class MouseDeadZoneUI : MonoBehaviour
{
    [SerializeField] private RectTransform valueBarTransform, backgroundTransform, deadZoneAreaTransform;
    [SerializeField] private TMP_InputField deadZoneInputField;
    [SerializeField] private Slider deadZoneSlider;

    private MouseSettings mouseSettings;
    private float normalValue;

    public RectTransform BackgroundTransform => backgroundTransform;

    public static event Action OnDeadZoneOffsetChange;

    private void OnEnable()
    {
        mouseSettings = MouseSettings.Instance;
        MouseDeadZoneArrow.OnArrowDrag += ChangeDeadZoneArea;
        
        deadZoneSlider.minValue = 0f;
        deadZoneSlider.maxValue = mouseSettings.MouseAxisLimit;
        deadZoneSlider.onValueChanged.AddListener(ChangeDeadZoneArea);
        deadZoneInputField.onEndEdit.AddListener(ChangeDeadZoneValueOnInputField);
        
        ChangeDeadZoneArea(mouseSettings.DeadZoneSliderValue);
        ChangeBarPosition();
    }
    
    private void OnDisable()
    {
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
    /// DeadZone �̹����� ũ�⸦ ����
    /// </summary>
    /// <param name="value">�����̴� �� 0 ~ MouseAxisLimit ����</param>
    private void ChangeDeadZoneArea(float value)
    {
        ChangeDeadZoneValue(value);
        CalculateDeadZoneOffset(value);
        mouseSettings.ChangeTurnAxisSpeed(Mathf.Lerp(mouseSettings.MouseAxisLimit, 0.1f, normalValue));
    }

    /// <summary>
    /// �����̴��� ���� ����
    /// </summary>
    /// <param name="newSliderValue"></param>
    private void ChangeDeadZoneValue(float newSliderValue)
    {
        mouseSettings.ChangeDeadZoneSliderValue(newSliderValue);
        deadZoneSlider.value = newSliderValue;
        deadZoneInputField.text = (Mathf.Floor(newSliderValue * 100f) / 100f).ToString(CultureInfo.CurrentCulture);
    }
    
    /// <summary>
    /// DeadZone �̹����� Left ���� ���
    /// </summary>
    /// <param name="newSliderValue"></param>
    private void CalculateDeadZoneOffset(float newSliderValue)
    {
        normalValue = Mathf.InverseLerp(0f, mouseSettings.MouseAxisLimit, newSliderValue);
        float deadZoneSpriteOffset = Mathf.Lerp(1f, -(backgroundTransform.rect.width - 5f) * mouseSettings.DeadZoneLimit, normalValue);
        ChangeDeadZoneOffset(deadZoneSpriteOffset);
    }

    private void ChangeDeadZoneOffset(float offset)
    {
        deadZoneAreaTransform.localScale = new Vector3(offset, deadZoneAreaTransform.localScale.y, deadZoneAreaTransform.localScale.z);;
        //deadZoneAreaTransform.offsetMin = new Vector2(offset, deadZoneAreaTransform.offsetMin.y);
        OnDeadZoneOffsetChange?.Invoke();
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
    /// OnEnable���� ȣ���ϸ� �ڵ����� Update���� ����
    /// </summary>
    private async void ChangeBarPosition()
    {
        float targetValue = 0f;
        Vector2 barPos = valueBarTransform.anchoredPosition;
        
        while (true)
        {
            targetValue = Mathf.Abs(mouseSettings.MouseHorizontalSpeed) * ((backgroundTransform.rect.width - 5f) / mouseSettings.MouseAxisLimit);

            // ���� �����̴� ���� ��ǥ ������ �ε巴�� �����մϴ�.
            barPos.x = Mathf.Lerp(valueBarTransform.anchoredPosition.x, targetValue, Time.deltaTime * 100f);
            barPos.x = Mathf.Floor(barPos.x * 1000f) * 0.001f;
            valueBarTransform.anchoredPosition = barPos;

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }
}
