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
    // ��� ���� ����
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
        //Probability�� ���� ����
        //probability = 100;
        probability = (PlayerConstant.LeftFrontLookLAT * 2) + (PlayerConstant.LeftFrontLookCAT / 4);
    }

    private IEnumerator MainCode()
    {
        //��� �ڵ�

        //�߱״� ��� �Ҹ� �۰� �鸮�鼭
        //���� ���� ��鸲

        //�� ���¿����� ��� ���� ����� õõ�� ����
        float num = PlayerConstant.LeftFrontLookCAT;
        yield return new WaitForSeconds(5);
        //���� ��ġ ���� ���ϸ� �׳� ��� ����
        if (PlayerConstant.LeftFrontLookCAT - num < 4)
        {
            //������Ʈ ��Ȱ��ȭ�� �ڷ�ƾ �ڵ�����
            Deactivate();
            //yield break;
        }

        if (true)
        {
            //�������� ���� �̰�
            int randomNum = Random.Range(1, 101);
            if (randomNum <= 70)
            {
                //70�� Ȯ�� : ����̰� ���� ���а� �߿��ϰ� ��
            }
            else
            {
                //30�� Ȯ�� : ��վȿ��� ��� ������ ����(�Ͼ�� �ȱ�)
            }
        }
        yield break;
    }

    public override void Initialize(){}
}
