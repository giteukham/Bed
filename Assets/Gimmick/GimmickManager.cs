using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GimmickInterface;
using System;

public class GimmickManager : MonoBehaviour
{
    //�ٸ� ����Ʈ 3���� ������(��ü ���� ����Ʈ)
    //���� private��
    public List<List<IGimmick>> TotalList = new List<List<IGimmick>>();

    //������ ��� ����Ʈ
    private List<IGimmick> UnrealList = new List<IGimmick>();
    //��� ��� ����Ʈ
    private List<IGimmick> HumanList = new List<IGimmick>();
    //�繰 ��� ����Ʈ
    private List<IGimmick> ObjectList = new List<IGimmick>();

    //�������� ��� ������ ������ ��� ����Ʈ ��з� �̺�Ʈ
    public event Action<int> randomGimmickEvent;

    //���� ��� ���� �� �ʿ��� ����
    private int randomNum = 0;
    private List<IGimmick> TempList = new List<IGimmick>();

    private IGimmick nowGimmick = null;

    private void Awake()
    {
        //�ϴ� ��� ������ ����� ����Ʈ�� ����
        //TotalList.Add(UnrealList);
        TotalList.Add(HumanList);
        //TotalList.Add(ObjectList);

        //�̷������� ������Ʈ���� ã�Ƽ� ��� �־��ָ� ��(�����ڵ�)
        //UnrealList.Add(gameObject.GetComponent<IGimmick>());

        //��� ���� ���� �ڷ�ƾ 1ȸ ����
        StartCoroutine(RandomGimmick());
    }

    //��� ������ ���� TotalList(��ü ��� ���� ����Ʈ)�� �߰���
    public void TotalListInsert(ListGroup listGroup)
    {
        print("��Ż����Ʈ �μ�Ʈ");
        switch (listGroup)
        {
            case ListGroup.Unreal:
                TotalList.Add(UnrealList);
                break;
            case ListGroup.Human:
                TotalList.Add(HumanList);
                break;
            case ListGroup.Object:
                TotalList.Add(ObjectList);
                break;
            default:
                break;
        }
    }

    //��� ������ ���� TotalList(��ü ��� ���� ����Ʈ)���� ������
    public void TotalListDelete(ListGroup listGroup)
    {
        print("��Ż����Ʈ ����Ʈ");
        switch (listGroup)
        {
            case ListGroup.Unreal:
                TotalList.Remove(UnrealList);
                break;
            case ListGroup.Human:
                TotalList.Remove(HumanList);
                break;
            case ListGroup.Object:
                TotalList.Remove(ObjectList);
                break;
            default:
                break;
        }
    }

    //3���� ��� ����Ʈ�� �з��ؼ� �߰���
    public void TypeListInsert(ListGroup listGroup, IGimmick gimmick)
    {
        print("Ÿ�Ը���Ʈ �μ�Ʈ");
        switch (listGroup)
        {
            case ListGroup.Unreal:
                UnrealList.Add(gimmick);
                break;
            case ListGroup.Human:
                HumanList.Add(gimmick);
                break;
            case ListGroup.Object:
                ObjectList.Add(gimmick);
                break;
            default:
                break;
        }
    }

    //3���� ��� ����Ʈ�� �з��ؼ� ������(���� ���� ���� �� ������ �ϴ� �������)
    public void TypeListDelete(ListGroup listGroup, IGimmick gimmick)
    {
        print("Ÿ�Ը���Ʈ ����Ʈ");
        switch (listGroup)
        {
            case ListGroup.Unreal:
                UnrealList.Remove(gimmick);
                break;
            case ListGroup.Human:
                HumanList.Remove(gimmick);
                break;
            case ListGroup.Object:
                ObjectList.Remove(gimmick);
                break;
            default:
                break;
        }
    }

    IEnumerator RandomGimmick()
    {
        print("������� �ڷ�ƾ 1ȸ ����");
        //while �������� ���߿� GameManager ������ �ۼ��ؼ� �� �ȿ��� ���� �������� ���� �˷��ִ� ���� ������ ��
        //���� ������ �� �̻� �ݺ����� �ʵ���
        while (true)
        {
            yield return new WaitForSeconds(3);
            ChoiceGimmick();
        }
    }

    //Ÿ�Ժ� ����Ʈ ������, ���� ��� �ϳ� ��������(Ȯ���� �̿��ؼ�) ����
    private void ChoiceGimmick()
    {
        print("���̽� ���");
        print("����Ʈ ī��Ʈ : " + TotalList.Count);

        //���� ��� ������ ����� ����ǰ� �ִٸ� �� �̻� ��� ���� ����(�ִ� 3��)
        if (TotalList.Count <= 0)
        {
            return;
        }
        print("���̽� ��� ���");

        //������ ����Ʈ �ʱ�ȭ
        UnrealList.Clear();
        HumanList.Clear();
        ObjectList.Clear();

        //��ͺ� Ȯ���� ���� �������� ����Ʈ�� ����(randomNum �̻��� percent���� ������ �־�� ���� ����)
        randomNum = UnityEngine.Random.Range(1, 101);
        randomGimmickEvent?.Invoke(randomNum);

        //������ ����Ʈ 3�� �� ���� ����ǰ� ���� ���� ���� ����Ʈ 1���� ����
        randomNum = UnityEngine.Random.Range(0, TotalList.Count);
        TempList = TotalList[randomNum];

        //�׽�Ʈ�� ���� �޸ո���Ʈ�� ����************
        //TempList = TotalList[TotalList.IndexOf(HumanList)];

        //�� ����Ʈ �ȿ��� �� �������� ��� ����
        randomNum = UnityEngine.Random.Range(0, TempList.Count);
        nowGimmick = TempList[randomNum];

        //�׽�Ʈ�� ���� ����� �޸ո���Ʈ�� ����(���߿� �ڵ� ������ ��)
        //nowGimmick = HumanList[randomNum];

        //�� ��� ����
        nowGimmick.OnStart();

    }


}
