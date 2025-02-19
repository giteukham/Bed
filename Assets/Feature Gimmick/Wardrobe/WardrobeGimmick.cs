using AbstractGimmick;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    [SerializeField] private float shakeTime = 5f;
    [SerializeField] private float detectedTargetTime = 1f;
    private bool realDetected = false;
    private Vector3 catPosition;
    private Vector3 wardrobePosition;
    private Quaternion wardrobeRotation;
    private Tween shakePositionTween;
    private Tween shakeRotationTween;
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
        gameObject.SetActive(false);
    }

    public override void Initialize()
    {
        cat.SetActive(false);
        eyesAndDark.SetActive(false);
        realDetected = false;
        shakePositionTween = null;
        shakeRotationTween = null;
    }

    public override void UpdateProbability()
    {
        probability = (PlayerConstant.LeftFrontLookCAT) + (PlayerConstant.LeftFrontLookLAT / 4);
        //probability = 100;
    }

    private IEnumerator MainCode()
    {
        //쿵쾅 거리는 소리 들리면서 옷장 흔들림
        AudioManager.Instance.PlaySound(AudioManager.Instance.wardrobeShake, transform.position);
        shakePositionTween = wardrobe.transform.DOShakePosition(shakeTime, 0.02f, 5, 90, false, false, ShakeRandomnessMode.Full).OnComplete(() => wardrobe.transform.localPosition = wardrobePosition);
        shakeRotationTween = wardrobe.transform.DOShakeRotation(shakeTime, 0.05f, 5, 90, false, ShakeRandomnessMode.Full).OnComplete(() => wardrobe.transform.localRotation = wardrobeRotation);

        //옷장 흔들리는 중에 플레이어가 보는지 감지
        StartCoroutine(CheckDetecting());

        //shakeTime동안 대기
        yield return new WaitForSeconds(shakeTime);

        if (realDetected == true)   //감지 성공
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
            AudioManager.Instance.PlaySound(AudioManager.Instance.wardrobeHinges, transform.position);
            rightDoor.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, -25, 0) * rightDoor.transform.localRotation, 2f);

            //70퍼 확률로 고양이가 빼꼼 나와서 그냥 움
            if (randomNum <= 70)
            {
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
                //잠시 대기
                yield return new WaitForSeconds(5);
                GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Stress, +10);
                GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Fear, +10);
            }

            //문 닫히는 코드 추가후 Deactivate();
            AudioManager.Instance.PlaySound(AudioManager.Instance.wardrobeHinges, transform.position);
            rightDoor.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 25, 0) * rightDoor.transform.localRotation, 2f).OnComplete(() => Deactivate());
        }
        else    //감지 실패
        {
            Deactivate();
        }
    }

    private IEnumerator CheckDetecting()
    {
        //목표 시간
        float endTime = timeLimit + shakeTime;
        //감지 시간
        float detectedTime = 0f;

        //1초 이상 바라보면 감지된 것으로 판단 후 반복문 종료
        while (detectedTime < detectedTargetTime)
        {
            if (isDetected == true)
            {
                //오류 Update와 다름
                detectedTime += Time.deltaTime;
            }

            //shakeTime만큼 시간 지나면 종료
            if (endTime < timeLimit)
            {
                //감지 실패
                realDetected = false;
                yield break;
            }

            //프레임마다 대기
            yield return null;
        }
        //감지 성공
        shakePositionTween?.Kill();
        shakeRotationTween?.Kill();
        wardrobe.transform.localPosition = wardrobePosition;
        wardrobe.transform.localRotation = wardrobeRotation;
        AudioManager.Instance.StopSound(AudioManager.Instance.wardrobeShake, FMOD.Studio.STOP_MODE.IMMEDIATE);
        realDetected = true;
        yield break;
    }
}
