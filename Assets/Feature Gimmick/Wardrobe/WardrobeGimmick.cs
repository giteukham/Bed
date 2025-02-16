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
    [SerializeField] private GameObject wardrobe;
    [SerializeField] private GameObject rightDoor;
    private Vector3 catPosition;
    private Vector3 wardrobePosition;
    private Quaternion wardrobeRotation;
    #endregion

    private void Awake()
    {
        catPosition = cat.transform.localPosition;
        wardrobe.transform.SetParent(null);
        wardrobePosition = wardrobe.transform.localPosition;
        wardrobeRotation = wardrobe.transform.localRotation;

    }

    IEnumerator TestCode()
    {
        yield return new WaitForSeconds(5);
        wardrobe.transform.DOShakePosition(1, 0.02f, 2, 90, false, false).OnComplete(() => wardrobe.transform.localPosition = wardrobePosition);
    }

    private void Update()
    {
        timeLimit += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.T))
        {
            wardrobe.transform.DOShakePosition(1, 0.02f, 5, 90, false, false, ShakeRandomnessMode.Full).OnComplete(() => wardrobe.transform.localPosition = wardrobePosition);
            wardrobe.transform.DOShakeRotation(1, 0.05f, 5, 90, false, ShakeRandomnessMode.Full).OnComplete(() => wardrobe.transform.localRotation = wardrobeRotation);
        }
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
        AudioManager.Instance.PlaySound(AudioManager.Instance.wardrobeHinges, transform.position);
        wardrobe.transform.DOShakePosition(1, 1, 2, 90, false, false, ShakeRandomnessMode.Full).OnComplete(() => wardrobe.transform.localPosition = wardrobePosition);
        wardrobe.transform.DOShakeRotation(1, 0.05f, 5, 90, false, ShakeRandomnessMode.Full).OnComplete(() => wardrobe.transform.localRotation = wardrobeRotation);

        float num = PlayerConstant.LeftFrontLookCAT;

        yield return new WaitForSeconds(7);

        if (PlayerConstant.LeftFrontLookCAT - num < 5)
        {
            print("���� �Ⱥ���");
            Deactivate();
        }
        else
        {
            //���� ������ ����
            rightDoor.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, -15, 0) * rightDoor.transform.localRotation, 2f);

            //���� Ȯ���� ���� ����
            int randomNum = Random.Range(1, 101);
            //70�� Ȯ���� ����̰� ���� ���ͼ� �׳� ��
            if (randomNum <= 70)
            {
                print("����� ��� ����");
                //�Ҹ���
                AudioManager.Instance.PlaySound(AudioManager.Instance.wardrobeCat, cat.transform.position);
                //������ ��¦ ����
                cat.transform.DOLocalMove(new Vector3(-0.45f, 0, 0.015f), 2f).OnComplete(
                    () =>
                    {
                        GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Stress, +5);
                    }
                    );
                //��� ���
                yield return new WaitForSeconds(10);
                //�ٽ� ��
                cat.transform.DOLocalMove(catPosition, 2f).OnComplete(
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
                //��� ���
                yield return new WaitForSeconds(5);
                print("�ȱ� ��� ����");
                GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Stress, +10);
                GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Fear, +10);
            }

            //�� ������ �ڵ� �߰��� Deactivate();
            rightDoor.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 15, 0) * rightDoor.transform.localRotation, 2f).OnComplete(() => Deactivate());
        }
    }
}
