using GimmickInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioGimmick : MonoBehaviour, IGimmick
{
    public ListGroup myGroup { get; set; } = ListGroup.Object;
    public int percent { get; set; } = 100;
    public ExPlayer Player { get; set; }
    public GameObject gimmickObject { get; set; }

    //Player ������Ƽ�� ��ŷ ����
    [SerializeField]
    private ExPlayer player;

    [SerializeField]
    private GimmickManager gimmickManager;

    private float gimmickTime = 0;

    private IEnumerator co;

    private void Awake()
    {
        Player = player;
        gimmickObject = gameObject;

        //�Ʒ� �� �ڵ� ��� ��� �Ŵ������� �ϵ��� �ٲ� ��
        //�׷� �̺�Ʈ 2���� �ʿ������
        Player.TendencyDataEvent += PercentRedefine;
        gimmickManager.randomGimmickEvent += InsertIntoListUsingPercent;

        //���� Ÿ�Կ� �´� ����Ʈ�� ��� ����
        //ó���� ��Ȱ��ȭ �Ǿ��ִµ� �̷��� ���ʿ� ����Ʈ�� �� ������ ��������
        //�ٸ� ����� �ʿ���
        gimmickManager.TypeListInsert(myGroup, this);

        co = MainCode();

        //�⺻���� �����Ŀ� ������Ʈ �ٷ� ��(�ٷ� ���� ������ OnEnable ������� ����)
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        //TotalList���� ���� �Ҽ� ����Ʈ ����
        gimmickManager.TotalListDelete(myGroup);

        //����ð� �ʱ�ȭ
        gimmickTime = 0;

        OnStart();
    }

    private void Update()
    {
        gimmickTime += Time.deltaTime;
    }

    public void OnEnd()
    {
        //�ڷ�ƾ ����(�׳� SetActive(false)�� �ص� �ڵ�����Ǳ�� ��)
        StopCoroutine(co);
        //�ڷ�ƾ ������ ���Ÿ� ���Ҵ� ����� �����۵� ��
        co = MainCode();
        //TotalList���� ���� �Ҽ� ����Ʈ �߰�
        print("������� ��Ż����Ʈ �μ�Ʈ ����");
        gimmickManager.TotalListInsert(myGroup);

        gameObject.SetActive(false);
    }

    public void OnStart()
    {
        //���� �Ҹ� 1ȸ ����(�Ҹ� ���̴� 10�� �̻��̾�� ��)
        AudioManager.instance.PlayOneShot(AudioManager.instance.radio, transform.position);
        OnUpdate();
    }

    public void OnUpdate()
    {
        StartCoroutine(co);
    }

    public void PercentRedefine(bool mouseMove, bool eyeBlink)
    {

    }

    public void InsertIntoListUsingPercent(int randomNum)
    {
        print("�μ�Ʈ ���� ����Ʈ ��¡ �ۼ�Ʈ : ����");
        if (randomNum <= percent)
        {
            //�� ����� Ÿ�Կ� �´� ����Ʈ�� ������
            gimmickManager.TypeListInsert(myGroup, this);
        }
    }

    private IEnumerator MainCode()
    {
        print("������� �����ڵ� ����");
        while (gimmickTime < 10)
        {
            yield return new WaitForSeconds(0.1f);
            //��� ���� ������
            if (false)
            {
                //�ٷ� OnEnd�� �Ѿ
                OnEnd();
                //yield break;
            }
        }

        //10�� ������ ��� ���� ���� ���� ��

        //���� ������� ���н� �÷��̾� ��Ʈ���� ���� �ø��� �ڵ�

        //���� OnEnd ����
        OnEnd();

    }

}
