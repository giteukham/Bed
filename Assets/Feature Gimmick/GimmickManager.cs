using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GimmickManager : MonoBehaviour
{
    public static GimmickManager instance { get; private set;}
    [SerializeField] private List<Gimmick> AllGimicks;
    [SerializeField] private Gimmick unrealGimmick, humanGimmick, objectGimmick;

    private void Awake()
    {
        if (instance != null) Debug.LogError("Gimmick Manager already exists");
        instance = this;

        foreach (Gimmick gimmick in AllGimicks) 
            if(gimmick.gameObject.activeSelf == false) 
            {
                gimmick.gameObject.SetActive(true);
                gimmick.gameObject.SetActive(false);
            }

        //StartCoroutine(GimmickCycleLogic());
    }

    private void Update()
    {
        if (unrealGimmick == null && humanGimmick == null && objectGimmick == null) TimeManager.GimmickRunningCheck(false);
        else TimeManager.GimmickRunningCheck(true);
    }

    // Ÿ�� �ߺ� �˻�
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

    // TimeManager���� ȣ��
    public void PickGimmick()
    {
        if (unrealGimmick != null && humanGimmick != null && objectGimmick != null) return;

        //������ Ȯ���� ���ϱ�
        int randomInt = Random.Range(30, 101);

        foreach (Gimmick gimmick in AllGimicks)
        {
            //���� Ȯ���� ������ Ȯ���� ���� �����鼭 ���� Ÿ���� ����� ����ǰ� ���� �ʴٸ� ���� ��� ����
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

                gimmick.Activate();
                break;
            }
        }
    }

    // TimeManager���� ȣ��
    // ��ͺ� ����Ȯ�� ������(UpdateProbability�� �� ��� ��ũ��Ʈ���� �ٸ�)
    public void RedefineProbability()
    {
        Debug.Log("������");
        foreach (Gimmick gimmick in AllGimicks)
        {
            gimmick.UpdateProbability();
        }
    }

    public void ResetDeactivateGimmick(Gimmick gimmick)
    {
        if(gimmick.ExclusionGimmickList != null) 
            foreach (Gimmick exclusionGimmick in gimmick.ExclusionGimmickList) 
                AllGimicks.Add(exclusionGimmick);
        AllGimicks.Remove(gimmick);
        
        for ( int i = AllGimicks.Count - 1; i > 0; i--) // ����Ʈ ���� Fisher-Yates Shuffle
        {
            int randomInt = Random.Range(0, i + 1);
            Gimmick temp = AllGimicks[i];
            AllGimicks[i] = AllGimicks[randomInt];
            AllGimicks[randomInt] = temp;
        }

        AllGimicks.Add(gimmick); // ��ġ, Ȯ�� �ʱ�ȭ
        gimmick.probability = 0;
        
        switch (gimmick.type)      // Ÿ�Ժ������� ����
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
}
