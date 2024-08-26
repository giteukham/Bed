using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioGimmick : Gimmick
{
    //��� �Ŵ��� ������ ����
    [SerializeField]
    private GimmickManager gimmickManager;

    public override GimmickType Type { get; protected set; } = GimmickType.Object;
    public override float Probability { get; set; } = 100;

    private void Awake()
    {
        //�⺻���� �����Ŀ� ������Ʈ �ٷ� ��(�ٷ� ���� ������ OnEnable ������� ����)
        gameObject.SetActive(false);
    }

    private void Update()
    {
        timeLimit += Time.deltaTime;
    }

    public override void Activate()
    {
        print("������� ����");
        SettingVariables();
        StartCoroutine(MainCode());
    }

    public override void Deactivate()
    {
        gimmickManager.LowerProbability(this);
        gimmickManager.objectGimmick = null;
        gameObject.SetActive(false);
    }

    public override void UpdateProbability()
    {
        //Probability�� ���� ����
        Probability = 100;
    }

    private IEnumerator MainCode()
    {
        AudioManager.instance.PlayOneShot(AudioManager.instance.radio, transform.position);
        while (timeLimit < 10)
        {
            yield return null;
            //��� ���� ������
            if (false)
            {
                Deactivate();
            }
        }

        //10�� ������ ��� ���� ���� ���� ��

        //���� ������� ���н� �÷��̾� ��Ʈ���� ���� �ø��� �ڵ�

        //���� Deactivate ����
        Deactivate();

    }
}
