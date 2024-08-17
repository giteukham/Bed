using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GimmickInterface;
using System;
using Unity.VisualScripting;

public class GimmickManager : MonoBehaviour
{
    public GameObject[] gimmickObjects;
    public List<Gimmick> allGimmicks = new List<Gimmick>();

    //다른 리스트 3개를 관리함(전체 관리 리스트)
    //원래 private임
    public List<List<Gimmick>> TotalList = new List<List<Gimmick>>();

    //비현실 기믹 리스트
    private List<Gimmick> UnrealList = new List<Gimmick>();
    //사람 기믹 리스트
    private List<Gimmick> HumanList = new List<Gimmick>();
    //사물 기믹 리스트
    private List<Gimmick> ObjectList = new List<Gimmick>();

    //랜덤 기믹 뽑을 때 필요한 변수
    private int randomNum = 0;
    private List<Gimmick> TempList = new List<Gimmick>();

    private Gimmick nowGimmick = null;

    private void Awake()
    {
        //일단 모든 종류의 기믹을 리스트에 넣음
        TotalList.Add(UnrealList);
        TotalList.Add(HumanList);
        TotalList.Add(ObjectList);

        //기믹 랜덤 실행 코루틴 1회 실행
        StartCoroutine(RandomGimmick());

        print("기믹 오브젝트 길이 : " + gimmickObjects.Length);

        foreach (GameObject gimmickObject in gimmickObjects)
        {
            allGimmicks.Add(gimmickObject.GetComponent<Gimmick>());
        }

        print("allGimmicks 길이 : " + allGimmicks.Count);
    }

    //기믹 종류에 따라 TotalList(전체 기믹 관리 리스트)에 추가함
    public void TotalListInsert(GimmickType listGroup)
    {
        switch (listGroup)
        {
            case GimmickType.Unreal:
                TotalList.Add(UnrealList);
                break;
            case GimmickType.Human:
                TotalList.Add(HumanList);
                break;
            case GimmickType.Object:
                TotalList.Add(ObjectList);
                break;
            default:
                break;
        }
    }

    //기믹 종류에 따라 TotalList(전체 기믹 관리 리스트)에서 삭제함
    public void TotalListDelete(GimmickType listGroup)
    {
        switch (listGroup)
        {
            case GimmickType.Unreal:
                TotalList.Remove(UnrealList);
                break;
            case GimmickType.Human:
                TotalList.Remove(HumanList);
                break;
            case GimmickType.Object:
                TotalList.Remove(ObjectList);
                break;
            default:
                break;
        }
    }

    //3종류 기믹 리스트에 분류해서 추가함
    public void TypeListInsert(GimmickType listGroup, Gimmick gimmick)
    {
        switch (listGroup)
        {
            case GimmickType.Unreal:
                UnrealList.Add(gimmick);
                break;
            case GimmickType.Human:
                HumanList.Add(gimmick);
                break;
            case GimmickType.Object:
                ObjectList.Add(gimmick);
                break;
            default:
                break;
        }
    }

    //3종류 기믹 리스트에 분류해서 삭제함(별로 쓰진 않을 것 같은데 일단 만들었음)
    public void TypeListDelete(GimmickType listGroup, Gimmick gimmick)
    {
        switch (listGroup)
        {
            case GimmickType.Unreal:
                UnrealList.Remove(gimmick);
                break;
            case GimmickType.Human:
                HumanList.Remove(gimmick);
                break;
            case GimmickType.Object:
                ObjectList.Remove(gimmick);
                break;
            default:
                break;
        }
    }

    IEnumerator RandomGimmick()
    {
        //while 조건으로 나중에 GameManager 같은거 작성해서 그 안에서 게임 끝난는지 여부 알려주는 변수 가져올 것
        //게임 끝나면 더 이상 반복되지 않도록
        while (true)
        {
            yield return new WaitForSeconds(3);
            ChoiceGimmick();
        }
    }

    //타입별 리스트 재정의, 이후 기믹 하나 랜덤으로(확률을 이용해서) 선택
    private void ChoiceGimmick()
    {
        print("리스트 카운트 : " + TotalList.Count);

        //현재 모든 종류의 기믹이 실행되고 있다면 더 이상 기믹 실행 안함(최대 3개)
        //이 방식을 바꿔야함 이유는 3종류 기믹 다 실행되고 있으면 TotalList는 항상 0일거고
        //그럼 평생 아래쪽에 있는 코드들은 실행되지 못함, 그 아래 코드중에 토탈리스트에 타입리스트 추가하는 코드가 있음

        if (TotalList.Count <= 0)
        {
            return;
        }

        //종류별 리스트 초기화
        UnrealList.Clear();
        HumanList.Clear();
        ObjectList.Clear();

        //기믹별 확률에 따라 종류별로 리스트에 삽입(randomNum1 이상의 percent값을 가지고 있어야 삽입 가능)
        randomNum = UnityEngine.Random.Range(1, 101);
        //randomGimmickEvent?.Invoke(randomNum1);
        //위의 이벤트 대신에 아래 for문을 대신 사용
        for (int i = 0; i < allGimmicks.Count; i++)
        {
            //아래 PercentRedefine코드는 Player에게서 매개변수 받아서 넘겨줄 것
            //아니면 불규칙적이라도 그냥 Player 스크립트에서 실행시키던가
            //allGimmicks[i].PercentRedefine(true, true);
            //allGimmicks[i].InsertIntoListUsingPercent(randomNum1);
        }

        //종류별 리스트 3개 중 현재 실행되고 있지 않은 종류 리스트 1개를 뽑음
        randomNum = UnityEngine.Random.Range(0, TotalList.Count);
        TempList = TotalList[randomNum];

        //그 리스트 안에서 또 랜덤으로 기믹 뽑음
        randomNum = UnityEngine.Random.Range(0, TempList.Count);
        nowGimmick = TempList[randomNum];

        //새 기믹 시작
        //nowGimmick.gimmickObject.SetActive(true);

    }


}
