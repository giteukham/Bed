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

    //Player ������Ƽ�� ��ŷ ����
    [SerializeField]
    private ExPlayer player;

    //�÷��̾� ���콺 ������, �������� ������ ���� ����
    public ExPlayer Player { get; set; }

    //�ϴ� �ν����� â���� ��͸Ŵ��� ������Ʈ �ִ°ɷ� ��������
    //���Ŀ� ��͸Ŵ��� ������ �ֵ��� �������� ���߿� ���Ұ���
    [SerializeField]
    private GimmickManager gimmickManager;

    private void Awake()
    {
        Player = player;

        //�̺�Ʈ ����(�÷��̾�� ���� Awake�� �ϸ� ������, Start�� �ص� ������)
        Player.percentEvent += PercentRedefine;
        gimmickManager.randomGimmickEvent += InsertIntoListUsingPercent;

        //���� Ÿ�Կ� �´� ����Ʈ�� ��� ����
        gimmickManager.TypeListInsert(myGroup, this);

        
    }

    //��� ������ �� �� ������ ������
    public void OnEnd()
    {
        print("����׽�Ʈ �¿���");
        //GimmickManager �ʿ� �����ٴ� ��ȣ�ְ� ���� �Ǿ��� ���μҼ� ����Ʈ��
        //�ٽ� GimmickList�� �־�� ��
        gimmickManager.TotalListInsert(myGroup);
    }

    //����� ó�� ������ ��
    public void OnStart()
    {
        print("����׽�Ʈ �½�ŸƮ");
        //�̰� �ܺ� �����ؼ� OnStart�� ����ǰ� �ϰų�
        //�ƴϸ� OnEnable�����ŷ� ������ѵ� �� ��

        //GimmickList���� ���� �Ҽ� ����Ʈ ����
        gimmickManager.TotalListDelete(myGroup);
        print("��Ż����Ʈ �� : " + gimmickManager.TotalList.Count);

        //���� ��� �ڵ� ����
        OnUpdate();
    }

    //����� �����ϰ� �ִ� ���߿�
    public void OnUpdate()
    {
        //�����ڵ�~
        print("����׽�Ʈ �¾�����Ʈ");
        StartCoroutine(TestCode());
        //�����ڵ�~

        //��� �ڵ� �� ����Ǹ� OnEnd ����
        //���� �ڷ�ƾ�� TestCode���� �������� ������
        //OnEnd();
    }

    //�÷��̾� ��ũ��Ʈ�� �ִ� percentEvent �̺�Ʈ ����� �ڵ� ����� �޼ҵ�(percent ������Ƽ �� ����)
    //��, �� ����� ������ Ȯ���� ���ϴ� �޼ҵ���
    public void PercentRedefine(int mouseMove, int eyeBlink)
    {
        print("�ۼ�Ʈ ��������");
        //���� �� ��Ϳ� �ִ� ����Ȯ�� �ۼ�Ʈ ������ �Ű������� �̿��Ͽ� ��ġ ����
        //���� �̷�������
        //���ǹ��� ������ ��� ���� �ٸ�
        if (true)
        {
            percent += 3;
        }
        else if(true)
        {
            percent -= 3;
        }

        //percent�� 1���� 100�� ���� �����ϵ�����
        if (percent > 100)
        {
            percent = 100;
        }
        else if (percent < 1)
        {
            percent = 1;
        }
    }

    //��͸Ŵ����� �ִ� randomGimmickEvent �̺�Ʈ�� ����Ǹ� �ڵ� ����� �޼ҵ�
    //randomNum�� ���ؼ� percent ���� ���ų� ũ�� ����Ʈ�� ����
    private void InsertIntoListUsingPercent(int randomNum)
    {
        print("�μ�Ʈ ���� ����Ʈ ��¡ �ۼ�Ʈ");
        if (randomNum <= percent)
        {
            //�� ����� Ÿ�Կ� �´� ����Ʈ�� ������
            gimmickManager.TypeListInsert(myGroup, this);
        }
    }

    private IEnumerator TestCode()
    {
        print("���� �ڵ� ����");
        yield return new WaitForSeconds(9);
        OnEnd();
        print("���� �ڵ� ��");
    }

}
