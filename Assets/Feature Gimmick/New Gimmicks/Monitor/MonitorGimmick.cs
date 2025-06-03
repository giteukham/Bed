using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using AbstractGimmick;
using Assets;
using UnityEngine;
using UnityEngine.Serialization;
using ConeCollider = Bed.Collider.ConeCollider;

public class MonitorGimmick : Gimmick
{
    [SerializeField]
    private GameObject monitor;

    [SerializeField]
    private GameObject monitorScreen;

    [SerializeField]
    private GameObject monitorMan;

    [SerializeField]
    [Tooltip("모니터 켜지기 위해 오른쪽 바라봐야 하는 시간")]
    private float lookRightTimeForMonitor = 5f;

    [SerializeField]
    [Tooltip("파훼를 위해 안 쳐다봐야 하는 시간")]
    private float notLookForClear = 5f;

    [SerializeField]
    [Tooltip("모니터 맨을 n초 이상 쳐다보면 손 튀어나오는 시간")]
    private float lookingTimeForSticksOutHand = 5f;

    private Coroutine gimmickCoroutine;

    public override GimmickType type { get; protected set; }
    public override float probability { get; set; }
    public override List<Gimmick> ExclusionGimmickList { get; set; }

    public override void UpdateProbability()
    {
        probability = 100;
    }

    public override void Initialize()
    {
        monitorScreen.SetActive(false);
        monitorMan.SetActive(false);
        monitor.SetActive(true);
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
        // 오른쪽 볼 때 경과 시간
        var elapsedLookRight = 0f;
        // while (true)
        // {
        //     // 오른쪽을 바라보는 시간이 n초 이상이거나 오른쪽으로 몸 돈 상태면 모니터 On
        //     if (elapsedLookRight >= lookRightTimeForMonitor || PlayerConstant.isRightState)
        //     {
        //         break;
        //     }
            
        //     if (PlayerConstant.isRightFrontLook)
        //     {
        //         elapsedLookRight += Time.deltaTime;
        //     }
        //     else
        //     {
        //         elapsedLookRight = 0f;
        //     }
           
        //     yield return null;
        // }
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

        // Monitor Man 안 쳐다볼 경우 경과 시간
        var elapsedNotLookForClear = 0f;
        
        // Monitor Man 쳐다볼 경우 경과 시간
        var elapsedLook = 0f;
        
        while (true)
        {
            // Monitor 맨을 안 쳐다볼 경우 쳐다보고 있던 시간은 초기화
            if (!ConeCollider.TriggeredObject ||
                !ConeCollider.TriggeredObject.Equals(monitorMan))
            {
                elapsedLook = 0f;
                elapsedNotLookForClear += Time.deltaTime;
            }
            else if (ConeCollider.TriggeredObject && ConeCollider.TriggeredObject.Equals(monitorMan))
            {
                elapsedNotLookForClear = 0f;
                elapsedLook += Time.deltaTime;
            }

            // 쳐다보는 시간이 lookingTimeForSticksOutHand 초 이상일 경우 while 정지
            // if (elapsedLook >= lookingTimeForSticksOutHand)
            // {
            //     break;
            // }

            // 왼쪽을 보거나 안 쳐다보는 시간 n초 후 파훼
            // if (PlayerConstant.isLeftState || elapsedNotLookForClear >= notLookForClear)
            // {
            //     Deactivate();
            //     yield break;
            // }

            yield return null;
        }
        
        // 손 튀어나오는 기믹 추가
    }
}
