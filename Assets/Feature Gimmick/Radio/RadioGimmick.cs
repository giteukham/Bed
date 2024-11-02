using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioGimmick : Gimmick
{
    [field: SerializeField] public override GimmickType Type { get; protected set; }
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
        base.Activate();
        StartCoroutine(MainCode());
    }

    public override void Deactivate()
    {
        base.Deactivate();
        gameObject.SetActive(false);
    }

    public override void UpdateProbability()
    {
        //Probability�� ���� ����
        Probability = 100;
    }

    private IEnumerator MainCode()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.radio, transform.position);
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

    public override void Initialize(){}
}
