using Cinemachine;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;


public class PlayerAnimation : MonoBehaviour
{
    private static Animator playerAnimator;
    private static Animator blanketAnimator;
    [SerializeField] private Transform blanket;
    private static Vector3 blanketPosition;

    private void Start()
    {
        TryGetComponent(out playerAnimator);
        blanketAnimator = blanket.GetComponent<Animator>();
        blanketPosition = blanket.transform.position;
    }
    
    public static void PlayAnimation(string triggerName)
    {
        playerAnimator.SetTrigger(triggerName);
        blanketAnimator.SetTrigger(triggerName);
        AudioManager.instance.PlayOneShot(AudioManager.instance.blanketMoving, blanketPosition);
        PlayerConstant.BodyMovementCAT++;
        PlayerConstant.BodyMovementLAT++;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="isPause">true면 정지, false면 다시 재생</param>
    public static void PauseAndResumeAnimation(bool isPause)
    {
        if (isPause)
        {
            playerAnimator.speed = 0;
            blanketAnimator.speed = 0;
        }
        else
        {
            playerAnimator.speed = 1;
            blanketAnimator.speed = 1;
        }
    }
}