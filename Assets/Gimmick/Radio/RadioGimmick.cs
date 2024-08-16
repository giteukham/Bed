using GimmickInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioGimmick : MonoBehaviour, IGimmick
{
    public ListGroup myGroup { get; set; } = ListGroup.Object;
    public int percent { get; set; } = 100;
    public ExPlayer Player { get; set; }
    public GameObject gimmickObject { get; set; }

    //Player 프로퍼티의 백킹 변수
    [SerializeField]
    private ExPlayer player;

    //기믹 매니저 참조할 변수
    [SerializeField]
    private GimmickManager gimmickManager;

    //기믹 시작후 경과 시간
    private float timeLimit = 0;

    private IEnumerator co;

    private void Awake()
    {
        Player = player;
        gimmickObject = gameObject;

        //본인 타입에 맞는 리스트에 기믹 넣음(의미 없는 코드임 어치피 기믹매니저에서 Clear 시키고 확률보고 다시 타입 리스트에 널흠)
        gimmickManager.TypeListInsert(myGroup, this);

        co = MainCode();

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

    public void OnEnd()
    {
        //코루틴 종료(그냥 SetActive(false)만 해도 자동종료되기는 함)
        StopCoroutine(co);
        //코루틴 변수로 쓸거면 재할당 해줘야 정상작동 함
        co = MainCode();
        //TotalList에서 본인 소속 리스트 추가
        print("라디오기믹 토탈리스트 인서트 실행");
        gimmickManager.TotalListInsert(myGroup);

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        print("라디오 디스에이블");
    }

    public void OnStart()
    {
        //라디오 소리 1회 실행(소리 길이는 10초 이상이어야 함)
        AudioManager.instance.PlayOneShot(AudioManager.instance.radio, transform.position);
        OnUpdate();
    }

    public void OnUpdate()
    {
        StartCoroutine(co);
    }

    public void PercentRedefine(bool mouseMove, bool eyeBlink)
    {

    }

    public void InsertIntoListUsingPercent(int randomNum)
    {
        print("인설트 인투 리스트 유징 퍼센트 : 라디오");
        if (randomNum <= percent)
        {
            //이 기믹을 타입에 맞는 리스트에 삽입함
            gimmickManager.TypeListInsert(myGroup, this);
        }
    }

    private IEnumerator MainCode()
    {
        print("라디오기믹 메인코드 실행");
        while (timeLimit < 10)
        {
            yield return new WaitForSeconds(0.1f);
            //기믹 파훼 성공시
            if (false)
            {
                //바로 OnEnd로 넘어감
                OnEnd();
                //yield break;
            }
        }

        //10초 지나서 기믹 파훼 실패 했을 때

        //대충 기믹파훼 실패시 플레이어 스트레스 지수 올리는 코드

        //이후 OnEnd 실행
        OnEnd();

    }

}
