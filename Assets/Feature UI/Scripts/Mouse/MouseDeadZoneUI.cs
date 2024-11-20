
using System;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public class MouseDeadZoneUI : MonoBehaviour
{
    [SerializeField] private RectTransform valueBarTransform, deadZoneAreaTransform, backgroundTransform;
    [SerializeField] private TMP_InputField deadZoneInputField;
    [SerializeField] private Slider deadZoneSlider;
    private MouseSettings mouseSettings;

    private float velocity;
    
    private void OnEnable()
    {
        mouseSettings = MouseSettings.Instance;
        ChangeBarValue();
        
        deadZoneSlider.onValueChanged.AddListener(ChangeDeadZoneArea);
    }

    private void Update()
    {
        
    }

    private void ChangeDeadZoneArea(float value)
    {
        float deadZoneValue = Mathf.Lerp(795f, 400f, value);
        deadZoneAreaTransform.offsetMin = new Vector2(deadZoneValue, deadZoneAreaTransform.offsetMin.y);
    }
    
    private async void ChangeBarValue()
    {
        float targetValue = 0f;
        while (true)
        {
            targetValue = mouseSettings.MouseHorizontalSpeed * 500f;
            targetValue = Mathf.Clamp(targetValue, 0f, backgroundTransform.rect.width - 5f);
            
            // 현재 슬라이더 값을 목표 값으로 부드럽게 보간합니다.
            valueBarTransform.anchoredPosition = new Vector2(Mathf.Lerp(valueBarTransform.anchoredPosition.x, targetValue, Time.deltaTime * 5f), 0f);
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }
}
