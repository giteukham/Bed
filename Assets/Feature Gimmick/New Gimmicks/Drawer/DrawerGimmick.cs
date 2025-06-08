using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using Assets;
using UnityEngine;
using ConeCollider = Bed.Collider.ConeCollider;

public class DrawerGimmick : Gimmick
{
    public override GimmickType type { get; protected set; }
    public override float probability { get; set; }
    public override List<Gimmick> ExclusionGimmickList { get; set; }

    [SerializeField]
    private GameObject mom;

    private Coroutine gimmickCoroutine;
    private bool isWatching = false;

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

        gimmickCoroutine = StartCoroutine(StartGimmick());
    }

    public override void Deactivate()
    {
        base.Deactivate();
        PlayerConstant.isMouseMoveParalysis = false;
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

        var timer = 0f;
        var prevBlinkCount = PlayerConstant.EyeBlinkCAT;
        
        while (true)
        {
            // mom 쳐다보면 5초 간 시선 고정
            if (ConeCollider.TriggeredObject &&
                ConeCollider.TriggeredObject.Equals(mom) &&
                isWatching == false)
            {
                isWatching = true;
                PlayerConstant.isMouseMoveParalysis = true;
                PlayerConstant.isRedemption = true;
            }

            if (isWatching)
            {
                timer += Time.deltaTime;
            }

            // 쳐다보는 시간이 5초를 넘거나
            // 쳐다보면서 눈을 감거나
            // 닫히는 소리가 들리면 파훼
            if (timer >= 5f ||
                (isWatching && (!PlayerConstant.isEyeOpen || PlayerConstant.EyeBlinkCAT != prevBlinkCount))) 
                //|| AudioManager.Instance.DuplicateCheck(""))
            {
                Deactivate();
                yield break;
            }

            yield return null;
        }
    }
}
