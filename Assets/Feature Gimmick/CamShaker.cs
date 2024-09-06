using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// 기믹의 컴포넌트로 들어가는 카메라 쉐이킹 스크립트
/// </summary>
public class CamShaker : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;

    [SerializeField]
    private CinemachineVirtualCamera virtualCamera; // 가상카메라
    private CinemachineTransposer transposer;       // 가상카메라의 offset에 접근하기 위한 변수

    private IEnumerator currentCoroutine;           // 코루틴을 안전하게 관리하기 위한 변수

    private void Awake()
    {
        //본인에게 있는 CinemachineImpulseSource 컴포넌트를 가져옴
        impulseSource = GetComponent<CinemachineImpulseSource>();

        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        //ShakeCam(10, 1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ShakeCam(5, 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            StopShakeCam();
        }
    }

    /// <summary>
    /// 카메라 흔드는 메소드
    /// </summary>
    /// <param name="shakeTime"></param> 카메라 흔들림 지속 시간
    /// <param name="shakePower"></param> 흔들림 강도(0.01 ~ 0.5 범위 추천함)
    public void ShakeCam(float shakeTime, float shakePower)
    {
        currentCoroutine = ShakeOffset(shakeTime, shakePower);
        StartCoroutine(currentCoroutine);
    }

    /// <summary>
    /// 카메라 쉐이킹 종료
    /// </summary>
    public void StopShakeCam()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
            //카메라 원위치
            transposer.m_FollowOffset = Vector3.zero;
        }

    }

    private IEnumerator ShakeOffset(float shakeTime, float shakePower)
    {
        float duration = shakeTime;
        //bool test = false;
        //지속시간만큼 흔들기
        while (duration > 0.0f)
        {
            yield return null;


            /*if (duration < 5f && test == false)
            {
                shakePower *= 10;
                test = true;
            }*/

            transposer.m_FollowOffset = Random.insideUnitSphere * shakePower;
            duration -= Time.deltaTime;
        }
        transposer.m_FollowOffset = Vector3.zero;
    }


    /// <summary>
    /// 카메라 흔들림 단 1회 실행
    /// </summary>
    /// <param name="power"></param>
    public void MakeImpulse(float power)
    {
        impulseSource.GenerateImpulseWithForce(power);
    }

    public void DeleteImpulse()
    {
        //존재하는 모든 Impulse를 지우는 코드
        //하지만 현재 오류로 적용되지 않고 있음
        //시네머신 3 버전에서는 오류가 없다고 함
        CinemachineImpulseManager.Instance.Clear();
    }
}
