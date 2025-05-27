using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbstractGimmick;
using System;

enum MovingState
{
    None,
    Left,
    Right
}

public class CatGimmick : Gimmick
{
    #region Override Variables
    [field: SerializeField] public override GimmickType type { get; protected set; }
    [SerializeField] private float _probability;
    public override float probability 
    { 
        get => _probability; 
        set => _probability = Mathf.Clamp(value, 0, 100); 
    }
    [field: SerializeField] public override List<Gimmick> ExclusionGimmickList { get; set; }
    [SerializeField]private Animator animator;
    #endregion

    #region Variables
    [SerializeField] private int moveCount;
    private Coroutine checkMovingCoroutine;
    private MovingState movingState = MovingState.None;
    #endregion
    private void Awake()
    {
    }

    public override void Activate()
    {
        base.Activate();
        StartCoroutine(MainCode());
    }

    public override void Deactivate()
    {
        base.Deactivate();
        gameObject.SetActive(false);
    }

    private IEnumerator MainCode()
    {
        PlayerConstant.isRedemption = true;
        int chance = UnityEngine.Random.Range(0, 100);
        if (chance < 11)
            animator.Play("ChipiChipiChapaChapa");
        else
            animator.Play("Loaf");
        
        checkMovingCoroutine ??= StartCoroutine(CheckMovingCoroutine());
        while (true)
        {
            if (moveCount >= 3)
            {
                StopCoroutine(checkMovingCoroutine);
                checkMovingCoroutine = null;
                PlayerConstant.isRedemption = false;
                if(movingState == MovingState.Left)
                    animator.Play("RunToRight");
                else if(movingState == MovingState.Right)
                    animator.Play("RunToLeft");
                yield return new WaitForSeconds(0.15f);
                Deactivate();
            }
            yield return null;
        }
    }

    IEnumerator CheckMovingCoroutine()
    {
        bool isMoving = false;
        while (true)
        {
            
            if (MouseSettings.Instance.MouseHorizontalSpeed >= MouseSettings.Instance.TurnRightSpeed)
            {
                movingState = MovingState.Left;
                moveCount ++;
                isMoving = true;
            }
            else if(MouseSettings.Instance.MouseHorizontalSpeed <= MouseSettings.Instance.TurnLeftSpeed)
            {
                movingState = MovingState.Right;
                moveCount ++;
                isMoving = true;
            }
    
            if(isMoving) 
            {
                yield return new WaitForSeconds(0.2f);
                isMoving = false;
            }
            else yield return null;
        }
    }

    public override void UpdateProbability()
    {
        //throw new System.NotImplementedException();
    }
    public override void Initialize()
    {
        moveCount = 0;
        movingState = MovingState.None;
    }
}
