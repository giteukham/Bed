using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GimmickInterface;
using System;

public class GimmickTest : MonoBehaviour, IGimmick
{
    //기믹 소속 그룹 표기 변수
    public ListGroup myGroup { get; set; } = ListGroup.Human;
    //등장 확률 변수
    public int percent { get; set; } = 100;
    //플레이어 접근할때 쓸 변수
    public ExPlayer Player { get; set; }
    //자신의 게임 오브젝트를 나타낼 변수
    public GameObject gimmickObject { get; set; }

    //Player 프로퍼티의 백킹 변수
    [SerializeField]
    private ExPlayer player;

    //기믹 매니저 참조할 변수
    [SerializeField]
    private GimmickManager gimmickManager;

    private float timeLimit = 0;

    private void Awake()
    {
        Player = player;
        gimmickObject = gameObject;

        //본인 타입에 맞는 리스트에 기믹 넣음
        gimmickManager.TypeListInsert(myGroup, this);

        //기본적인 구성후에 오브젝트 바로 끔(바로 끄기 때문에 OnEnable 실행되지 않음)
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        //TotalList에서 본인 소속 리스트 삭제
        gimmickManager.TotalListDelete(myGroup);

        //경과시간 초기화
        timeLimit = 0;

        OnStart();
    }

    private void Update()
    {
        timeLimit += Time.deltaTime;
    }

    //기믹 끝났을 때 맨 마지막 마무리
    public void OnEnd()
    {
        //TotalList에서 본인 소속 리스트 추가
        gimmickManager.TotalListInsert(myGroup);

        gameObject.SetActive(false);
    }

    //기믹이 처음 시작할 때
    public void OnStart()
    {
        //메인 기믹 코드 실행
        OnUpdate();
    }

    //기믹이 시작하고 있는 도중에
    public void OnUpdate()
    {
        //메인코드 실행
        StartCoroutine(MainCode());
    }

    //이 기믹이 등장할 확률을 정하는 메소드
    public void PercentRedefine(bool mouseMove, bool eyeBlink)
    {
        //현재 이 기믹에 있는 등장확률 퍼센트 변수를 매개변수를 이용하여 수치 조정
        //조건문의 조건은 기믹 별로 다름
        
        if (mouseMove == true)
        {
            percent += 5;
        }
        else
        {
            percent -= 5;
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
    public void InsertIntoListUsingPercent(int randomNum)
    {
        print("인설트 인투 리스트 유징 퍼센트");
        if (randomNum <= percent)
        {
            //이 기믹을 타입에 맞는 리스트에 삽입함
            gimmickManager.TypeListInsert(myGroup, this);
        }
    }

    private IEnumerator MainCode()
    {
        //기믹 파훼 제한시간 15초
        while (timeLimit < 15)
        {
            yield return new WaitForSeconds(0.1f);
            //기믹 파훼 성공시
            if (false)
            {
                //플레이어에게 데미지 주는거 없이 바로 OnEnd로 넘어감
                OnEnd();
            }
        }

        //15초 지나서 기믹 파훼 실패 했을 때
        //스트레스 지수 혹은 공포 지수를 플레이어에게 접근해서 올림
        //이후 OnEnd 실행
        OnEnd();
    }

}