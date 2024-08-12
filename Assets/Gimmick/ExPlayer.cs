using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//가나다라마바사아 닭 볕 붓 뷁
//뷁 쀍 닭 닰 닳 닧 겺
public class ExPlayer : MonoBehaviour
{
    /// <summary>
    /// 가상 플레이어 스크립트
    /// </summary>
     

    //여기에 이벤트 만들고 GimmickTest쪽에서 이벤트 구독할거임
    //코루틴으로 3초마다 이벤트 발동예정
    //이렇게 되면 GimmickTest의 percent 변수 조절 가능

    //가상 수치들(마우스 움직임, 눈깜빡임 등등...)
    int mouseMove = 0;
    int eyeBlink = 0;

    //많이 움직였는지, 눈 깜빡거렸는지 여부(안쓸수도 있음)
    //지금 bool변수로 참, 거짓 2가지 값만으로 기믹 등장 확률에 영향주는데
    //상황에 따라서 중간값도 필요해보임
    //enum 변수 하나 만들어서 쓰는것도 나쁘지 않아보임
    bool manyMousemove = false;
    bool manyEyeBlink = false;

    //이벤트 변수 선언
    public event Action<bool, bool> TendencyDataEvent;

    private void Awake()
    {
        //게임 시작시 코루틴 1회 실행 이후 반복
        StartCoroutine(EventCouroutine());
    }

    private void Update()
    {
        //대충 마우스 휠키 클릭, 마우스 움직임 받아서 변수에 넣는 코드
        //휠키 누를때마다 eyeBlink 변수에 넣고(혹은 휠 스크롤 했을때도)

        //마우스 좌표를 이전 위치와 비교하여 얼마나 움직였는지 혹은 x좌표와 y좌표와 이전 좌표와 반대 방향이 되어있는지
        //여부 판단하여 mouseMove에 저장(아니면 다른 방식을 써도 됨)
    }

    //이벤트 실행 메소드
    private void ThrowTendencyData()
    {
        //이벤트 구독한 모든 메소드에게 수치 넘김
        TendencyDataEvent?.Invoke(manyMousemove, manyEyeBlink);
    }

    //3초마다 1회 실행되는 코루틴
    IEnumerator EventCouroutine()
    {
        //무한반복
        while (true)
        {
            //3초마다 실행
            yield return new WaitForSeconds(3);
            //사용자 성향 분석(마우스 관련 움직임 변수 증감)
            UserTendencyAnalysis(mouseMove, eyeBlink);
            //성향 관련 변수 넘김(기믹들은 기믹 본인의 등장확률을 재정의 하게됨)
            ThrowTendencyData();
        }

    }

    ///사용자 성향 분석 메소드
    private void UserTendencyAnalysis(int mouseMove, int eyeBlink)
    {
        //대충 마우스 움직임, 눈깜빡임 변수들 더하고 빼는 메소드
        print("사용자 성향 분석");

        //일단 기준수치 50으로 뒀음, 3초마다 체크하는거라 실제로는 값이 더 적을거임
        //아니면 30초마다 체크하거나
        //사용자 움직임, 클릭 기준 평균치 내서 기준수치 정하는 것도 괜찮아보임
        if (mouseMove > 50)
        {
            manyMousemove = true;
        }
        else
        {
            manyMousemove= false;
        }

        if (eyeBlink > 50)
        {
            manyEyeBlink = true;
        }
        else
        {
            manyEyeBlink= false;
        }

    }
}
