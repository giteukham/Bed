using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbstractGimmick;
using Cinemachine;

public class ElevatorGimmick : Gimmick
{
    public override GimmickType Type { get; protected set; } = GimmickType.Object;
    public override float Probability { get; set; } = 100;

    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        timeLimit += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            impulseSource.GenerateImpulseWithForce(5f);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            //CinemachineImpulseManager.Instance.Clear();
            impulseSource.m_ImpulseDefinition = null;
            impulseSource.enabled = false;
            //impulseSource.GenerateImpulseWithForce(0);
            print("p ����");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CinemachineImpulseManager.Instance.IgnoreTimeScale = true;
            print("q ����");
        }
    }

    public override void Activate()
    {
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
        Probability = 100;
    }

    private IEnumerator MainCode()
    {
        yield return null;
        //ó���� ���� �Ÿ��� ����۵���
        //�׵� õõ�� ���� �ö󰡸鼭 �ö󰡴� �۵���
        //������ ���ڱ� ��! �Ҹ� ���鼭 ī�޶� ��鸲

    }
}
