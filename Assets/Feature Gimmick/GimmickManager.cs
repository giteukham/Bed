using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GimmickManager : MonoBehaviour
{
    public enum GameProgress
    {
        //���� ���ڷ� ������ ������ �Ұ��Ѱ� ����صα�
        //(���� End�� ������ First�� �����Ǵ� ���� ���� : ���� �� �ٸ��� �ؾ��ҵ�)
        First = 10,
        Middle = 3,
        End = 9
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

    //���� ���� ��ų ��Ͱ� ���� Ÿ���� ����� ����ǰ� �ִ��� Ȯ��
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

    //��� �з� �� Ÿ�Կ� �´� ������ ����
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
        //��� ����
        gimmick.Activate();
    }

    private IEnumerator RandomGimmick()
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

        if (unrealGimmick == null || humanGimmick == null || objectGimmick == null)
        {
            //��� 3���� �߿� �������� ���� �ȵǴ°� ������ �׳� �ڵ� ��� ����
        }
        else
        {
            //���� 3�� �� ����ǰ� �ִ� ���¸� �޼ҵ� Ż��
            return;
        }

        //1~10�� ���� ����Ʈ ������ ����
        randomNum1 = Random.Range(1, 10);
        for (int i = 0; i < randomNum1; i++)
        {
            ShakeList();
        }

        //������ Ȯ���� ���ϱ�
        randomNum1 = Random.Range(1, 101);

        foreach (Gimmick item in allGimicks)
        {
            //���� Ȯ���� ������ Ȯ���� ���� �����鼭 ���� Ÿ���� ����� ����ǰ� ���� �ʴٸ� ���� ��� ����
            if (item.Probability >= randomNum1 && CanActivateGimmick(item) == true)
            {
                //��� ����
                ActivateGimmick(item);
                break;
            }
        }

    }

    //��ͺ� ����Ȯ�� ������(UpdateProbability�� �� ��� ��ũ��Ʈ���� �ٸ�)
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
        randomNum1 = Random.Range(0, allGimicks.Count);
        temp = allGimicks[randomNum1];

        randomNum2 = Random.Range(0, allGimicks.Count);
        allGimicks[randomNum1] = allGimicks[randomNum2];

        allGimicks[randomNum2] = temp;
    }

    //����Ȯ�� ���ߴ� �޼ҵ�(����Ʈ �� �ڷ� �ű�� Ȯ�� 0���� ����)
    public void LowerProbability(Gimmick gimmick)
    {
        allGimicks.Remove(gimmick);
        allGimicks.Add(gimmick);
        gimmick.Probability = 0;
    }
}
