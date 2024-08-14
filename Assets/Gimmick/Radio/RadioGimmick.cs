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

    [SerializeField]
    private GimmickManager gimmickManager;

    private float gimmickTime = 0;

    private IEnumerator co;

    private void Awake()
    {
        Player = player;
        gimmickObject = gameObject;

        Player.TendencyDataEvent += PercentRedefine;
        gimmickManager.randomGimmickEvent += InsertIntoListUsingPercent;

        //본인 타입에 맞는 리스트에 기믹 넣음
        //처음에 비활성화 되어있는데 이러면 애초에 리스트에 들어가 있지도 않을거임
        //다른 방법이 필요함
        gimmickManager.TypeListInsert(myGroup, this);

        co = MainCode();

        //기본적인 구성후에 오브젝트 바로 끔
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        //TotalList에서 본인 소속 리스트 삭제
        gimmickManager.TotalListDelete(myGroup);

        //경과시간 초기화
        gimmickTime = 0;

        OnStart();
    }

    private void Update()
    {
        gimmickTime += Time.deltaTime;
    }

    public void OnEnd()
    {
        //코루틴 종료(그냥 SetActive(false)만 해도 자동종료되기는 함)
        StopCoroutine(co);
        gameObject.SetActive(false);
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

    private void InsertIntoListUsingPercent(int randomNum)
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
        while (gimmickTime < 10)
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
