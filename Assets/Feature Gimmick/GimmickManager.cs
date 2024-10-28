using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GimmickManager : MonoBehaviour
{
    public enum GameProgress
    {
        //같은 숫자로 설정시 역참조 불가한거 기억해두기
        First = 10,
        Middle = 3,
        End = 10
    }

    [SerializeField]
    private List<Gimmick> allGimicks;

    public Gimmick unrealGimmick;
    public Gimmick humanGimmick;
    public Gimmick objectGimmick;

    private int randomNum1 = 0;
    private int randomNum2 = 0;

    private Gimmick temp;

    public GameProgress progress = GameProgress.First;

    private void Awake()
    {
        StartCoroutine(RandomGimmick());
    }

    private void Update()
    {
        if (unrealGimmick == null && humanGimmick == null && objectGimmick == null) TimeManager.GimmickRunningCheck(false);
        else TimeManager.GimmickRunningCheck(true);
    }

    //현재 실행 시킬 기믹과 같은 타입의 기믹이 실행되고 있는지 확인
    private bool CanActivateGimmick(Gimmick gimmick)
    {
        switch (gimmick.Type)
        {
            case GimmickType.Unreal:
                return unrealGimmick == null;
            case GimmickType.Human:
                return humanGimmick == null;
            case GimmickType.Object:
                return objectGimmick == null;
            default:
                return false;
        }
    }

    //기믹 분류 후 타입에 맞는 변수에 넣음
    private void ActivateGimmick(Gimmick gimmick)
    {
        switch (gimmick.Type)
        {
            case GimmickType.Unreal:
                unrealGimmick = gimmick;
                break;
            case GimmickType.Human:
                humanGimmick = gimmick;
                break;
            case GimmickType.Object:
                objectGimmick = gimmick;
                break;
        }
        //기믹 실행
        gimmick.Activate();
    }

    private IEnumerator RandomGimmick()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            yield return new WaitForSeconds((int)progress);
            RedefineProbability();  // 나올 확률 재정의
            ChoiceGimmick();        // 기믹타입 3종류 중에 하나라도 실행이 안되고 있으면 자동으로 기믹 고르게 함
        }
    }

    private void ChoiceGimmick()
    {

        if (unrealGimmick == null || humanGimmick == null || objectGimmick == null)
        {
            //기믹 3종류 중에 한종류라도 실행 안되는거 있으면 그냥 코드 계속 실행
        }
        else
        {
            //종류 3개 다 실행되고 있는 상태면 메소드 탈출
            return;
        }

        //1~10번 정도 리스트 무작위 섞기
        randomNum1 = Random.Range(1, 10);
        for (int i = 0; i < randomNum1; i++)
        {
            ShakeList();
        }

        //무작위 확률값 구하기
        randomNum1 = Random.Range(1, 101);

        foreach (Gimmick item in allGimicks)
        {
            //만약 확률이 무작위 확률값 보다 높으면서 같은 타입의 기믹이 실행되고 있지 않다면 뽑힌 기믹 실행
            if (item.Probability >= randomNum1 && CanActivateGimmick(item) == true)
            {
                //기믹 실행
                ActivateGimmick(item);
                break;
            }
        }

    }

    //기믹별 등장확률 재정의(UpdateProbability는 각 기믹 스크립트마다 다름)
    private void RedefineProbability()
    {
        foreach (Gimmick item in allGimicks)
        {
            item.UpdateProbability();
        }
    }

    //allGimicks 리스트 무작위로 섞는 메소드
    private void ShakeList()
    {
        randomNum1 = Random.Range(0, allGimicks.Count);
        temp = allGimicks[randomNum1];

        randomNum2 = Random.Range(0, allGimicks.Count);
        allGimicks[randomNum1] = allGimicks[randomNum2];

        allGimicks[randomNum2] = temp;
    }

    //등장확률 낮추는 메소드(리스트 맨 뒤로 옮기고 확률 0으로 낮춤)
    public void LowerProbability(Gimmick gimmick)
    {
        allGimicks.Remove(gimmick);
        allGimicks.Add(gimmick);
        gimmick.Probability = 0;
    }
}
