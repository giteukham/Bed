using AbstractGimmick;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardrobeGimmick : Gimmick
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
    [SerializeField] private GameObject cat;
    #endregion

    private void Awake()
    {
        
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

    public override void Initialize()
    {
        
    }

    public override void UpdateProbability()
    {
        //probability = (PlayerConstant.LeftFrontLookCAT) + (PlayerConstant.LeftFrontLookLAT / 4);
        probability = 100;
    }

    private IEnumerator MainCode()
    {
        print("메인코드 실행");
        //삐그덕 거리는 소리 들리면서 옷장 흔들림

        float num = PlayerConstant.LeftFrontLookCAT;

        yield return new WaitForSeconds(7);

        if (PlayerConstant.LeftFrontLookCAT - num < 5)
        {
            print("오래 안봤음");
            //Deactivate();
        }
        else
        {
            //문이 서서히 열림

            //이후 확률에 따라 진행
            int randomNum = Random.Range(1, 101);
            //70퍼 확률로 고양이가 빼꼼 나와서 그냥 움
            if (randomNum <= 70)
            {
                print("고양이 기믹 실행");
                //소리냄
                AudioManager.Instance.PlaySound(AudioManager.Instance.WardrobeCat, cat.transform.position);
                //앞으로 살짝 나옴
                cat.transform.DOLocalMove(new Vector3(cat.transform.localPosition.x - 0.15f, cat.transform.localPosition.y, cat.transform.localPosition.z + 0.11f), 2f).OnComplete(
                    () =>
                    {
                        GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Stress, +5);
                    }
                    );
                //잠시 대기
                yield return new WaitForSeconds(10);
                //다시 들어감
                cat.transform.DOLocalMove(new Vector3(cat.transform.localPosition.x + 0.15f, cat.transform.localPosition.y, cat.transform.localPosition.z - 0.11f), 2f).OnComplete(
                    () =>
                    {
                        cat.transform.localPosition = new Vector3(-0.46f, 0.28f, -0.35f);
                        Deactivate();
                    }
                    );

            }
            //30퍼 확률로 장롱안에서 하얀색 안광만 보임
            else
            {
                print("안광 기믹 실행");
                GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Stress, +10);
                GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Fear, +10);
            }

            //문 닫히는 코드 추가후 Deactivate();
        }
    }
}
