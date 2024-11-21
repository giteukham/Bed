
using System;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class MouseDeadZoneUI : MonoBehaviour
{
    [SerializeField] private RectTransform valueBarTransform, backgroundTransform, deadZoneAreaTransform;
    [SerializeField] private TMP_InputField deadZoneInputField;
    [SerializeField] private Slider deadZoneSlider;
    [SerializeField, Range(0.1f, 1f), Tooltip("Deadzone 영역의 한계값")] private float deadZoneLimit;
    private MouseSettings mouseSettings;
    
    private void OnEnable()
    {
        mouseSettings = MouseSettings.Instance;

        deadZoneSlider.minValue = -mouseSettings.MouseAxisLimit;
        deadZoneSlider.maxValue = mouseSettings.MouseAxisLimit;
        deadZoneSlider.onValueChanged.AddListener(ChangeDeadZoneArea);

        ChangeBarPosition();
        
    }

    private void OnValidate()
    {
        if (mouseSettings != null)
        {
            ChangeDeadZoneArea(deadZoneSlider.value);
        }
    }


    private void ChangeDeadZoneArea(float value)
    {
        float minMouseAxis = -mouseSettings.MouseAxisLimit;
        float maxMouseAxis = mouseSettings.MouseAxisLimit;
        float lerpValue = Mathf.InverseLerp(minMouseAxis, maxMouseAxis, value);
        float deadZoneValue = Mathf.Lerp(backgroundTransform.rect.width - 5f, (backgroundTransform.rect.width - 5f) * deadZoneLimit, lerpValue);

        mouseSettings.ChangeTurnAxisSpeed(Mathf.Lerp(mouseSettings.MouseAxisLimit, 0.1f, lerpValue));
        deadZoneAreaTransform.offsetMin = new Vector2(deadZoneValue, deadZoneAreaTransform.offsetMin.y);
    }
    
    private async void ChangeBarPosition()
    {
        float targetValue = 0f;
        float x;
        while (true)
        {
            targetValue = Mathf.Abs(mouseSettings.MouseHorizontalSpeed) * ((backgroundTransform.rect.width - 5f) / mouseSettings.MouseAxisLimit);

            // 현재 슬라이더 값을 목표 값으로 부드럽게 보간합니다.
            x = Mathf.Lerp(valueBarTransform.anchoredPosition.x, targetValue, Time.deltaTime * 5f);
            x = Mathf.Floor(x * 100f) * 0.01f;
            valueBarTransform.anchoredPosition = new Vector2(x, 0f);

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }
}
