using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class HackingGimmick : Gimmick
{
    [SerializeField]
    private NewGimmickManager gimmickManager;

    [SerializeField]
    private PostProcessVolume processVolume;
    private ColorGrading colorGrading;

    //원래는 비현실인데 테스트를 위해서 휴먼 기믹으로 잠깐 변경함
    public override GimmickType Type { get; protected set; } = GimmickType.Human;
    public override float Probability { get; set; } = 100;

    private void Awake()
    {
        try
        {
            processVolume.profile.TryGetSettings<ColorGrading>(out colorGrading);
            gameObject.SetActive(false);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void Update()
    {
        try
        {
            //타임 스케일에 영향 안받음
            timeLimit += Time.unscaledDeltaTime;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public override void Activate()
    {
        try
        {
            print("해킹기믹 실행");
            SettingVariables();
            StartCoroutine(MainCode());
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public override void Deactivate()
    {
        try
        {
            gimmickManager.LowerProbability(this);
            gimmickManager.humanGimmick = null;
            gameObject.SetActive(false);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public override void UpdateProbability(ExPlayer player)
    {
        //눈 자주 감으면 기믹 나오게 하고 싶음, 그리고 게임 중 단 한번만 나왔으면 함
        try
        {
            Probability = 100;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private IEnumerator MainCode()
    {
        try
        {
            Time.timeScale = 0;
            //다른 소리 모두 일시정지 혹은 모두 데미지 없이 강제종료(이건 추후 논의)
            StartCoroutine(BlackOut());
            yield break;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private IEnumerator BlackOut()
    {
        //이상한 전자음 깨지는 소리 반복
        AudioManager.instance.PlayOneShot(AudioManager.instance.lag1, transform.position);
        yield return new WaitForSecondsRealtime(3);
        //화면 검은색으로
        colorGrading.postExposure.value = -100;
        colorGrading.active = true;
        yield return new WaitForSecondsRealtime(7);
        //삐- 거리는 소리
        AudioManager.instance.PlayOneShot(AudioManager.instance.lag2, transform.position);
        //화면 하얀색(일부 오브젝트 색깔 영향 세게 받음)
        colorGrading.postExposure.value = 100;
        yield return new WaitForSecondsRealtime(6);
        colorGrading.active = false;
        //플레이어에게 데미지 주는 코드 삽입
        Time.timeScale = 1;
        Deactivate();
    }
}
