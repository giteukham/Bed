using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Serialization;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class BreathSound : MonoBehaviour
{
    [SerializeField] private Animator playerHeadAnimator;
    
    [SerializeField] private Transform sourcePosition, sourceRotation;
    private Vector3 intervalTrnasform, breathSoundPosition;

    [Header("Breath Settings")]
    [SerializeField] private float breathTime = 2f;
    [SerializeField] private float timeToStop = 0.2f;
    
    private float breathProgress = 0f, stopProgress = 0f;
    private bool isBreathing = true;
    
    private Sequence breathSequence;
    private Guid inahleGuid, exhaleGuid;

    private void Awake()
    {
        intervalTrnasform = transform.position - sourcePosition.position;
    }

    private void Start()
    {
        breathProgress = 0f;
        stopProgress = 0f;
        TakeBreath();
    }

    private void TakeBreath()
    {
        breathSequence = DOTween.Sequence();
        breathSequence.Append(DOTween.To(() => breathProgress, x => breathProgress = x, 1f, breathTime));
        breathSequence.InsertCallback(0, InhaleSound);
        breathSequence.Append(DOTween.To(() => breathProgress, x => breathProgress = x, 0f, breathTime));
        breathSequence.InsertCallback(breathTime, ExhaleSound);
        breathSequence.OnUpdate(async () =>
        {
            if (PlayerConstant.isMovingState)
            {
                breathSequence.Pause();
                await UniTask.WaitUntil(() => !PlayerConstant.isMovingState);
                breathSequence.Play();
            }
        })
        .SetLoops(-1);
    }

    private void Update()
    {
        breathSoundPosition = sourcePosition.position + intervalTrnasform;
        transform.position = new Vector3(breathSoundPosition.x, breathSoundPosition.y, breathSoundPosition.z);
        transform.rotation = sourceRotation.rotation;

        if (inahleGuid != Guid.Empty) AudioManager.Instance.SetPosition(inahleGuid, transform.position);
        if (exhaleGuid != Guid.Empty) AudioManager.Instance.SetPosition(exhaleGuid, transform.position);
        
        playerHeadAnimator.SetFloat("Breath Progress", breathProgress);
        playerHeadAnimator.SetFloat("Is Not Breathing", stopProgress);
    }

    public void InhaleSound()
    {
        inahleGuid = AudioManager.Instance.PlayForce(AudioKeys.Inhale, transform.position);
    }

    public void ExhaleSound()
    {
        exhaleGuid = AudioManager.Instance.PlayForce(AudioKeys.Exhale, transform.position);
    }

    public void ToggleBreath()
    {
        if (PlayerConstant.isMovingState) return;
        
        if (breathSequence.IsPlaying())
        {
            AudioManager.Instance.StopSound(inahleGuid, STOP_MODE.IMMEDIATE);
            inahleGuid = Guid.Empty;
            AudioManager.Instance.StopSound(exhaleGuid, STOP_MODE.IMMEDIATE);
            exhaleGuid = Guid.Empty;
            breathSequence.Pause();
            DOTween.To(() => stopProgress, x => stopProgress = x, 1f, timeToStop);
        }
        else
        {
            DOTween.To(() => stopProgress, x =>
                {
                    stopProgress = x;
                    breathProgress = x;
                }, 0f, breathTime)
                .OnPlay(ExhaleSound)
                .OnComplete(() => breathSequence.Restart());
        }
    }
}