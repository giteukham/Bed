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



    
}
