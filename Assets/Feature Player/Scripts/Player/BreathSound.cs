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
            // Player 움직이는 상태였다가 안 움직이는 상태되면 다시 breathSequence 재생
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

        AudioManager.Instance.SetPosition(AudioManager.Instance.inhale, transform.position);
        AudioManager.Instance.SetPosition(AudioManager.Instance.exhale, transform.position);
        
        playerHeadAnimator.SetFloat("Breath Progress", breathProgress);
        playerHeadAnimator.SetFloat("Is Not Breathing", stopProgress);
    }

    public void InhaleSound()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.inhale, transform.position);
    }

    public void ExhaleSound()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.exhale, transform.position);
    }

    public void ToggleBreath()
    {
        if (PlayerConstant.isMovingState) return;
        
        if (breathSequence.IsPlaying())
        {
            AudioManager.Instance.StopSound(AudioManager.Instance.inhale, STOP_MODE.IMMEDIATE);
            AudioManager.Instance.StopSound(AudioManager.Instance.exhale, STOP_MODE.IMMEDIATE);
            breathSequence.Pause();
            DOTween.To(() => stopProgress, x => stopProgress = x, 1f, timeToStop);
            //.OnPlay(() => ); // TOOD: 숨 참는 소리 추가해야 함
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