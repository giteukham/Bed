
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    #region Tutorials
    [Header("Tutorials")]
    [SerializeField] private GameObject eyeOpenTutorial;
    [SerializeField] private GameObject leftMoveTutorial;
    [SerializeField] private GameObject openOptionTutorial;
    [SerializeField] private GameObject blinkTutorial;
    #endregion

    #region Tutorial Related Values
    [Header("Tutorial Related Values")]
    [SerializeField, Tooltip("눈 뜨기 튜토리얼 활성화 시간")]
    private float eyeOpenTutorial_ActiveTime;

    [SerializeField, Tooltip("몸 방향 왼쪽으로 바꾸기 튜토리얼 활성화 시간")]
    private float leftMoveTutorial_ActiveTime;

    [SerializeField, Tooltip("눈 깜빡이기 튜토리얼 활성화 시간")]
    private float blink_ActiveTime;

    [SerializeField, Tooltip("튜토리얼 스킵 시간")]
    private float ready_CheckTime;
    #endregion

    [SerializeField] private CockroachForTutorial cockroach;
    public bool isEyeOpenTutorialActivate = false;
    public bool isBlinkTutorialActivate = false;

    private Coroutine eyeOpenTutorialCoroutine, blinkTutorialCoroutine, leftMoveTutorialCoroutine;
    float time = 0f;

    private void OnValidate()
    // 값이 0이면 기본 값으로 설정
    {
        if (eyeOpenTutorial_ActiveTime <= 0) eyeOpenTutorial_ActiveTime = 6f; 
        if (leftMoveTutorial_ActiveTime <= 0) leftMoveTutorial_ActiveTime = 8f;   
        if ( blink_ActiveTime <= 0) blink_ActiveTime = 4f;
        if (ready_CheckTime <= 0) ready_CheckTime = 1f;
    }

    private void Update()
    {
        if (PlayerConstant.isLeftState && PlayerConstant.isEyeOpen && GameManager.Instance.tutorialTestEnable)
        {
            time += Time.deltaTime;
            if (time >= ready_CheckTime)
            {
                if (eyeOpenTutorial.activeSelf) EyeOpenTutorial(false);
                if (eyeOpenTutorialCoroutine != null) StopCoroutine(eyeOpenTutorialCoroutine);
                if (leftMoveTutorial.activeSelf) LeftMoveTutorial(false);
                if (leftMoveTutorialCoroutine != null) StopCoroutine(leftMoveTutorialCoroutine);
                if (blinkTutorial.activeSelf) BlinkTutorial(false);
                if (blinkTutorialCoroutine != null) StopCoroutine(blinkTutorialCoroutine);
                if (cockroach.gameObject.activeSelf) cockroach.Exit();
                GameManager.Instance.tutorialTestEnable = false;
            }
        }
        else time = 0f;
    }

    public void EyeOpenTutorialStart()
    {
        eyeOpenTutorialCoroutine = StartCoroutine(EyeOpenTutorialCoroutine());
    }
    
    private IEnumerator EyeOpenTutorialCoroutine()
    {
        if(!GameManager.Instance.isBlinkInit) yield return new WaitForSeconds(0.25f);
        float startTime = Time.time;
        
        while(isEyeOpenTutorialActivate == false)
        {
            if (Time.time - startTime >= eyeOpenTutorial_ActiveTime && PlayerConstant.isEyeOpen == false && isEyeOpenTutorialActivate == false) EyeOpenTutorial(true);
            if (BlinkEffect.Blink <= 0.93f) 
            {
                isEyeOpenTutorialActivate = true;
                EyeOpenTutorial(false);
                BlinkTutorialStart();
                yield break;
            }
            yield return null;
        }
    }
    
    public void BlinkTutorialStart()
    {
        blinkTutorialCoroutine = StartCoroutine(BlinkTutorialCoroutine());
    }   

    private IEnumerator BlinkTutorialCoroutine()
    {
        float randomNum = UnityEngine.Random.Range(3.5f, 5f);
        yield return new WaitForSeconds(ready_CheckTime + randomNum);
        cockroach.gameObject.SetActive(true);
        yield return new WaitForSeconds(blink_ActiveTime);
        BlinkTutorial(true);

        while(true)
        {
            if(isBlinkTutorialActivate) 
            {
                BlinkTutorial(false);
                LeftMoveTutorialStart();
                yield break;
            }
            yield return null;
        }
    }

    public void LeftMoveTutorialStart()
    {
        leftMoveTutorialCoroutine = StartCoroutine(LeftMoveTutorialCoroutine());
    }
    
    private IEnumerator LeftMoveTutorialCoroutine()
    {
        isEyeOpenTutorialActivate = false;

        float startTime = Time.time;
        while (true)
        {
            if (Time.time - startTime >= leftMoveTutorial_ActiveTime && PlayerConstant.isLeftState == false) LeftMoveTutorial(true);

            if (PlayerConstant.isLeftState)
            {
                LeftMoveTutorial(false);
                GameManager.Instance.tutorialTestEnable = false;
                yield break;
            }

            yield return null;
        }
    }

    public bool CheckCockroachActive()
    {
        if (GameManager.Instance.tutorialTestEnable) return isBlinkTutorialActivate;
        else return !cockroach.gameObject.activeSelf;
    }

    private void ShowTutorial(GameObject Tutorial, bool isActive)
    {
        if (Tutorial.activeSelf == isActive) return;
        Tutorial.GetComponentInChildren<Tutorial>().TutorialActivate(isActive);
    }

    public void EyeOpenTutorial(bool isActive)
    {
        ShowTutorial(eyeOpenTutorial, isActive);
    }

    public void LeftMoveTutorial(bool isActive)
    {
        ShowTutorial(leftMoveTutorial, isActive);
    }

    public void OpenOptionTutorial(bool isActive)
    {
        ShowTutorial(openOptionTutorial, isActive);
    }

    public void BlinkTutorial(bool isActive)
    {
        ShowTutorial(blinkTutorial, isActive);
    }
}
