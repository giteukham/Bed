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
    [SerializeField] private GameObject wardrobe;
    [SerializeField] private GameObject rightDoor;
    [SerializeField] private GameObject cat;
    [SerializeField] private GameObject eyesAndDark;
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
        cat.SetActive(false);
        eyesAndDark.SetActive(false);
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
        cat.SetActive(false);
        eyesAndDark.SetActive(false);
        gameObject.SetActive(false);
    }

    public override void Initialize()
    {
        
    }

    public override void UpdateProbability()
    {
        probability = (PlayerConstant.LeftFrontLookCAT) + (PlayerConstant.LeftFrontLookLAT / 4);
        //probability = 100;
    }

    private IEnumerator MainCode()
    {
        //삐그덕 거리는 소리 들리면서 옷장 흔들림
        AudioManager.Instance.PlaySound(AudioManager.Instance.wardrobeHinges, transform.position);
        wardrobe.transform.DOShakePosition(1, 0.02f, 5, 90, false, false, ShakeRandomnessMode.Full).OnComplete(() => wardrobe.transform.localPosition = wardrobePosition);
        wardrobe.transform.DOShakeRotation(1, 0.05f, 5, 90, false, ShakeRandomnessMode.Full).OnComplete(() => wardrobe.transform.localRotation = wardrobeRotation);

        float num = PlayerConstant.LeftFrontLookCAT;

        yield return new WaitForSeconds(7);

        if (PlayerConstant.LeftFrontLookCAT - num < 5)
        {
            Deactivate();
        }
        else
        {
            //이후 확률에 따라 진행
            int randomNum = Random.Range(1, 101);
            if (randomNum <= 70)
            {
                cat.SetActive(true);
            }
            else
            {
                eyesAndDark.SetActive(true);
            }

            //문이 서서히 열림
            rightDoor.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, -25, 0) * rightDoor.transform.localRotation, 2f);
            
            //70퍼 확률로 고양이가 빼꼼 나와서 그냥 움
            if (randomNum <= 70)
            {
                cat.SetActive(true);
                //소리냄
                AudioManager.Instance.PlaySound(AudioManager.Instance.wardrobeCat, cat.transform.position);
                //앞으로 살짝 나옴
                cat.transform.DOLocalMove(new Vector3(-0.45f, 0, 0.015f), 2f).OnComplete(
                    () =>
                    {
                        GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Stress, +5);
                    }
                    );
                //잠시 대기
                yield return new WaitForSeconds(10);
                //다시 들어감
                cat.transform.DOLocalMove(catPosition, 2f).OnComplete(() => cat.transform.localPosition = new Vector3(-0.46f, 0.28f, -0.35f));

            }
            //30퍼 확률로 장롱안에서 하얀색 안광만 보임
            else
            {
                eyesAndDark.SetActive(true);
                //잠시 대기
                yield return new WaitForSeconds(5);
                GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Stress, +10);
                GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Fear, +10);
            }

            //문 닫히는 코드 추가후 Deactivate();
            rightDoor.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 25, 0) * rightDoor.transform.localRotation, 2f).OnComplete(() => Deactivate());
        }
    }
}
