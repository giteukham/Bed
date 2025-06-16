using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using Assets;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using ConeCollider = Bed.Collider.ConeCollider;

public class DrawerGimmick : Gimmick
{
    public override GimmickType type { get; protected set; }
    public override float probability { get; set; }
    public override List<Gimmick> ExclusionGimmickList { get; set; }

    [SerializeField]
    private GameObject mom;

    [SerializeField]
    private Transform head;

    [SerializeField]
    [Tooltip("머리를 좌우로 돌리는 최대 각도")]
    private float turnMaxAngle = 20f;

    [SerializeField]
    [Tooltip("머리를 좌우로 돌리는 시간")]
    private float turnDuration = 0.05f;

    private Coroutine gimmickCoroutine, shakeHeadCoroutine;
    private Coroutine headMovementCoroutine;
    private bool isWatching = false;
    private float turnAngleValue = 0f;

    /// <summary>
    /// 쳐다보고 2초 안에 파훼하면 페널티 없음
    /// </summary>
    private bool hasPanalty = false;
    
    private void Awake()
    {
        mom.SetActive(false);
    }

    public override void UpdateProbability()
    {
    }

    public override void Initialize()
    {
        Drawer.Open(false);
        mom.SetActive(false);
    }

    public override void Activate()
    {
        base.Activate();
        if (gimmickCoroutine != null)
        {
            StopCoroutine(gimmickCoroutine);
            gimmickCoroutine = null;
        }

        if (shakeHeadCoroutine != null)
        {
            StopCoroutine(shakeHeadCoroutine);
            shakeHeadCoroutine = null;
        }

        gimmickCoroutine = StartCoroutine(StartGimmick());
    }

    public override void Deactivate()
    {
        base.Deactivate();
        PlayerConstant.isRedemption = false;
        isWatching = false;
    }

    private IEnumerator StartGimmick()
    {
        // 서랍장 열리는 소리
        // AudioManager.Instance.Play("", mom.transform.position);
        
        yield return new WaitUntil(() => 
            PlayerConstant.isLeftLook || 
            PlayerConstant.isLeftState || 
            PlayerConstant.isLeftFrontLook);
        
        Drawer.Open(true);
        mom.SetActive(true);

        var headMoveSpeedOverDuration = 0f;
        var shakeDuration = 0f;
        
        while (true)
        {
            if (ConeCollider.TriggeredObject &&
                ConeCollider.TriggeredObject.Equals(mom) &&
                isWatching == false)
            {
                TurnSideToSideMomHead();
                isWatching = true;
                PlayerConstant.isRedemption = true;
            }
            
            if (isWatching && PlayerConstant.headMoveSpeed >= 10f)
            {
                headMoveSpeedOverDuration += Time.deltaTime;
            }

            // HeadMoveSpeed가 10 이상 인 시간이 1초 이상이거나
            // 닫히는 소리가 들리면 파훼
            if (headMoveSpeedOverDuration >= 1f) 
                //|| AudioManager.Instance.DuplicateCheck(""))
            {
                Deactivate();
                yield break;
            }

            yield return null;
        }
    }

    private void TurnSideToSideMomHead()
    {
        if (headMovementCoroutine != null)
        {
            StopCoroutine(headMovementCoroutine);
        }

        DOTween.To(() => turnAngleValue, x => turnAngleValue = x, turnMaxAngle, 10f)
            .SetDelay(2f)
            .OnPlay(() => hasPanalty = true); 
        
        headMovementCoroutine = StartCoroutine(HeadMovementLoop());
    }

    private IEnumerator HeadMovementLoop()
    {
        while (true)
        {
            var headY = head.localRotation.y;
            var headX = head.localRotation.x;
            
            var left = new Vector3(headX, headY - turnAngleValue, head.localRotation.z);
            var right = new Vector3(headX, headY + turnAngleValue, head.localRotation.z);
            var up = new Vector3(headX + turnAngleValue, headY, head.localRotation.z);
            var down = new Vector3(headX - turnAngleValue, headY, head.localRotation.z);
        
            yield return head.DOLocalRotate(left, turnDuration).WaitForCompletion();
            yield return head.DOLocalRotate(right, turnDuration).WaitForCompletion();
            yield return head.DOLocalRotate(up, turnDuration).WaitForCompletion();
            yield return head.DOLocalRotate(down, turnDuration).WaitForCompletion();
        }
    }

    private void StopHeadMovement()
    {
        if (headMovementCoroutine != null)
        {
            StopCoroutine(headMovementCoroutine);
            headMovementCoroutine = null;
        }
    }
}
