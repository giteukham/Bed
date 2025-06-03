using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using AbstractGimmick;
using Assets;
using UnityEngine;
using UnityEngine.Serialization;
using DG.Tweening;
using ConeCollider = Bed.Collider.ConeCollider;
using Unity.VisualScripting;
using Sequence = DG.Tweening.Sequence;
using UnityEngine.UI;

public class MonitorGimmick : Gimmick
{
    [SerializeField]
    private GameObject monitor;

    [SerializeField]
    private GameObject monitorScreen;

    [SerializeField]
    private GameObject monitorMan;

    [SerializeField]
    private GameObject mirror;

    [SerializeField]
    private GameObject blackScreen;
    [SerializeField]
    private Transform monitorManArm, mirrorArm, monitorManArmHand, mirrorArmHand;
    [SerializeField]
    private GameObject monitorManArmInitPosition, mirrorArmInitPosition, monitorManArmTargetPosition, mirrorArmTargetPosition; 

    [SerializeField]
    [Tooltip("팔 나오기 위해 모니터맨을 보는 시간")]
    private float lookingThresholdTime = 8f;

    [SerializeField]
    [Tooltip("팔이 뻗쳐 나오는 시간")]
    private float armStretchTime = 0.3f;

    public override GimmickType type { get; protected set; }
    public override float probability { get; set; }
    public override List<Gimmick> ExclusionGimmickList { get; set; }

    public override void UpdateProbability()
    {
        probability = 100;
    }

    public override void Initialize()
    {
        blackScreen.SetActive(false);
        
        AudioManager.Instance.AllVoumeInit();
        blackScreen.GetComponent<RawImage>().DOColor(Color.white, 0f);
    }

    public override void Activate()
    {
        base.Activate();
        StartCoroutine(StartGimmick());
    }

    public override void Deactivate()
    {
        base.Deactivate();
        gameObject.SetActive(false);
        
    }

    private IEnumerator StartGimmick()
    {
        monitorScreen.SetActive(true);

        // 눈 감은 횟수 기록용
        var prevBlinkCount = PlayerConstant.EyeBlinkCAT;
        while (true)
        {
            // 모니터를 쳐다보면서 눈 감을 때까지 기다림
            if (ConeCollider.TriggeredObject &&
                ConeCollider.TriggeredObject.Equals(monitorScreen) &&
                prevBlinkCount != PlayerConstant.EyeBlinkCAT)
            {
                break;
            }

            // 이 if문이 없는 상태에서 모니터를 안 보고 눈을 감으면
            // 위 if문에서 prevBlinkCount != PlayerConstant.EyeBlinkCAT 이 부분이 true가 되고
            // monitorScreen에 닿자마자 나머지 조건들도 true가 되버려서 monitorMan이 켜짐
            if (!ConeCollider.TriggeredObject &&
                prevBlinkCount != PlayerConstant.EyeBlinkCAT)
            {
                prevBlinkCount++;
            }

            yield return null;
        }

        monitor.SetActive(false);
        monitorScreen.SetActive(false);
        monitorMan.SetActive(true);
        
        // Monitor Man 쳐다볼 경우 경과 시간
        var elapsedTimeLook = 0f;
        bool onMonitorManArm = false, onMirrorArm = false;
        
        while (true)
        {
            // Monitor 맨을 안 쳐다볼 경우 쳐다보고 있던 시간은 초기화
            if (!ConeCollider.TriggeredObject ||
                !ConeCollider.TriggeredObject.Equals(monitorMan))
            {
                elapsedTimeLook = 0f;
            }
            else if (ConeCollider.TriggeredObject && 
                     ConeCollider.TriggeredObject.Equals(monitorMan))
            {
                elapsedTimeLook += Time.deltaTime;
            }

            if (elapsedTimeLook >= lookingThresholdTime)
            {
                onMonitorManArm = true;
                break;
            }

            if (PlayerConstant.isLeftState)
            {
                yield return new WaitForSeconds(0.4f);
                onMirrorArm = true;
                break;
            }

            yield return null;
        }

        if (onMonitorManArm == true)
        {
            Quaternion originalRotation = monitorManArmHand.rotation;

            Sequence seq = DOTween.Sequence();
            seq.Append(monitorManArm.DOMove(monitorManArmTargetPosition.transform.position, armStretchTime))
            .Join(monitorManArmHand.DOShakeRotation(armStretchTime, strength: 10f, vibrato: 30, randomness: 90, fadeOut: false))
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                StartCoroutine(BlackOutCoroutine());
                monitorManArm.position = monitorManArmInitPosition.transform.position;
                monitorManArmHand.rotation = originalRotation;
            });
        }
        else if (onMirrorArm == true)
        {
            Quaternion originalRotation = mirrorArmHand.rotation;

            Sequence seq = DOTween.Sequence();
            seq.Append(mirrorArm.DOMove(mirrorArmTargetPosition.transform.position, armStretchTime))
            .Join(mirrorArmHand.DOShakeRotation(armStretchTime, strength: 10f, vibrato: 30, randomness: 90, fadeOut: false))
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                StartCoroutine(BlackOutCoroutine());
                mirrorArm.position = mirrorArmInitPosition.transform.position;
                mirrorArmHand.rotation = originalRotation;
            });
        }
    }

    private IEnumerator BlackOutCoroutine()
    {
        blackScreen.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        blackScreen.GetComponent<RawImage>().DOColor(Color.black, 0.07f);
        AudioManager.Instance.AllVolumeDown(0);
        monitorScreen.SetActive(false);
        monitorMan.SetActive(false);
        monitor.SetActive(true);
        
        yield return new WaitForSeconds(4f);

        blackScreen.SetActive(false);
        AudioManager.Instance.AllVoumeInit();
        
        Deactivate();
    }
}
