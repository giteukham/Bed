using System;
using System.Collections;
using UnityEngine;

public static class PlayerConstant
{
    #region Player Stats 
    //게이지
    public static int stressLevel = 0, noiseLevel = 0, noiseStage = 0;
    public static int stressLevelMax, noiseLevelMax = 100;
    public static int stressLevelMin, noiseLevelMin = 0;
    public static int noiseStageMax = 10, noiseStageMin = -10;
    public static float headMoveSpeed = 0;
    public static bool isEyeOpen = false; 
    public static bool isLeftState, isRightState, isMiddleState, isMovingState;

    // isShock : 움직임 관련 조작이 모두 안되고 배게 소리가 안들림림 (게임 오버후 초기화때 사용)
    // isParalysis : 움직임 관련 조작이 모두 안됨 (튜토리얼때 사용, 게임 오버 연출때 사용)
    // isRedemption : 몸 움직임만 안됨 (게임 오버 연출때 사용)
    public static bool isShock, isParalysis, isRedemption = false;  
    public static bool isPlayerStop = false;
    public static float pixelationFactor = 0.25f;

    // CAT: Cumulative Action Time (누적 행동 시간, 게임 플레이 동안 누적 기록)
    // LAT: Last Action Time (최근 행동 시간, 가장 최근 기믹이 시작때 부터 끝날때 까지 기록)
 
    /// <summary>
    /// 눈을 얼마나 감았는지 (누적)
    /// </summary>
    public static float EyeClosedCAT = 0;
    
    /// <summary>
    /// 눈을 얼마나 감았는지 (최근)
    /// </summary>
    public static float EyeClosedLAT = 0; 

    /// <summary>
    /// 눈을 얼마나 깜빡이는지 (누적)
    /// </summary>
    public static int EyeBlinkCAT = 0;

    /// <summary>
    /// 눈을 얼마나 깜빡이는지 (최근)
    /// </summary>
    public static int EyeBlinkLAT = 0; 

    /// <summary>
    /// 머리를 얼마나 움직이는지 (누적)
    /// </summary>
    public static float HeadMovementCAT = 0;

    /// <summary>
    /// 머리를 얼마나 움직이는지 (최근)
    /// </summary>
    public static float HeadMovementLAT = 0; 

    /// <summary>
    /// 몸을 얼마나 움직이는지 (누적)
    /// </summary>
    public static int BodyMovementCAT = 0;

    /// <summary>
    /// 몸을 얼마나 움직이는지 (최근)
    /// </summary>
    public static int BodyMovementLAT = 0; 

    /// <summary>
    /// 왼쪽으로 얼마나 누워있는지 (누적)
    /// </summary>
    public static float LeftStateCAT = 0;

    /// <summary>
    /// 왼쪽으로 얼마나 누워있는지 (최근)
    /// </summary>
    public static float LeftStateLAT = 0; 

    /// <summary>
    /// 오른쪽으로 얼마나 누워있는지 (누적)
    /// </summary>
    public static float RightStateCAT = 0;
    
    /// <summary>
    /// 오른쪽으로 얼마나 누워있는지 (최근)
    /// </summary>
    public static float RightStateLAT = 0; 

    /// <summary>
    /// 정면으로 얼마나 누워있는지 (누적)
    /// </summary>
    public static float MiddleStateCAT = 0;

    /// <summary>
    /// 정면으로 얼마나 누워있는지 (최근)
    /// </summary>
    public static float MiddleStateLAT = 0; 

    /// <summary>
    /// 왼쪽을 얼마나 보고있는지 (누적)
    /// </summary>
    public static float LeftLookCAT = 0;

    /// <summary>
    /// 왼쪽을 얼마나 보고있는지 (최근)
    /// </summary>
    public static float LeftLookLAT = 0;

    /// <summary>
    /// 왼쪽 앞을 얼마나 보고있는지 (누적)
    /// </summary>
    public static float LeftFrontLookCAT = 0;

    /// <summary>
    /// 왼쪽 앞을 얼마나 보고있는지 (최근)
    /// </summary>
    public static float LeftFrontLookLAT = 0;

    /// <summary>
    /// 오른쪽을 얼마나 보고있는지 (누적)
    /// </summary>
    public static float RightLookCAT = 0;

    /// <summary>
    /// 오른쪽을 얼마나 보고있는지 (최근)
    /// </summary>
    public static float RightLookLAT = 0;

    /// <summary>
    /// 오른쪽 앞을 얼마나 보고있는지 (누적)
    /// </summary>
    public static float RightFrontLookCAT = 0;

    /// <summary>
    /// 오른쪽 앞을 얼마나 보고있는지 (최근)
    /// </summary>
    public static float RightFrontLookLAT = 0;

    /// <summary>
    /// 앞을 얼마나 보고있는지 (누적)
    /// </summary>
    public static float FrontLookCAT = 0;

    /// <summary>
    /// 앞을 얼마나 보고있는지 (최근)
    /// </summary>
    public static float FrontLookLAT = 0;

    /// <summary>
    /// 위를 얼마나 보고있는지 (누적)
    /// </summary>
    public static float UpLookCAT = 0;

    /// <summary>
    /// 위를 얼마나 보고있는지 (최근)
    /// </summary>
    public static float UpLookLAT = 0;

    /// <summary>
    /// 아래를 얼마나 보고있는지 (누적)
    /// </summary>
    public static float DownLookCAT = 0;

    /// <summary>
    /// 아래를 얼마나 보고있는지 (최근)
    /// </summary>
    public static float DownLookLAT = 0;
    #endregion

    /// <summary>
    /// 기믹이 끝날때마다 실행되는 함수
    /// </summary>
    public static void ResetLATStats()
    {
        EyeClosedLAT = 0;
        EyeBlinkLAT = 0;
        HeadMovementLAT = 0;
        BodyMovementLAT = 0;
        LeftStateLAT = 0;
        RightStateLAT = 0;
        MiddleStateLAT = 0;
        LeftLookLAT = 0;
        LeftFrontLookLAT = 0;
        RightLookLAT = 0;
        RightFrontLookLAT = 0;
        FrontLookLAT = 0;
        UpLookLAT = 0;
        DownLookLAT = 0;
    }

    public static void ResetCATStats()
    {
        EyeClosedCAT = 0;
        EyeBlinkCAT = 0;
        HeadMovementCAT = 0;
        BodyMovementCAT = 0;
        LeftStateCAT = 0;
        RightStateCAT = 0;
        MiddleStateCAT = 0;
        LeftLookCAT = 0;
        LeftFrontLookCAT = 0;
        RightLookCAT = 0;
        RightFrontLookCAT = 0;
        FrontLookCAT = 0;
        UpLookCAT = 0;
        DownLookCAT = 0;
    }

    public static void ResetAllStats()
    {
        ResetLATStats();
        ResetCATStats();
    }

    #region Player Eye Control Constants
    public static float blinkSpeed = 0.1f;    // 깜빡이는 속도
    #endregion

}