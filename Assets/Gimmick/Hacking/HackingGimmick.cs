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
        while (timeLimit < 10)
        {
            //WaitForSecondsRealtime은 Time.timeScale에 영향 안받음
            yield return new WaitForSecondsRealtime(0.1f);
            try
            {
                print("테스트 코드 실행중");
                //기믹 실행 조건 어떻게 할지 아직 미정
                if (isDetected == true)
                {
                    Time.timeScale = 0;
                    //다른 소리 모두 일시정지 혹은 모두 데미지 없이 강제종료(이건 추후 논의)
                    //검은 화면 띄우고 렉걸린듯한 소리(라디오 전자음 같은거)
                    StartCoroutine(BlackOut());
                    yield break;
                }
                else
                {
                    Time.timeScale = 1;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        Deactivate();
    }

    private IEnumerator BlackOut()
    {
        colorGrading.active = true;
        yield return new WaitForSecondsRealtime(10);
        //어둠에서 빠져나오는 듯한 소리
        //빠져나오는 듯한 소리에 맞춰서 화면 변환 딜레이
        yield return new WaitForSecondsRealtime(0.5f);
        colorGrading.active = false;
        //플레이어에게 데미지 주는 코드 삽입
        Time.timeScale = 1;
        Deactivate();
    }
}
