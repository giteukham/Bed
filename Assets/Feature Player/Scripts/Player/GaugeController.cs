using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeController : MonoBehaviour
{
    private Coroutine fearGaugeCoroutine, stressGaugeCoroutine;

    public enum GaugeTypes
    {
        Fear,
        Stress
    }
    
    private float fearTargetValue, stressTargetValue = 0;

    public static GaugeController instance { get; private set;}

    private void Awake() 
    {
        if (instance != null) Debug.LogError("GaugeController already exists");
        instance = this;
    }

    /// <summary>
    /// 게이지 설정, 부호 붙여서 사용
    /// </summary>
    /// <param name="type">게이지 타입</param>
    /// <param name="value">수치</param>
    public void SetGuage(GaugeTypes type, float value)
    {
        if (type == GaugeTypes.Fear) 
        {
            if (fearGaugeCoroutine != null) StopCoroutine(fearGaugeCoroutine);
            fearTargetValue += value;
            fearTargetValue = Mathf.Clamp(fearTargetValue, 0, 100);
            fearGaugeCoroutine = StartCoroutine(UpdateGauge(type, fearTargetValue));
        }
        else 
        {
            if (stressGaugeCoroutine != null) StopCoroutine(stressGaugeCoroutine);
            stressTargetValue += value;
            stressTargetValue = Mathf.Clamp(stressTargetValue, 0, 100);
            stressGaugeCoroutine = StartCoroutine(UpdateGauge(type, stressTargetValue));
        }
    }

    private IEnumerator UpdateGauge(GaugeTypes type, float targetValue)
    {
        float startValue;
        float duration = 1f;
        float elapsed = 0f;

        if (type == GaugeTypes.Fear) 
        {
            startValue = PlayerConstant.fearGauge;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                PlayerConstant.fearGauge = (int)Mathf.Lerp(startValue, targetValue, elapsed / duration);
                yield return null; 
            }
            PlayerConstant.fearGauge = (int)targetValue;
            fearTargetValue = PlayerConstant.fearGauge;
        }
        else 
        {
            startValue = PlayerConstant.stressGauge;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                PlayerConstant.stressGauge = (int)Mathf.Lerp(startValue, targetValue, elapsed / duration);
                yield return null; 
            }
            PlayerConstant.stressGauge = (int)targetValue;
            stressTargetValue = PlayerConstant.stressGauge;
        }
        fearGaugeCoroutine = null;
    }
}
