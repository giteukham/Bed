using UnityEngine;
using TMPro;
using System.Collections;

public class TimeManager : MonoBehaviour
{
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

    #region Game Time Variables
    [Header("Game Time Variables")]
    private int targetTimeToMin = 480; // 게임 시간 480분(현실 시간1440초)이 흐르면 끝
    public static int playTimeToMin = 0;  // 게임 시간 기준 누적 분
    private float realTimeCounter; // 실제 시간
    private int timeInterval = 3; // 시간 흐름 간격
    private static bool isGimmickRunning = false; //현재 기믹이 실행되고 있는지
    #endregion
    
    private void Start()
    {
        timeText.text = $"{hours:00}:{minutes:00}";
    }
    private void Update()
    {
        UpdateClockTime(); 

        if(isGimmickRunning) timeInterval = 9;
        realTimeCounter += Time.deltaTime;

        if (realTimeCounter >= timeInterval)
        {
            playTimeToMin ++;     // 게임 시간 1초 증가
            realTimeCounter = 0;  
            timeInterval = isGimmickRunning ? 9 : 3; 
        }
    }

    private void UpdateClockTime() // 시계 갱신
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
    /// 게임 시간 매게변수만큼 빼기
    /// </summary>
    /// <param name="_time"></param>
    public static void BackPlayTime(int _time)
    {
        if(playTimeToMin < _time) playTimeToMin = 0;
        else playTimeToMin -= _time;
    }
}