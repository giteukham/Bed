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
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.blanketMoving, blanketPosition);
        PlayerConstant.BodyMovementCAT++;
        PlayerConstant.BodyMovementLAT++;
    }
}