using System;
using UnityEngine;

public static class PlayerConstant
{
    #region Player Stats 
    //게이지
    public static int stressGauge = 0, fearGauge = 0;
    public static int stressGaugeMax = 100, fearGaugeMax = 100;

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
    }
    
    #region Player Eye Control Constants
    public static float blinkSpeed = 0.1f;    // 깜빡이는 속도
    #endregion

}