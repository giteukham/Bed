using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using Assets;
using DG.Tweening;
using UnityEngine;
using ConeCollider = Bed.Collider.ConeCollider;

public class DrawerGimmick : Gimmick
{
    public override GimmickType type { get; protected set; }
    public override float probability { get; set; }
    public override List<Gimmick> ExclusionGimmickList { get; set; }
    
    [SerializeField]
    private GameObject openedDrawers, closedDrawers;

    [SerializeField]
    private GameObject mom;

    private Coroutine gimmickCoroutine, shakeHeadCoroutine;
    private bool isWatching = false;
    
    [SerializeField]
    [Tooltip("초기 흔들림 강도")]
    private float shakeInitStrength = 30f;

    [SerializeField]
    [Tooltip("5초 후 Shake 강도 증가량")]
    private float shakeStrengthIncreaseValue = 1f;

    [SerializeField]
    [Tooltip("좌우로 흔들리는 범위")]
    private float shakeRange = 0.005f;

    private void Awake()
    {
        Drawers.isOpen = false;
        mom.SetActive(false);
    }

    public override void UpdateProbability()
    {
    }

    public override void Initialize()
    {
        Drawers.isOpen = false;
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
        
        mom.SetActive(true);
        Drawers.isOpen = true;

        var headMoveSpeedOverDuration = 0f;
        var shakeDuration = 0f;
        
        while (true)
        {
            if (ConeCollider.TriggeredObject &&
                ConeCollider.TriggeredObject.Equals(mom) &&
                isWatching == false)
            {
                shakeHeadCoroutine = StartCoroutine(ShakeMomHead(999f));
                isWatching = true;
                PlayerConstant.isRedemption = true;
            }

            shakeDuration += Time.deltaTime;
            if (shakeDuration >= 5f)
            {
                shakeInitStrength += Time.deltaTime * shakeStrengthIncreaseValue;
            }

            if (isWatching && PlayerConstant.headMoveSpeed >= 10f)
            {
                headMoveSpeedOverDuration += Time.deltaTime;
            }

            // HeadMoveSpeed가 10 이상 인 시간이 10초 이상이거나
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

    private IEnumerator ShakeMomHead(float duration)
    {
        var timer = 0f;
        while (true)
        {
            if (timer >= duration)
            {
                yield break;
            }
            timer += Time.deltaTime;
            
            // 좌우로 흔들리는 강도
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + shakeRange * Mathf.Sin(Time.time * shakeInitStrength));

            yield return null;
        }

    }
}
