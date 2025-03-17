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
        //???? ????? ??? ???? ???? ???
        AudioManager.Instance.PlaySound(AudioManager.Instance.wardrobeShake, transform.position);
        shakePositionTween = wardrobe.transform.DOShakePosition(shakeTime, 0.02f, 5, 90, false, false, ShakeRandomnessMode.Full).OnComplete(() => wardrobe.transform.localPosition = wardrobePosition);
        shakeRotationTween = wardrobe.transform.DOShakeRotation(shakeTime, 0.05f, 5, 90, false, ShakeRandomnessMode.Full).OnComplete(() => wardrobe.transform.localRotation = wardrobeRotation);

        //???? ????? ??? ?¡À???? ?????? ????
        StartCoroutine(CheckDetecting());

        //shakeTime???? ???
        yield return new WaitForSeconds(shakeTime);

        if (realDetected == true)   //???? ????
        {
            //???? ????? ???? ????
            int randomNum = Random.Range(1, 101);
            if (randomNum <= 70)
            {
                cat.SetActive(true);
            }
            else
            {
                eyesAndDark.SetActive(true);
            }

            //???? ?????? ????
            AudioManager.Instance.PlaySound(AudioManager.Instance.wardrobeHinges, transform.position);
            rightDoor.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, -25, 0) * rightDoor.transform.localRotation, 2f);

            //70?? ????? ??????? ???? ????? ??? ??
            if (randomNum <= 70)
            {
                //?????
                AudioManager.Instance.PlaySound(AudioManager.Instance.wardrobeCat, cat.transform.position);
                //?????? ??? ????
                cat.transform.DOLocalMove(new Vector3(-0.45f, 0, 0.015f), 2f).OnComplete(
                    () =>
                    {
                        GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Stress, +5);
                    }
                    );
                //??? ???
                yield return new WaitForSeconds(10);
                //??? ???
                cat.transform.DOLocalMove(catPosition, 2f).OnComplete(() => cat.transform.localPosition = new Vector3(-0.46f, 0.28f, -0.35f));

            }
            //30?? ????? ??????? ???? ????? ????
            else
            {
                //??? ???
                yield return new WaitForSeconds(5);
                GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Stress, +10);
                GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Fear, +10);
            }

            //?? ?????? ??? ????? Deactivate();
            AudioManager.Instance.PlaySound(AudioManager.Instance.wardrobeHinges, transform.position);
            rightDoor.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 25, 0) * rightDoor.transform.localRotation, 2f).OnComplete(() => Deactivate());
        }
        else    //???? ????
        {
            Deactivate();
        }
    }

    private IEnumerator CheckDetecting()
    {
        //??? ?©£?
        float endTime = timeLimit + shakeTime;
        //???? ?©£?
        float detectedTime = 0f;

        //1?? ??? ???? ?????? ?????? ??? ?? ????? ????
        while (detectedTime < detectedTargetTime)
        {
            if (isDetected == true)
            {
                //???? Update?? ???
                detectedTime += Time.deltaTime;
            }

            //shakeTime??? ?©£? ?????? ????
            if (endTime < timeLimit)
            {
                //???? ????
                realDetected = false;
                yield break;
            }

            //????????? ???
            yield return null;
        }
        //???? ????
        shakePositionTween?.Kill();
        shakeRotationTween?.Kill();
        wardrobe.transform.localPosition = wardrobePosition;
        wardrobe.transform.localRotation = wardrobeRotation;
        AudioManager.Instance.StopSound(AudioManager.Instance.wardrobeShake, FMOD.Studio.STOP_MODE.IMMEDIATE);
        realDetected = true;
        yield break;
    }
}