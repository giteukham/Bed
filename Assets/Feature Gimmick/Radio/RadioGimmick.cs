using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioGimmick : Gimmick
{
    #region Override Variables
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
        //기본적인 구성후에 오브젝트 바로 끔(바로 끄기 때문에 OnEnable 실행되지 않음)
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
        probability = 100;
    }

    private IEnumerator MainCode()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.radio, transform.position);
        while (timeLimit < 10)
        {
            yield return null;
            //기믹 파훼 성공시
            if (false)
            {
                //Deactivate();
            }
        }

        //10초 지나서 기믹 파훼 실패 했을 때

        //대충 기믹파훼 실패시 플레이어 스트레스 지수 올리는 코드

        //이후 Deactivate 실행
        Deactivate();

    }

    public override void Initialize(){}
}
