using UnityEngine;
using TMPro;
using System.Collections;

public class TimeManager : MonoBehaviour
{
     // 23 시 ~ 07 시

    #region Game Time Variables
    [Header("Game Time Variables")]
    [SerializeField, Tooltip("기본 3초")]private float timeIntervalInitValue = 3f; // 시간 흐름 간격 값
    private float timeInterval; // 시간 간격
    [SerializeField]private float cycleInterval; // 사이클 간격
    private const int TARGET_TIME_TO_MIN = 480; // 게임 시간 480분(현실 시간1440초)이 흐르면 끝
    public static int playTimeToMin = 0;  // 게임 시간 기준 누적 분
    private float realTimeCounter; // 실제로 흐르는 시간
    private float gimmickPickTimeCounter; // 기믹 선택 시간
    private float gimmickRedefineProbabilityTimeCounter; // 기믹 확률 재정의 시간
    private static bool isGimmickRunning = false; //현재 기믹이 실행되고 있는지

    #endregion

    #region AlarmClock Related Variables
    [Header("AlarmClock Related Variables")]
    [SerializeField]private TextMeshPro timeText; 
    [SerializeField]private Renderer clockRenderer; 
    [SerializeField]private Texture amTexture;
    [SerializeField]private Texture pmTexture;
    private int hours; 
    private int hourInit = 11; // 시 초기 값
    private int minutes = 0;
    private bool isAM = false; 
    #endregion

    #region Lighting Related Variables
    [Header("Lighting Related Variables")]
    [SerializeField] private GameObject moonLightObject;
    [SerializeField] private Light sunLight;
    private Light moonLight;
    private Material skyboxMaterial;

    private Color startSkyColor = new Color32(50, 50, 50, 255);
    private Color endSkyColor = new Color32(119, 103, 110, 255);
    private Color startEquatorColor = new Color32(0, 0, 0, 255);
    private Color endEquatorColor = new Color32(11, 8, 12, 255);
    private Color currentSkyColor, currentEquatorColor;
    #endregion
    
    private void Start()
    {
        timeText.text = $"{hours:00}:{minutes:00}";
        moonLight = moonLightObject.GetComponent<Light>();
        skyboxMaterial = RenderSettings.skybox;
        skyboxMaterial.SetFloat("_Exposure", 0.9f);
        RenderSettings.ambientSkyColor = startSkyColor;
        RenderSettings.ambientEquatorColor = startEquatorColor;

        timeInterval = timeIntervalInitValue;
        cycleInterval = timeIntervalInitValue * 3;
    }
    private void Update()
    {
        UpdateClock(); 
        UpdateLighting();
        UpdateGimmickCycle();

        if(isGimmickRunning) timeInterval = timeIntervalInitValue * 3;
        else timeInterval = timeIntervalInitValue;
        realTimeCounter += Time.deltaTime;

        // 3초마다 (기믹이 실행중이면 3*3초마다) 게임 시간 1분 증가
        if (realTimeCounter >= timeInterval)
        {
            playTimeToMin ++;     // 게임 시간 1초 증가
            realTimeCounter = 0;  
            timeInterval = isGimmickRunning ? timeIntervalInitValue * 3 : timeIntervalInitValue; 
        }
    }

    private void UpdateClock() // 시계 갱신
    {
        minutes = playTimeToMin % 60;
        isAM = playTimeToMin >= 60;
        if(isAM)
        {
            hours = hourInit + playTimeToMin / 60 - 12;
            if(clockRenderer.material.GetTexture("_EmissionMap") != amTexture)
                clockRenderer.material.SetTexture("_EmissionMap", amTexture);
        }
        else // isPM
        {
            hours = hourInit + playTimeToMin / 60;
            if(clockRenderer.material.GetTexture("_EmissionMap") != pmTexture)
                clockRenderer.material.SetTexture("_EmissionMap", pmTexture);
        }
        timeText.text = $"{hours:00}:{minutes:00}";
    }

    private void UpdateLighting() // 조명 갱신
    {
        // 30분 전, 2시간 전, 4시간 후에 각각 다른 조명 색상 적용

        // ------ ambientEquatorColor 값만 조절 ------
        if (30 > playTimeToMin) currentEquatorColor = startEquatorColor; // 30분 전까지는

        if (120 >= playTimeToMin && playTimeToMin > 30) // 30분 지나고 2시간 전
        {
            float t = Mathf.InverseLerp(30, 120, playTimeToMin);
            currentEquatorColor = Color.Lerp(startEquatorColor, endEquatorColor, t);
        }
        // ------ ambientEquatorColor 값만 조절 ------


        // ------ ambientSkyColor, moonLight, sunLight, skyboxMaterial 값 조절 ------
        if (playTimeToMin < 240) // 4시간 전까지는
        {
            moonLight.intensity = 1;
            sunLight.intensity = 0;
            skyboxMaterial.SetFloat("_Exposure", 0.9f);
            currentSkyColor = startSkyColor;
        }

        else  // 4시간 지나면
        {
            float t = Mathf.InverseLerp(240, 450, playTimeToMin); 
            currentSkyColor = Color.Lerp(startSkyColor, endSkyColor, t);
            moonLight.intensity = 1 - t; // 달빛이 점점 약해지게
            sunLight.intensity = t;      // 햋빛이 점점 밝아지게       
            skyboxMaterial.SetFloat("_Exposure", 0.9f + t); // 스카이박스가 밝아지게
        }
        // ------ ambientSkyColor, moonLight, sunLight, skyboxMaterial 값 조절 ------

        moonLightObject.transform.rotation = Quaternion.Euler(20f-(0.09f * playTimeToMin), 110f+(0.09f * playTimeToMin), 0); // 조명 각도 갱신
        RenderSettings.ambientSkyColor = currentSkyColor;
        RenderSettings.ambientEquatorColor = currentEquatorColor;
    }

    private void UpdateGimmickCycle() // 기믹 사이클 갱신
    {
        gimmickPickTimeCounter +=  Time.deltaTime;
        gimmickRedefineProbabilityTimeCounter += Time.deltaTime;

        // timeIntervalValue마다 기믹 확률 재정의
        if (gimmickRedefineProbabilityTimeCounter > timeIntervalInitValue) 
        {
            GimmickManager.instance.RedefineProbability();
            gimmickRedefineProbabilityTimeCounter = 0;
        }

        // 사이클 간격마다 기믹 뽑기
        if (gimmickPickTimeCounter >= cycleInterval)  
        {
            GimmickManager.instance.PickGimmick();
            gimmickPickTimeCounter = 0;
        }

        if (30 > playTimeToMin) // 30분 전
        {
            cycleInterval = timeIntervalInitValue * 3;
        }

        else if (120 >= playTimeToMin && playTimeToMin > 30) // 30분 지나고 2시간 전
        {
            cycleInterval = timeIntervalInitValue * 1.5f;
        }

        else if (240 > playTimeToMin && playTimeToMin > 120 ) // 2시간 지나고 4시간 전
        {
            cycleInterval = timeIntervalInitValue;
        } 

        else if (playTimeToMin >= 240)// 4시간 지나면
        {
            cycleInterval = timeIntervalInitValue * 1.5f;
        }
    }

    /// <summary>
    /// 기믹 실행 여부 확인
    /// </summary>
    /// <param name="_isRunning"></param>
    public static void GimmickRunningCheck(bool _isRunning)
    {
        isGimmickRunning = _isRunning;
    }

    /// <summary>
    /// 게임 시간 초기화
    /// </summary>
    public static void ResetPlayTime()
    {
        playTimeToMin = 0;
    }   

    /// <summary>
    /// 게임 시간 파라미터 값 만큼 빼기
    /// </summary>
    /// <param name="_time"></param>
    public static void BackPlayTime(int _time)
    {
        if(playTimeToMin < _time) playTimeToMin = 0;
        else playTimeToMin -= _time;
    }
}