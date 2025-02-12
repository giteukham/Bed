
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

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
    #endregion

    [SerializeField] private GameObject cockroach;
    private bool isEyeOpenTutorialEnabled = false;

    private void OnValidate()
    // 값이 0이면 기본 값으로 설정
    {
        if (eyeOpenTutorial_ActiveTime <= 0) eyeOpenTutorial_ActiveTime = 6f; 
        if (leftMoveTutorial_ActiveTime <= 0) leftMoveTutorial_ActiveTime = 8f;   
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) cockroach.GetComponent<CockroachForTutorial>().Exit();
    }

    public void EyeOpenTutorialStart()
    {
        StartCoroutine(EyeOpenTutorialCoroutine());
    }
    
    private IEnumerator EyeOpenTutorialCoroutine()
    {
        if(!GameManager.Instance.isBlinkInit) yield return new WaitForSeconds(0.25f);
        float startTime = Time.time;
        
        while(isEyeOpenTutorialEnabled == false)
        {
            if (Time.time - startTime >= eyeOpenTutorial_ActiveTime && PlayerConstant.isEyeOpen == false && isEyeOpenTutorialEnabled == false) EyeOpenTutorial(true);
            if (BlinkEffect.Blink <= 0.93f) 
            {
                isEyeOpenTutorialEnabled = true;
                EyeOpenTutorial(false);
                //StartCoroutine(LeftMoveTutorialCoroutine());
                yield break;
            }
            yield return null;
        }
    }

    public void LeftMoveTutorialStart()
    {
        StartCoroutine(LeftMoveTutorialCoroutine());
    }
    
    private IEnumerator LeftMoveTutorialCoroutine()
    {
        isEyeOpenTutorialEnabled = true;

        float startTime = Time.time;
        while (true)
        {
            if (Time.time - startTime >= leftMoveTutorial_ActiveTime && PlayerConstant.isLeftState == false)
                LeftMoveTutorial(true);

            if (PlayerConstant.isLeftState == true)
            {
                LeftMoveTutorial(false);
                yield break;
            }

            yield return null;
        }
    }

    private void ShowTutorial(GameObject Tutorial, bool isActive)
    {
        if (Tutorial.activeSelf == isActive) return;

        Tutorial.SetActive(isActive);
        DOTweenAnimation[] dOTweenAnimations = Tutorial.GetComponentsInChildren<DOTweenAnimation>();
        foreach (var child in dOTweenAnimations) child.DORestart();
    }

    public void EyeOpenTutorial(bool isActive)
    {
        ShowTutorial(eyeOpenTutorial, isActive);
    }

    public bool GetEyeOpenManaul()
    {
        return eyeOpenTutorial.activeSelf;
    }

    public void LeftMoveTutorial(bool isActive)
    {
        ShowTutorial(leftMoveTutorial, isActive);
    }

    public bool GetLeftMoveTutorial()
    {
        return leftMoveTutorial.activeSelf;
    }

    public void OpenOptionTutorial(bool isActive)
    {
        ShowTutorial(openOptionTutorial, isActive);
    }

    public bool GetOpenOptionTutorial()
    {
        return openOptionTutorial.activeSelf;
    }

    public void BlinkTutorial(bool isActive)
    {
        ShowTutorial(blinkTutorial, isActive);
    }

    public bool GetBlinkTutorial()
    {
        return blinkTutorial.activeSelf;
    }
}
