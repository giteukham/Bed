using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GimmickInterface;

public class GimmickManager : MonoBehaviour
{
    //�ٸ� ����Ʈ 3���� ������(��ü ���� ����Ʈ)
    private List<List<IGimmick>> GimmickList = new List<List<IGimmick>>();

    //������ ��� ����Ʈ
    private List<IGimmick> UnrealList = new List<IGimmick>();
    //��� ��� ����Ʈ
    private List<IGimmick> HumanList = new List<IGimmick>();
    //�繰 ��� ����Ʈ
    private List<IGimmick> ObjectList = new List<IGimmick>();

    private void Awake()
    {
        //�ϴ� ��� ������ ����� ����Ʈ�� ����
        GimmickList.Add(UnrealList);
        GimmickList.Add(HumanList);
        GimmickList.Add(ObjectList);

        //�̷������� ������Ʈ���� ã�Ƽ� ��� �־��ָ� ��(�����ڵ�)
        UnrealList.Add(gameObject.GetComponent<IGimmick>());
    }

    //��� ������ ���� GimmickList�� �߰���
    public void ListInsert(ListGroup listGroup)
    {
        switch (listGroup)
        {
            case ListGroup.Unreal:
                GimmickList.Add(UnrealList);
                break;
            case ListGroup.Human:
                GimmickList.Add(HumanList);
                break;
            case ListGroup.Object:
                GimmickList.Add(ObjectList);
                break;
            default:
                break;
        }
    }

    //��� ������ ���� GimickList���� ������
    public void ListDelete(ListGroup listGroup)
    {
        switch (listGroup)
        {
            case ListGroup.Unreal:
                GimmickList.Remove(UnrealList);
                break;
            case ListGroup.Human:
                GimmickList.Remove(HumanList);
                break;
            case ListGroup.Object:
                GimmickList.Remove(ObjectList);
                break;
            default:
                break;
        }
    }


}
