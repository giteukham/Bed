using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GimmickManager : MonoSingleton<GimmickManager>
{
    [SerializeField] private List<Gimmick> AllGimicks;
    [SerializeField] private List<IMarkovGimmick> AllMarkovGimmicks;
    [SerializeField] private Gimmick unrealGimmick, humanGimmick, objectGimmick;

    public Gimmick CurrentGimmick { get; private set; } = null;

    private void Awake()
    {
        Gimmick[] foundGimmicks = Resources.FindObjectsOfTypeAll<Gimmick>();

        AllGimicks = new List<Gimmick>();
        
        foreach (Gimmick gimmick in foundGimmicks)
            if (gimmick.gameObject.scene.IsValid() && gimmick.gameObject.CompareTag("Gimmick")) AllGimicks.Add(gimmick);
    }

    // 타입 중복 검사
    private bool CheckDuplication(Gimmick gimmick)
    {
        switch (gimmick.type)
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

    // TimeManager에서 호출
    public void PickGimmick()
    {
        if (unrealGimmick != null && humanGimmick != null && objectGimmick != null) return;

        //무작위 확률값 구하기
        int randomInt = Random.Range(30, 101);

        foreach (Gimmick gimmick in AllGimicks)
        {
            //만약 확률이 무작위 확률값 보다 높으면서 같은 타입의 기믹이 실행되고 있지 않다면 뽑힌 기믹 실행
            if (gimmick.probability >= randomInt && CheckDuplication(gimmick) == true)
            {
                switch (gimmick.type)
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

                if(gimmick.ExclusionGimmickList != null) 
                    foreach (Gimmick exclusionGimmick in gimmick.ExclusionGimmickList) 
                        AllGimicks.Remove(exclusionGimmick);

                if(!gimmick.gameObject.activeSelf) gimmick.gameObject.SetActive(true);
                gimmick.Activate();
                break;
            }
        }
    }
    
    public void PickDemoGimmick()
    {
        var gimmick = AllGimicks[0];
        if (typeof(NeighborGimmick) == gimmick.GetType() || typeof(ParentsGimmick) == gimmick.GetType())
        {
            AllGimicks.Remove(gimmick);
            AllGimicks.Add(gimmick);
            PickDemoGimmick();
        }
        
        if(!gimmick.gameObject.activeSelf) gimmick.gameObject.SetActive(true);
        gimmick.Activate();
        AllGimicks.Remove(gimmick);
        AllGimicks.Add(gimmick);
    }

    public Gimmick ForceActivateGimmick(string gimmickName)
    {
        var gimmick = AllGimicks.Find(g => g.name.Equals(gimmickName));
        gimmick?.Activate();

        return gimmick;
    }

    public void ChangeAllMarkovGimmickState(MarkovGimmickData.MarkovGimmickType type)
    {
        foreach (var gimmick in AllGimicks.ToList())
        {
            if (gimmick is IMarkovGimmick markovGimmick)
            {
                markovGimmick.ChangeMarkovState(type);
            }
        }
    }

    // TimeManager에서 호출
    // 기믹별 등장확률 재정의(UpdateProbability는 각 기믹 스크립트마다 다름)
    public void RedefineProbability()
    {
        foreach (Gimmick gimmick in AllGimicks.ToList())
        {
            gimmick?.UpdateProbability();
        }
    }

    public void ResetDeactivateGimmick(Gimmick gimmick)
    {
        if(gimmick.ExclusionGimmickList != null) 
            foreach (Gimmick exclusionGimmick in gimmick.ExclusionGimmickList) 
                AllGimicks.Add(exclusionGimmick);
        AllGimicks.Remove(gimmick);
        
        for ( int i = AllGimicks.Count - 1; i > 0; i--) // 리스트 섞기 Fisher-Yates Shuffle
        {
            int randomInt = Random.Range(0, i + 1);
            Gimmick temp = AllGimicks[i];
            AllGimicks[i] = AllGimicks[randomInt];
            AllGimicks[randomInt] = temp;
        }

        AllGimicks.Add(gimmick); // 위치, 확률 초기화
        gimmick.probability = 0;
        
        switch (gimmick.type)      // 타입변수에서 제거
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

    public void InitGimmicks()
    {
        for (int i = 0; i < AllGimicks.Count; i++)
        {
            ResetDeactivateGimmick(AllGimicks[i]);
        }
    }

    public void DeactivateGimmicks(Gimmick gimmick)
    {
        for(int i = AllGimicks.Count - 1; i >= 0; i--)
        {
            if(!AllGimicks[i].name.Equals(gimmick.name)) 
            {
                AllGimicks[i].Deactivate();
                Debug.Log(AllGimicks[i].name + " Deactivated");
            }
        }
    }
}
