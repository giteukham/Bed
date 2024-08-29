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
            print("p 누름");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            CinemachineImpulseManager.Instance.IgnoreTimeScale = true;
            print("q 누름");
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
        //처음에 우우웅 거리는 기계작동음
        //그뒤 천천히 숫자 올라가면서 올라가는 작동음
        //몇초후 갑자기 쾅! 소리 나면서 카메라 흔들림

    }
}
