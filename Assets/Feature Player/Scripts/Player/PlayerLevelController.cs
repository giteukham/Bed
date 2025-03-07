using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerLevelController : MonoSingleton<PlayerLevelController>
{
    private Coroutine noiseLevelDecayRoutine, stressLevelDecayRoutine, noiseMonitorRoutine;
    [HideInInspector]public UnityEvent<int> OnNoiseChanged, OnStressChanged = new UnityEvent<int>();
    private float lowNoiseDuration = 0f;
    private int prevNoiseLevel, currentNoiseLevel;

    public void OnGameStart()
    {
        OnStressChanged.AddListener(AdjustStress);
        OnNoiseChanged.AddListener(AdjustNoise);
        
        if (noiseMonitorRoutine != null)
        StopCoroutine(noiseMonitorRoutine);
    
        noiseMonitorRoutine = StartCoroutine(MonitorNoiseLevel());
    }

    public void Initialize()
    {
        if (noiseMonitorRoutine != null) StopCoroutine(noiseMonitorRoutine);
        noiseMonitorRoutine = null;
        OnStressChanged.RemoveListener(AdjustStress);
        OnNoiseChanged.RemoveListener(AdjustNoise);
        PlayerConstant.noiseLevel = 0;
        PlayerConstant.stressLevel = 0;
        PlayerConstant.noiseStage = 0;
    }

    private IEnumerator MonitorNoiseLevel()
    {
        yield return new WaitForSeconds(9f);
        while (true)
        {
            if (PlayerConstant.noiseLevel < 30)
            {
                PlayerConstant.noiseStage = Mathf.Clamp(PlayerConstant.noiseStage - 1, PlayerConstant.noiseStageMin, PlayerConstant.noiseStageMax);
                yield return new WaitForSeconds(9f);
            }
            else
            {
                currentNoiseLevel = PlayerConstant.noiseLevel;
                if ((currentNoiseLevel / 10) - (prevNoiseLevel / 10) > 1) PlayerConstant.noiseStage = Mathf.Clamp(PlayerConstant.noiseStage + ((currentNoiseLevel / 10) - (prevNoiseLevel / 10)), PlayerConstant.noiseStageMin, PlayerConstant.noiseStageMax);
                else PlayerConstant.noiseStage = Mathf.Clamp(PlayerConstant.noiseStage + 1, PlayerConstant.noiseStageMin, PlayerConstant.noiseStageMax);
                
                if (lowNoiseDuration != 0f) lowNoiseDuration = 0f;
                prevNoiseLevel = PlayerConstant.noiseLevel;
                yield return new WaitForSeconds(9f);
            }
        }
    }

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
        yield return new WaitForSeconds(8f);

        while (PlayerConstant.stressLevel > PlayerConstant.stressLevelMin)
        {
            PlayerConstant.stressLevel--;
            yield return new WaitForSeconds(1f);
        }
    }

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
        yield return new WaitForSeconds(5f);

        while (PlayerConstant.noiseLevel > PlayerConstant.noiseLevelMin)
        {
            PlayerConstant.noiseLevel--;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
