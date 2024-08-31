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
        //처음에 우우웅 거리는 기계작동음

        //엘리베이터 숫자가 점점 내려감
        while (floor > 2)
        {
            yield return new WaitForSeconds(2);
            floor--;
            tmo.text = floor.ToString();
        }
        //쾅! 소리와 함께 카메라 흔들림 엘리베이터 몇초간 정지
        impulseSource.GenerateImpulseWithForce(3);
        tmo.text = "ERROR";
        yield return new WaitForSeconds(4);


        //그뒤 서서히 숫자 올라가면서 올라가는 작동음
        while (floor <= 6)
        {
            yield return new WaitForSeconds(0.7f);
            floor++;
            tmo.text = floor.ToString();
        }

        //쾅! 소리들리면서 카메라 흔들림
        impulseSource.GenerateImpulseWithForce(4);

        //갑자기 급격히 상승
        while (floor < 66)
        {
            yield return new WaitForSeconds(0.05f);
            floor++;
            tmo.text = floor.ToString();
        }
        //카메라 지속적으로 계속 흔들림
        camShaker.ShakeCam(66, 0.02f);

        while (floor < 666)
        {
            yield return null;
            floor++;
            tmo.text = floor.ToString();
        }
        //숫자 666 되고 잠시 대기
        yield return new WaitForSeconds(0.6f);

        //지금 흔들림 제거하고 더 센 흔들림 생성
        camShaker.StopShakeCam();
        camShaker.ShakeCam(66, 0.1f);

        //최근 플레이어 행동변수 초기화
        PlayerConstant.ResetLATStats();

        //숫자가 마구 뒤섞여 나옴(최대 9자리)
        for (int i = 0; i < 1500; i++)
        {
            yield return null;
            //6번 이상 눈 깜빡이면 데미지 없이 기믹종료
            if (PlayerConstant.EyeBlinkLAT >= 6)
            {
                Deactivate();
            }
            randomNum = Random.Range(100000, 1000000000);
            tmo.text = randomNum.ToString();
        }

        //카메라 흔들림 제거
        tmo.text = "666!66_66";
        camShaker.StopShakeCam();

        //플레이어 데미지 받고 기믹종료
        Deactivate();
    }
}
