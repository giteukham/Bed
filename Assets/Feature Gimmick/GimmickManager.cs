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
        //���� ���ڷ� ������ ������ �Ұ��Ѱ� ����صα�
        //(���� End�� ������ First�� �����Ǵ� ���� ���� : ���� �� �ٸ��� �ؾ��ҵ�)
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

    // Ÿ�� �ߺ� �˻�
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
            //���� ���� �����Ȳ�� ���� ��� �̴� �ӵ� ����
            yield return new WaitForSeconds((int)progress);
            RedefineProbability();  // ���� Ȯ�� ������
            ChoiceGimmick();        // ���Ÿ�� 3���� �߿� �ϳ��� ������ �ȵǰ� ������ �ڵ����� ��� ���� ��
        }
    }

    private void ChoiceGimmick()
    {
        if (unrealGimmick != null && humanGimmick != null && objectGimmick != null) return;

        //1~10�� ���� ����Ʈ ������ ����
        int randomInt = Random.Range(1, 10);
        for (int i = 0; i < randomInt; i++)
        {
            ShakeList();
        }
        //������ Ȯ���� ���ϱ�
        randomInt = Random.Range(1, 101);

        foreach (Gimmick gimmick in allGimicks)
        {
            //���� Ȯ���� ������ Ȯ���� ���� �����鼭 ���� Ÿ���� ����� ����ǰ� ���� �ʴٸ� ���� ��� ����
            if (gimmick.Probability >= randomInt && CheckDuplication(gimmick) == true)
            {
                AssignGimmickTypeVariable(gimmick);
                gimmick.Activate();
                break;
            }
        }
    }

    // ��ͺ� ����Ȯ�� ������(UpdateProbability�� �� ��� ��ũ��Ʈ���� �ٸ�)
    private void RedefineProbability()
    {
        foreach (Gimmick item in allGimicks)
        {
            item.UpdateProbability();
        }
    }

    //allGimicks ����Ʈ �������� ���� �޼ҵ�
    private void ShakeList()
    {
        int randomInt = Random.Range(0, allGimicks.Count);
        Gimmick temp = allGimicks[randomInt];

        int randomInt2 = Random.Range(0, allGimicks.Count);
        allGimicks[randomInt] = allGimicks[randomInt2];

        allGimicks[randomInt2] = temp;
    }

    //����Ȯ�� ���ߴ� �޼ҵ�(����Ʈ �� �ڷ� �ű�� Ȯ�� 0���� ����)
    public void LowerProbability(Gimmick gimmick)
    {
        allGimicks.Remove(gimmick);
        allGimicks.Add(gimmick);
        gimmick.Probability = 0;
    }

    // �������� ��� Ÿ�� ������ �ش� ��� �Ҵ�
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

    // �������� ��� Ÿ�� �������� �ش� ��� ����
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
        LowerProbability(gimmick); // Ȯ�� 0���� �ʱ�ȭ
        RemoveGimmickTypeVariable(gimmick); // �������� ����
    }
}
