using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FMODUnity;
using UnityEngine;

public class BreathSound : MonoBehaviour
{
    [SerializeField] private PlayerBase player;

    private Animator headAnimator;

    private void Awake()
    {
        headAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        AudioManager.Instance.SetPosition(AudioManager.Instance.inhale, transform.position);
        AudioManager.Instance.SetPosition(AudioManager.Instance.exhale, transform.position);
    }

    public void InhaleSound()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.inhale, transform.position);
    }

    public void ExhaleSound()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.exhale, transform.position);
    }
    
    // Moving 상태가 아니여야 숨소리가 나고, 옵션 창이 켜질 때 moving 상태가 아니여야 숨소리가 계속 됨.
    public async UniTaskVoid ToInhale()
    {
        await UniTask.WaitUntil(() => !PlayerConstant.isMovingState || (!PlayerConstant.isMovingState && PlayerConstant.isPlayerStop));
        headAnimator.SetTrigger("toInhale");
    }
    
    public async UniTaskVoid ToExhale()
    {
        await UniTask.WaitUntil(() => !PlayerConstant.isMovingState || (!PlayerConstant.isMovingState && PlayerConstant.isPlayerStop));
        headAnimator.SetTrigger("toExhale");
    }
}