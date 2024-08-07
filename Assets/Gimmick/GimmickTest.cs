using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GimmickInterface;

public class GimmickTest : MonoBehaviour, IGimmick
{
    //�ڽ��� ��� ����Ʈ�� �����ִ��� �ʱⰪ �ٷ� ����
    public ListGroup myGroup { get; set; } = ListGroup.Human;

    //�ϴ� �ν����� â���� ��͸Ŵ��� ������Ʈ �ִ°ɷ� ��������
    //���Ŀ� ��͸Ŵ��� ������ �ֵ��� �������� ���߿� ���Ұ���
    [SerializeField]
    private GimmickManager gimmickManager;

    private void Awake()
    {
        //���� ���� �ʱⰪ �ٷ� �����ϴ� ��� ���� ������ �̰� �׳� ������ ����
        //myGroup = ListGroup.Human;

    }

    //��� ������ �� �� ������ ������
    public void OnEnd()
    {
        //GimmickManager �ʿ� �����ٴ� ��ȣ�ְ� ���� �Ǿ��� ���μҼ� ����Ʈ��
        //�ٽ� GimmickList�� �־�� ��
        gimmickManager.ListInsert(myGroup);
    }

    //����� ó�� ������ ��
    public void OnStart()
    {
        //�̰� �ܺ� �����ؼ� OnStart�� ����ǰ� �ϰų�
        //�ƴϸ� OnEnable�����ŷ� ������ѵ� �� ��

        //GimmickList���� ���� �Ҽ� ����Ʈ ����
        gimmickManager.ListDelete(myGroup);
    }

    //����� �����ϰ� �ִ� ���߿�
    public void OnUpdate()
    {
        //��� �ڵ� �� ����Ǹ� OnEnd ����
        OnEnd();
    }

}
