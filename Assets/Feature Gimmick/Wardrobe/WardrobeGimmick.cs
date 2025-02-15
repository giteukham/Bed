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
        print("�����ڵ� ����");
        //�߱״� �Ÿ��� �Ҹ� �鸮�鼭 ���� ��鸲

        float num = PlayerConstant.LeftFrontLookCAT;

        yield return new WaitForSeconds(7);

        if (PlayerConstant.LeftFrontLookCAT - num < 5)
        {
            print("���� �Ⱥ���");
            //Deactivate();
        }
        else
        {
            //���� ������ ����

            //���� Ȯ���� ���� ����
            int randomNum = Random.Range(1, 101);
            //70�� Ȯ���� ����̰� ���� ���ͼ� �׳� ��
            if (randomNum <= 70)
            {
                print("����� ��� ����");
                //�Ҹ���
                AudioManager.Instance.PlaySound(AudioManager.Instance.WardrobeCat, cat.transform.position);
                //������ ��¦ ����
                cat.transform.DOLocalMove(new Vector3(cat.transform.localPosition.x - 0.15f, cat.transform.localPosition.y, cat.transform.localPosition.z + 0.11f), 2f).OnComplete(
                    () =>
                    {
                        GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Stress, +5);
                    }
                    );
                //��� ���
                yield return new WaitForSeconds(10);
                //�ٽ� ��
                cat.transform.DOLocalMove(new Vector3(cat.transform.localPosition.x + 0.15f, cat.transform.localPosition.y, cat.transform.localPosition.z - 0.11f), 2f).OnComplete(
                    () =>
                    {
                        cat.transform.localPosition = new Vector3(-0.46f, 0.28f, -0.35f);
                        Deactivate();
                    }
                    );

            }
            //30�� Ȯ���� ��վȿ��� �Ͼ�� �ȱ��� ����
            else
            {
                print("�ȱ� ��� ����");
                GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Stress, +10);
                GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Fear, +10);
            }

            //�� ������ �ڵ� �߰��� Deactivate();
        }
    }
}
