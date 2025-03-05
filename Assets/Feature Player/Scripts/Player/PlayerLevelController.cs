using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerLevelController : MonoSingleton<PlayerLevelController>
{
    private Coroutine noiseLevelDecayRoutine, stressLevelDecayRoutine;
    [HideInInspector]public UnityEvent<int> OnNoiseChanged, OnStressChanged = new UnityEvent<int>();

    private void Awake()
    {
        OnStressChanged.AddListener(AdjustStress);
        OnNoiseChanged.AddListener(AdjustNoise);
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
