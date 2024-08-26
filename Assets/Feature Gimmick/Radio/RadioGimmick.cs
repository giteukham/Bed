using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioGimmick : Gimmick
{
    //기믹 매니저 참조할 변수
    [SerializeField]
    private GimmickManager gimmickManager;

    public override GimmickType Type { get; protected set; } = GimmickType.Object;
    public override float Probability { get; set; } = 100;

    private void Awake()
    {
        //기본적인 구성후에 오브젝트 바로 끔(바로 끄기 때문에 OnEnable 실행되지 않음)
        gameObject.SetActive(false);
    }

    private void Update()
    {
        timeLimit += Time.deltaTime;
    }

    public override void Activate()
    {
        print("라디오기믹 실행");
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
        //Probability에 대한 계산식
        Probability = 100;
    }

    private IEnumerator MainCode()
    {
        AudioManager.instance.PlayOneShot(AudioManager.instance.radio, transform.position);
        while (timeLimit < 10)
        {
            yield return null;
            //기믹 파훼 성공시
            if (false)
            {
                Deactivate();
            }
        }

        //10초 지나서 기믹 파훼 실패 했을 때

        //대충 기믹파훼 실패시 플레이어 스트레스 지수 올리는 코드

        //이후 Deactivate 실행
        Deactivate();

    }
}
