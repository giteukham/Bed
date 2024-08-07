using UnityEngine;
using TMPro;
using System.Collections;

public class TimeController : MonoBehaviour
{
    public TextMeshPro timeText; 
    public Renderer clockRenderer; 
    public Texture amEmissionTexture;

    private int targetTimeToMin = 480; // 게임 시간 480분(현실 시간1440초)이 흐르면 끝
    public int playTimeToMin = 0;  // 게임 시간 기준 누적 분

    private int hours = 11; 
    private int minutes = 0;
    private bool isAM = false; 

    // 평소엔 실제 시간 3초마다 게임 시간 1분씩 흐르게
    // 근데 테스트 할땐 3배 빠르게 1초로
    private float timeInterval = 1f; 
    private float gimmickTimeInterval = 9f; // 기믹땐 3배 느리게
    

    private void Start()
    {
        StartCoroutine(UpdateTime());
    }

    private IEnumerator UpdateTime()
    {
        while (true)
        {
            UpdateTimeText();
            yield return new WaitForSeconds(timeInterval);
            playTimeToMin ++;
            IncrementTime();
        }
    }

    private void UpdateTimeText()
    {
        timeText.text = $"{hours:00}:{minutes:00}";
    }

    private void IncrementTime()
    {
        minutes++;
        if (minutes >= 60)
        {
            minutes = 0;
            hours++;
            if (hours == 12) 
            {
                hours = 0;
                isAM = !isAM;
                clockRenderer.material.SetTexture("_EmissionMap", amEmissionTexture); // am으로 Emission texture 변경
            } 
        }
    }
}