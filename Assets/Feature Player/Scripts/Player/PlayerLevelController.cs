using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerLevelController : MonoSingleton<PlayerLevelController>
{
    private Coroutine noiseLevelDecayRoutine, stressLevelDecayRoutine, noiseMonitorRoutine, headMoveMonitorRoutine;
    [HideInInspector]public UnityEvent<int> OnNoiseChanged, OnStressChanged = new UnityEvent<int>();
    private int prevNoiseLevel, currentNoiseLevel;

    #region Level Related Values
    [SerializeField]private float stressLevel_ActivityDetectionTime;
    [SerializeField]private float stressLevel_DecreaseLatency;  
    [SerializeField]private float noiseLevel_ActivityDetectionTime;
    [SerializeField]private float noiseLevel_DecreaseLatency;
    [SerializeField]private float noiseStage_UpdateLatency;
    [SerializeField]private float headMove_MonitorLatency;
    #endregion

    private void OnValidate()
    // 값이 0이면 기본 값으로 설정
    {
        if (stressLevel_ActivityDetectionTime <= 0) stressLevel_ActivityDetectionTime = 8f;
        if (stressLevel_DecreaseLatency <= 0) stressLevel_DecreaseLatency = 1;
        if (noiseLevel_ActivityDetectionTime <= 0 ) noiseLevel_ActivityDetectionTime = 5f;
        if (noiseLevel_DecreaseLatency <= 0) noiseLevel_DecreaseLatency = 0.5f;
        if (noiseStage_UpdateLatency <= 0) noiseStage_UpdateLatency = 6f; 
        if (headMove_MonitorLatency <=0) headMove_MonitorLatency = 1f;
    }

    public void Initialize()
    {
        if (noiseMonitorRoutine != null) StopCoroutine(noiseMonitorRoutine);
        noiseMonitorRoutine = null;
        if (headMoveMonitorRoutine != null) StopCoroutine(headMoveMonitorRoutine);
        headMoveMonitorRoutine = null;
        OnStressChanged.RemoveListener(AdjustStress);
        OnNoiseChanged.RemoveListener(AdjustNoise);
        PlayerConstant.noiseLevel = 0;
        PlayerConstant.stressLevel = 0;
        PlayerConstant.noiseStage = 0;
    }

    public void OnGameStart()
    {
        OnStressChanged.AddListener(AdjustStress);
        OnNoiseChanged.AddListener(AdjustNoise);
        
        if (noiseMonitorRoutine != null) StopCoroutine(noiseMonitorRoutine);
        noiseMonitorRoutine = StartCoroutine(MonitorNoiseLevel());

        if (headMoveMonitorRoutine != null) StopCoroutine(headMoveMonitorRoutine);
        headMoveMonitorRoutine = StartCoroutine(MonitorHeadMove());
    }

    private IEnumerator MonitorNoiseLevel()
    {
        while (true)
        {
            yield return new WaitForSeconds(noiseStage_UpdateLatency);
            if (PlayerConstant.noiseLevel < 30)
                PlayerConstant.noiseStage = Mathf.Clamp(PlayerConstant.noiseStage - 1, PlayerConstant.noiseStageMin, PlayerConstant.noiseStageMax);
            else
            {
                currentNoiseLevel = PlayerConstant.noiseLevel;
                if ((currentNoiseLevel / 10) - (prevNoiseLevel / 10) > 1) PlayerConstant.noiseStage = Mathf.Clamp(PlayerConstant.noiseStage + ((currentNoiseLevel / 10) - (prevNoiseLevel / 10)), PlayerConstant.noiseStageMin, PlayerConstant.noiseStageMax);
                else PlayerConstant.noiseStage = Mathf.Clamp(PlayerConstant.noiseStage + 1, PlayerConstant.noiseStageMin, PlayerConstant.noiseStageMax);
                prevNoiseLevel = PlayerConstant.noiseLevel;
            }
        }
    }

    // 사용처
    // Gimmicks
    public void AdjustStress(int amount)
    {
        PlayerConstant.stressLevel = Mathf.Clamp(PlayerConstant.stressLevel + amount, PlayerConstant.stressLevelMin, PlayerConstant.stressLevelMax);

        if (amount >= 0)
        {
            if (stressLevelDecayRoutine != null)
            StopCoroutine(stressLevelDecayRoutine);

            stressLevelDecayRoutine = StartCoroutine(StartStressLevelDecay());
        }
    }

    private IEnumerator StartStressLevelDecay()
    {
        yield return new WaitForSeconds(stressLevel_ActivityDetectionTime);

        while (PlayerConstant.stressLevel > PlayerConstant.stressLevelMin)
        {
            PlayerConstant.stressLevel--;
            yield return new WaitForSeconds(stressLevel_DecreaseLatency);
        }
    }

    // 사용처
    // PlayerDirectionStates
    // PlayerEyeStates
    // Player
    public void AdjustNoise(int amount)
    {
        PlayerConstant.noiseLevel = Mathf.Clamp(PlayerConstant.noiseLevel + amount, PlayerConstant.noiseLevelMin, PlayerConstant.noiseLevelMax);

        if(amount >= 0)
        {
            if (noiseLevelDecayRoutine != null)
            StopCoroutine(noiseLevelDecayRoutine);

            noiseLevelDecayRoutine = StartCoroutine(StartNoiseLevelDecay());
        }
    }

    private IEnumerator StartNoiseLevelDecay()
    {
        yield return new WaitForSeconds(noiseLevel_ActivityDetectionTime);

        while (PlayerConstant.noiseLevel > PlayerConstant.noiseLevelMin)
        {
            PlayerConstant.noiseLevel--;
            yield return new WaitForSeconds(noiseLevel_DecreaseLatency);
        }
    }

    private IEnumerator MonitorHeadMove()
    {
        yield return new WaitForSeconds(headMove_MonitorLatency);
        while(true)
        {
            if (PlayerConstant.headMoveSpeed > 7) 
            { 
                OnNoiseChanged.Invoke(5);
                yield return new WaitForSeconds(headMove_MonitorLatency);
            }
            else if (PlayerConstant.headMoveSpeed <= 8 && PlayerConstant.headMoveSpeed >= 4) 
            {
                OnNoiseChanged.Invoke(0);
                yield return new WaitForSeconds(headMove_MonitorLatency);
            }
            else yield return null;
        }
    }
}
