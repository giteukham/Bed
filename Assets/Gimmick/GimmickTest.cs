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

    //Player 프로퍼티의 백킹 변수
    [SerializeField]
    private ExPlayer player;

    //플레이어 마우스 움직임, 눈깜빡임 접근을 위한 변수
    public ExPlayer Player { get; set; }

    //일단 인스펙터 창에서 기믹매니저 오브젝트 넣는걸로 설정했음
    //추후에 기믹매니저 하위로 넣든지 말든지는 나중에 정할거임
    [SerializeField]
    private GimmickManager gimmickManager;

    private void Awake()
    {
        Player = player;

        //이벤트 구독(플레이어보다 빨라서 Awake로 하면 오류남, Start로 해도 오류남)
        Player.percentEvent += PercentRedefine;
        gimmickManager.randomGimmickEvent += InsertIntoListUsingPercent;

        //본인 타입에 맞는 리스트에 기믹 넣음
        gimmickManager.TypeListInsert(myGroup, this);

        
    }

    //기믹 끝났을 때 맨 마지막 마무리
    public void OnEnd()
    {
        print("기믹테스트 온엔드");
        //GimmickManager 쪽에 끝났다는 신호주고 제외 되었던 본인소속 리스트를
        //다시 GimmickList에 넣어야 함
        gimmickManager.TotalListInsert(myGroup);
    }

    //기믹이 처음 시작할 때
    public void OnStart()
    {
        print("기믹테스트 온스타트");
        //이건 외부 접근해서 OnStart가 실행되게 하거나
        //아니면 OnEnable같은거로 실행시켜도 될 듯

        //GimmickList에서 본인 소속 리스트 삭제
        gimmickManager.TotalListDelete(myGroup);
        print("토탈리스트 수 : " + gimmickManager.TotalList.Count);

        //메인 기믹 코드 실행
        OnUpdate();
    }

    //기믹이 시작하고 있는 도중에
    public void OnUpdate()
    {
        //메인코드~
        print("기믹테스트 온업데이트");
        StartCoroutine(TestCode());
        //메인코드~

        //모든 코드 다 실행되면 OnEnd 실행
        //현재 코루틴인 TestCode에서 마지막에 실행함
        //OnEnd();
    }

    //플레이어 스크립트에 있는 percentEvent 이벤트 실행시 자동 실행될 메소드(percent 프로퍼티 값 변경)
    //즉, 이 기믹이 등장할 확률을 정하는 메소드임
    public void PercentRedefine(int mouseMove, int eyeBlink)
    {
        print("퍼센트 리디파인");
        //현재 이 기믹에 있는 등장확률 퍼센트 변수를 매개변수를 이용하여 수치 조정
        //대충 이런식으로
        //조건문의 조건은 기믹 별로 다름
        if (true)
        {
            percent += 3;
        }
        else if(true)
        {
            percent -= 3;
        }

        //percent가 1에서 100의 값을 유지하도록함
        if (percent > 100)
        {
            percent = 100;
        }
        else if (percent < 1)
        {
            percent = 1;
        }
    }

    //기믹매니저에 있는 randomGimmickEvent 이벤트가 실행되면 자동 실행될 메소드
    //randomNum와 비교해서 percent 값이 같거나 크면 리스트에 넣음
    private void InsertIntoListUsingPercent(int randomNum)
    {
        print("인설트 인투 리스트 유징 퍼센트");
        if (randomNum <= percent)
        {
            //이 기믹을 타입에 맞는 리스트에 삽입함
            gimmickManager.TypeListInsert(myGroup, this);
        }
    }

    private IEnumerator TestCode()
    {
        print("메인 코드 시작");
        yield return new WaitForSeconds(9);
        OnEnd();
        print("메인 코드 끝");
    }

}
