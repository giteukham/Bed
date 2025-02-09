using System;
using System.Collections;
using System.Collections.Generic;
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
        transform.position = player.transform.position;
        AudioManager.Instance.SetPosition(AudioManager.Instance.inhale, transform.position);
        AudioManager.Instance.SetPosition(AudioManager.Instance.exhale, transform.position);
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