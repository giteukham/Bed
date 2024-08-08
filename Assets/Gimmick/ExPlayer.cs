using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExPlayer : MonoBehaviour
{
    /// <summary>
    /// 가상 플레이어 스크립트
    /// </summary>
     

    //여기에 이벤트 만들고 GimmickTest쪽에서 이벤트 구독할거임
    //코루틴으로 3초마다 이벤트 발동예정
    //이렇게 되면 GimmickTest의 percent 변수 조절 가능

    //가상 수치들(마우스 움직임, 눈깜빡임 등등...)
    int num1 = 0;
    int num2 = 0;

    //이벤트 변수 선언
    public event Action<int, int> percentEventHandler;

    private void Awake()
    {
        //게임 시작시 코루틴 1회 실행 이후 반복
        StartCoroutine(EventCouroutine());
    }

    //이벤트 실행 메소드
    private void ExcuteEvent()
    {
        //이벤트 구독한 모든 메소드에게 수치 넘김
        percentEventHandler?.Invoke(num1, num2);
    }

    //3초마다 1회 실행되는 코루틴
    IEnumerator EventCouroutine()
    {
        //무한반복
        while (true)
        {
            //3초마다 실행
            yield return new WaitForSeconds(3);
            //사용자 성향 분석
            UserTendencyAnalysis();
            //이벤트 실행
            ExcuteEvent();
        }

    }

    ///사용자 성향 분석 메소드
    private void UserTendencyAnalysis()
    {
        //대충 마우스 움직임, 눈깜빡임 변수들 더하고 빼는 메소드
    }
}
