using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardrobeGimmick : Gimmick
{
    #region Override Varizbles
    [field: SerializeField] public override GimmickType type { get; protected set; }
    [SerializeField] private float _probability;
    public override float probability
    {
        get => _probability;
        set => _probability = Mathf.Clamp(value, 0, 100);
    }
    [field: SerializeField] public override List<Gimmick> ExclusionGimmickList { get; set; }
    #endregion

    #region Variables
    // 기믹 개인 변수
    #endregion

    private void Awake()
    {
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
        //Probability에 대한 계산식
        //probability = 100;
        probability = (PlayerConstant.LeftFrontLookLAT * 2) + (PlayerConstant.LeftFrontLookCAT / 4);
    }

    private IEnumerator MainCode()
    {
        //기믹 코드

        //삐그덕 대는 소리 작게 들리면서
        //아주 조금 흔들림

        //그 상태에서도 계속 보면 장롱이 천천히 열림
        float num = PlayerConstant.LeftFrontLookCAT;
        yield return new WaitForSeconds(5);
        //일정 수치 도달 못하면 그냥 기믹 종료
        if (PlayerConstant.LeftFrontLookCAT - num < 4)
        {
            //오브젝트 비활성화로 코루틴 자동종료
            Deactivate();
            //yield break;
        }

        if (true)
        {
            //랜덤으로 숫자 뽑고
            int randomNum = Random.Range(1, 101);
            if (randomNum <= 70)
            {
                //70퍼 확률 : 고양이가 빼꼼 내밀고 야옹하고 움
            }
            else
            {
                //30퍼 확률 : 장롱안에서 사람 눈빛만 보임(하얀색 안광)
            }
        }
        yield break;
    }

    public override void Initialize(){}
}
