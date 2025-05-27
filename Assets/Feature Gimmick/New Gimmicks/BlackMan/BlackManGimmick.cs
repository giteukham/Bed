using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using AbstractGimmick;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class BlackManGimmick : Gimmick
{
    [SerializeField]
    [Tooltip("객체가 올라오는 속도")]
    private float risingSpeed = 5f;
    
    public override GimmickType type { get; protected set; }
    public override float probability { get; set; }
    public override List<Gimmick> ExclusionGimmickList { get; set; }
    public override void UpdateProbability()
    {
        probability = 100;
    }

    public override void Initialize()
    {
    }
    
    private void Start()
    {
        StartCoroutine(StartGimmick());
    }

    public override void Activate()
    {
        base.Activate();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        gameObject.SetActive(false);
    }

    private IEnumerator StartGimmick()
    {
        const float endYPos = 1.5f;
        var prevBlinkCount = PlayerConstant.EyeBlinkCAT;
        Coroutine stressLevelCoroutine = null;
        
        while (true)
        {
            // 다 올라오고 난 후 쳐다보면서 눈을 깜빡이면 파훼
            if (transform.position.y >= endYPos &&
                isDetected == true &&
                PlayerConstant.EyeBlinkCAT > prevBlinkCount)
            {
                Deactivate();
                yield break;
            }
            
            // 2초마다 5 stress가 증가
            if (isDetected == true)
            {
                stressLevelCoroutine ??= StartCoroutine(IncreaseStressLevel(5, 2));
            }
            else if (isDetected == false && stressLevelCoroutine != null)
            {
                StopCoroutine(stressLevelCoroutine);
                stressLevelCoroutine = null;
            }

            // 객체가 점점 올라감
            if (transform.position.y < endYPos)
            {
                transform.position += Vector3.up * (Time.deltaTime * risingSpeed);
            }

            yield return null;
        }
    }

    private IEnumerator IncreaseStressLevel(int amount, float duration)
    {
        while (true)
        {
            if (isDetected == false)
            {
                yield break;
            }

            yield return new WaitForSeconds(duration);
            PlayerLevelController.Instance.OnStressChanged.Invoke(amount);
        }
    }
}
