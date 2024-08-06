using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GimmickInterface;

public class GimmickManager : MonoBehaviour
{
    //�ٸ� ����Ʈ 3���� ������(��ü ���� ����Ʈ)
    List<List<IGimmick>> GimmickList = new List<List<IGimmick>>();

    //������ ��� ����Ʈ
    List<IGimmick> UnrealList = new List<IGimmick>();
    //��� ��� ����Ʈ
    List<IGimmick> HumanList = new List<IGimmick>();
    //�繰 ��� ����Ʈ
    List<IGimmick> ObjectList = new List<IGimmick>();

    private void Awake()
    {
        //�ϴ� ��� ������ ����� ����Ʈ�� ����
        GimmickList.Add(UnrealList);
        GimmickList.Add(HumanList);
        GimmickList.Add(ObjectList);

        //�̷������� ������Ʈ���� ã�Ƽ� ��� �־��ָ� ��(�����ڵ�)
        UnrealList.Add(gameObject.GetComponent<IGimmick>());
    }



}
