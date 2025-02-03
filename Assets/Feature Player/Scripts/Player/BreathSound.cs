using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathSound : MonoBehaviour
{
    private Animator headAnimator;
    
    private void Start()
    {
        headAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        StopBreath(PlayerConstant.isMovingState);
    }

    public void InhaleSound()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.inhale, transform.position);
    }

    public void ExhaleSound()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.exhale, transform.position);
    }

    public void StopBreath(bool isStop)
    {
        headAnimator.SetBool("isStop", isStop);
    }
    
    public void ToInhale()
    {
        headAnimator.SetTrigger("toInhale");
    }
    
    public void ToExhale()
    {
        headAnimator.SetTrigger("toExhale");
    }
}