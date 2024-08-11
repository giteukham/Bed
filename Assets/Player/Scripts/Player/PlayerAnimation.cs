using Cinemachine;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;


public class PlayerAnimation : MonoBehaviour
{
    private static Animator playerAnimator;

    private void Start()
    {
        TryGetComponent(out playerAnimator);
    }
    
    public static void PlayAnimation(string triggerName)
    {
        playerAnimator.SetTrigger(triggerName);
    }
}