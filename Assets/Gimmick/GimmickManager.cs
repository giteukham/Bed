using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GimmickInterface;

public class GimmickManager : MonoBehaviour
{
    //다른 리스트 3개를 관리함(전체 관리 리스트)
    List<List<IGimmick>> GimmickList = new List<List<IGimmick>>();

    //비현실 기믹 리스트
    List<IGimmick> UnrealList = new List<IGimmick>();
    //사람 기믹 리스트
    List<IGimmick> HumanList = new List<IGimmick>();
    //사물 기믹 리스트
    List<IGimmick> ObjectList = new List<IGimmick>();

    private void Awake()
    {
        //일단 모든 종류의 기믹을 리스트에 넣음
        GimmickList.Add(UnrealList);
        GimmickList.Add(HumanList);
        GimmickList.Add(ObjectList);

        //이런식으로 오브젝트에서 찾아서 기믹 넣어주면 됨(예시코드)
        UnrealList.Add(gameObject.GetComponent<IGimmick>());
    }



}
