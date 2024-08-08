using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GimmickInterface;
using System;

public class GimmickTest : MonoBehaviour, IGimmick
{
    //�ڽ��� ��� ����Ʈ�� �����ִ��� �ʱⰪ �ٷ� ����
    public ListGroup myGroup { get; set; } = ListGroup.Human;

    //�� ����� ���Ŀ� ������ Ȯ��
    //��� ȸ�� ������Ŀ� ���ϴ� �ൿ�� �� ���� ��ġ�� �ö�(��, �÷��̾� ���� ��Ŀ� ����� ����ϼ��� ����Ȯ�� ����)
    //�÷��̾� �ʿ��� �ڷ�ƾ���� 3�ʸ��� �̺�Ʈ �ɾ ��͵� �ۼ�Ʈ ��ġ���� �̺�Ʈ �����Ű�� �������� ����
    public int percent { get; set; } = 100;

    //�÷��̾� ���콺 ������, �������� ������ ���� ����
    public ExPlayer player { get; set; }

    //�ϴ� �ν����� â���� ��͸Ŵ��� ������Ʈ �ִ°ɷ� ��������
    //���Ŀ� ��͸Ŵ��� ������ �ֵ��� �������� ���߿� ���Ұ���
    [SerializeField]
    private GimmickManager gimmickManager;

    private void Awake()
    {
        //���� ���� �ʱⰪ �ٷ� �����ϴ� ��� ���� ������ �̰� �׳� ������ ����
        //myGroup = ListGroup.Human;

        //�̺�Ʈ ����
        player.percentEventHandler += PercentRedefine;

    }

    //��� ������ �� �� ������ ������
    public void OnEnd()
    {
        //GimmickManager �ʿ� �����ٴ� ��ȣ�ְ� ���� �Ǿ��� ���μҼ� ����Ʈ��
        //�ٽ� GimmickList�� �־�� ��

        if (true)
        {
            //���� 100��
        }
        else if(false)
        {

        }
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

    //�̺�Ʈ ����� �ڵ� ����� �޼ҵ�
    public void PercentRedefine(int num1, int num2)
    {
        //���� �� ��Ϳ� �ִ� ����Ȯ�� �ۼ�Ʈ ������ �Ű������� �̿��Ͽ� ��ġ ����
    }
}
