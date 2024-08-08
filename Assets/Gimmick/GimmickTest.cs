using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GimmickInterface;
using System;

public class GimmickTest : MonoBehaviour, IGimmick
{
    //자신이 어느 리스트에 속해있는지 초기값 바로 지정
    public ListGroup myGroup { get; set; } = ListGroup.Human;

    //이 기믹이 추후에 등장할 확률
    //기믹 회피 성공방식에 반하는 행동을 할 수록 수치가 올라감(즉, 플레이어 현재 방식에 어려운 기믹일수록 등장확률 증가)
    //플레이어 쪽에서 코루틴으로 3초마다 이벤트 걸어서 기믹들 퍼센트 수치조정 이벤트 실행시키면 괜찮을거 같음
    public int percent { get; set; } = 100;

    //플레이어 마우스 움직임, 눈깜빡임 접근을 위한 변수
    public ExPlayer player { get; set; }

    //일단 인스펙터 창에서 기믹매니저 오브젝트 넣는걸로 설정했음
    //추후에 기믹매니저 하위로 넣든지 말든지는 나중에 정할거임
    [SerializeField]
    private GimmickManager gimmickManager;

    private void Awake()
    {
        //위에 적은 초기값 바로 지정하는 방식 오류 없으면 이거 그냥 지워도 무관
        //myGroup = ListGroup.Human;

        //이벤트 구독
        player.percentEventHandler += PercentRedefine;

    }

    //기믹 끝났을 때 맨 마지막 마무리
    public void OnEnd()
    {
        //GimmickManager 쪽에 끝났다는 신호주고 제외 되었던 본인소속 리스트를
        //다시 GimmickList에 넣어야 함

        if (true)
        {
            //점수 100점
        }
        else if(false)
        {

        }
        gimmickManager.ListInsert(myGroup);
    }

    //기믹이 처음 시작할 때
    public void OnStart()
    {
        //이건 외부 접근해서 OnStart가 실행되게 하거나
        //아니면 OnEnable같은거로 실행시켜도 될 듯

        //GimmickList에서 본인 소속 리스트 삭제
        gimmickManager.ListDelete(myGroup);
    }

    //기믹이 시작하고 있는 도중에
    public void OnUpdate()
    {
        //모든 코드 다 실행되면 OnEnd 실행
        OnEnd();
    }

    //이벤트 실행시 자동 실행될 메소드
    public void PercentRedefine(int num1, int num2)
    {
        //현재 이 기믹에 있는 등장확률 퍼센트 변수를 매개변수를 이용하여 수치 조정
    }
}
