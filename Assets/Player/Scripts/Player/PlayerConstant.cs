using System;
using UnityEngine;

public static class PlayerConstant
{
    #region Player Stats
    public static int stressGauge = 0, fearGauge = 0;
    public static int stressGaugeMax = 100, fearGaugeMax = 100;
    #endregion
    
    #region Player Eye Control Constants
    public static float eyeOpenCloseInterval = 0.3f;    // 마우스 휠 내리거나 올릴 때 얼마나 눈이 감기는지
    public static int blinkSpeed = 7;                   // 마우스 휠 클릭하면 눈을 감고 뜨는 속도
    #endregion

}