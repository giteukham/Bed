using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GimmickManager : MonoBehaviour
{
    public static GimmickManager instance { get; private set;}
    public enum GameProgress
    {
        //같은 숫자로 설정시 역참조 불가한거 기억해두기
        //(현재 End로 설정시 First로 설정되는 문제 있음 : 숫자 다 다르게 해야할듯)
        First = 10,
        Middle = 3,
        End = 9
    }

    [SerializeField] private List<Gimmick> allGimicks;
    [SerializeField] private Gimmick unrealGimmick, humanGimmick, objectGimmick;
    public GameProgress progress;

    private void Awake()
    {
        if (instance != null) Debug.LogError("Gimmick Manager already exists");
        instance = this;

        foreach (Gimmick gimmick in allGimicks) 
            if(gimmick.gameObject.activeSelf == false) 
            {
                gimmick.gameObject.SetActive(true);
                gimmick.gameObject.SetActive(false);
            }

        StartCoroutine(GimmickCycleLogic());
    }

    private void Update()
    {
        if (unrealGimmick == null && humanGimmick == null && objectGimmick == null) TimeManager.GimmickRunningCheck(false);
        else TimeManager.GimmickRunningCheck(true);
    }

    // 타입 중복 검사
    private bool CheckDuplication(Gimmick gimmick)
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

    private IEnumerator GimmickCycleLogic()
    {
        while (true)
        {
            //yield return new WaitForSeconds(3);
            //현재 게임 진행상황에 따라서 기믹 뽑는 속도 조절
            yield return new WaitForSeconds((int)progress);
            RedefineProbability();  // 나올 확률 재정의
            ChoiceGimmick();        // 기믹타입 3종류 중에 하나라도 실행이 안되고 있으면 자동으로 기믹 고르게 함
        }
    }

    private void ChoiceGimmick()
    {
        if (unrealGimmick != null && humanGimmick != null && objectGimmick != null) return;

        //1~10번 정도 리스트 무작위 섞기
        int randomInt = Random.Range(1, 10);
        for (int i = 0; i < randomInt; i++)
        {
            ShakeList();
        }
        //무작위 확률값 구하기
        randomInt = Random.Range(1, 101);

        foreach (Gimmick gimmick in allGimicks)
        {
            //만약 확률이 무작위 확률값 보다 높으면서 같은 타입의 기믹이 실행되고 있지 않다면 뽑힌 기믹 실행
            if (gimmick.Probability >= randomInt && CheckDuplication(gimmick) == true)
            {
                AssignGimmickTypeVariable(gimmick);
                gimmick.Activate();
                break;
            }
        }
    }

    // 기믹별 등장확률 재정의(UpdateProbability는 각 기믹 스크립트마다 다름)
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
        int randomInt = Random.Range(0, allGimicks.Count);
        Gimmick temp = allGimicks[randomInt];

        int randomInt2 = Random.Range(0, allGimicks.Count);
        allGimicks[randomInt] = allGimicks[randomInt2];

        allGimicks[randomInt2] = temp;
    }

    //등장확률 낮추는 메소드(리스트 맨 뒤로 옮기고 확률 0으로 낮춤)
    public void LowerProbability(Gimmick gimmick)
    {
        allGimicks.Remove(gimmick);
        allGimicks.Add(gimmick);
        gimmick.Probability = 0;
    }

    // 실행중인 기믹 타입 변수에 해당 기믹 할당
    public void AssignGimmickTypeVariable(Gimmick gimmick)
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
    }

    // 실행중인 기믹 타입 변수에서 해당 기믹 제거
    public void RemoveGimmickTypeVariable(Gimmick gimmick)
    {
        switch (gimmick.Type)
        {
            case GimmickType.Unreal:
                unrealGimmick = null;
                break;
            case GimmickType.Human:
                humanGimmick = null;
                break;
            case GimmickType.Object:
                objectGimmick = null;
                break;
        }
    }

    public void ResetDeactivateGimmick(Gimmick gimmick)
    {
        LowerProbability(gimmick); // 확률 0으로 초기화
        RemoveGimmickTypeVariable(gimmick); // 변수에서 제거
    }
}
