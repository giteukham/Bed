using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbstractGimmick;
using Cinemachine;
using TMPro;

public class ElevatorGimmick : Gimmick
{
    public override GimmickType Type { get; protected set; } = GimmickType.Object;
    public override float Probability { get; set; } = 100;

    private CinemachineImpulseSource impulseSource;
    private TextMeshPro tmo;
    private CamShaker camShaker;
    private int floor = 6;
    private int randomNum = 0;

    [SerializeField]
    private GameObject room;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        tmo = GetComponentInChildren<TextMeshPro>();
        camShaker = GetComponent<CamShaker>();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        timeLimit += Time.deltaTime;
    }

    public override void Activate()
    {
        SettingVariables();
        room.SetActive(false);
        StartCoroutine(MainCode());
    }

    public override void Deactivate()
    {
        gimmickManager.LowerProbability(this);
        gimmickManager.objectGimmick = null;
        camShaker.StopShakeCam();
        floor = 6;
        tmo.text = "6";
        room.SetActive(true);
        gameObject.SetActive(false);
    }

    public override void UpdateProbability()
    {
        Probability = 100;
    }

    private IEnumerator MainCode()
    {
        //ó���� ���� �Ÿ��� ����۵���

        //���������� ���ڰ� ���� ������
        while (floor > 2)
        {
            yield return new WaitForSeconds(2);
            floor--;
            tmo.text = floor.ToString();
        }
        //��! �Ҹ��� �Բ� ī�޶� ��鸲 ���������� ���ʰ� ����
        impulseSource.GenerateImpulseWithForce(3);
        tmo.text = "ERROR";
        yield return new WaitForSeconds(4);


        //�׵� ������ ���� �ö󰡸鼭 �ö󰡴� �۵���
        while (floor <= 6)
        {
            yield return new WaitForSeconds(0.7f);
            floor++;
            tmo.text = floor.ToString();
        }

        //��! �Ҹ��鸮�鼭 ī�޶� ��鸲
        impulseSource.GenerateImpulseWithForce(4);

        //���ڱ� �ް��� ���
        while (floor < 66)
        {
            yield return new WaitForSeconds(0.05f);
            floor++;
            tmo.text = floor.ToString();
        }
        //ī�޶� ���������� ��� ��鸲
        camShaker.ShakeCam(66, 0.02f);

        while (floor < 666)
        {
            yield return null;
            floor++;
            tmo.text = floor.ToString();
        }
        //���� 666 �ǰ� ��� ���
        yield return new WaitForSeconds(0.6f);

        //���� ��鸲 �����ϰ� �� �� ��鸲 ����
        camShaker.StopShakeCam();
        camShaker.ShakeCam(66, 0.1f);

        //�ֱ� �÷��̾� �ൿ���� �ʱ�ȭ
        PlayerConstant.ResetLATStats();

        //���ڰ� ���� �ڼ��� ����(�ִ� 9�ڸ�)
        for (int i = 0; i < 1500; i++)
        {
            yield return null;
            //6�� �̻� �� �����̸� ������ ���� �������
            if (PlayerConstant.EyeBlinkLAT >= 6)
            {
                Deactivate();
            }
            randomNum = Random.Range(100000, 1000000000);
            tmo.text = randomNum.ToString();
        }

        //ī�޶� ��鸲 ����
        tmo.text = "666!66_66";
        camShaker.StopShakeCam();

        //�÷��̾� ������ �ް� �������
        Deactivate();
    }
}
