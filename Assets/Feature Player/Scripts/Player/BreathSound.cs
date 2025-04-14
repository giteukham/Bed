using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;

public class BreathSound : MonoBehaviour
{
    [SerializeField] private Animator playerHeadAnimator;
    private Animator breathAnimator;
    
    [SerializeField] private Transform sourcePosition, sourceRotation;
    private Vector3 intervalTrnasform, breathSoundPosition;

    private void Awake()
    {
        breathAnimator = GetComponent<Animator>();
        intervalTrnasform = transform.position - sourcePosition.position;
    }

    private void Update()
    {
        breathSoundPosition = sourcePosition.position + intervalTrnasform;
        transform.position = new Vector3(breathSoundPosition.x, breathSoundPosition.y, breathSoundPosition.z);
        transform.rotation = sourceRotation.rotation;

        AudioManager.Instance.SetPosition(AudioManager.Instance.inhale, transform.position);
        AudioManager.Instance.SetPosition(AudioManager.Instance.exhale, transform.position);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleBreathSound(!breathAnimator.enabled);
        }
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
        breathAnimator.SetTrigger("toInhale");
        playerHeadAnimator.SetTrigger("toInhale");
    }
    
    public async UniTaskVoid ToExhale()
    {
        await UniTask.WaitUntil(() => !PlayerConstant.isMovingState || (!PlayerConstant.isMovingState && PlayerConstant.isPlayerStop));
        breathAnimator.SetTrigger("toExhale");
        playerHeadAnimator.SetTrigger("toExhale");
    }
    
    public void ToggleBreathSound(bool isActive)
    {
        breathAnimator.enabled = isActive;
        playerHeadAnimator.enabled = isActive;
    }
}