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
    private float timeInterval = 3f; 
    private float gimmickTimeInterval = 9f; // 기믹땐 3배 느리게

    //현재 기믹 실행되고 있는지 여부 알려주는 변수
    public bool isGimmickRunning = false;
    

    private void Start()
    {
        StartCoroutine(UpdateTime());
    }


    //이건 기믹 안나왔을때 시간 지나는 코루틴
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

    //기믹 나왔을때 시간 지나는 코루틴
    private IEnumerator UpdateTimeGimmick()
    {
        while (true)
        {
            UpdateTimeText();
            yield return new WaitForSeconds(gimmickTimeInterval);
            playTimeToMin++;
            IncrementTime();
        }
    }

    //기믹 나왔을 때, 안나왔을 때 시간 흐름 방식 바꾸는 메소드
    private void TimeFlowChanger()
    {
        switch (isGimmickRunning)
        {
            //기믹 실행되고 있을때는 시간 느리게 흘러감(9 : 60)
            case true:
                StartCoroutine(UpdateTimeGimmick());
                break;
            //기믹 실행 안하고 있을때는 정상적으로 흘러감(3 : 60)
            case false:
                StartCoroutine(UpdateTime());
                break;
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