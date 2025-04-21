using System;
using System.Collections;
using UnityEngine;

public static class PlayerConstant
{
    #region Player Stats 
    //������
    public static int stressLevel = 0, noiseLevel = 0, noiseStage = 0;
    public static int stressLevelMax, noiseLevelMax = 100;
    public static int stressLevelMin, noiseLevelMin = 0;
    public static int noiseStageMax = 10, noiseStageMin = -10;
    public static float headMoveSpeed = 0;
    public static bool isEyeOpen = false; 
    public static bool isLeftState, isRightState, isMiddleState, isMovingState;

    // isShock : ������ ���� ������ ��� �ȵǰ� ��� �Ҹ��� �ȵ鸲�� (���� ������ �ʱ�ȭ�� ���)
    // isParalysis : ������ ���� ������ ��� �ȵ� (Ʃ�丮�� ���, ���� ���� ���⶧ ���)
    // isRedemption : �� �����Ӹ� �ȵ� (���� ���� ���⶧ ���)
    public static bool isShock, isParalysis, isRedemption = false;  
    public static bool isPlayerStop = false;
    public static float pixelationFactor = 0.25f;

    // CAT: Cumulative Action Time (���� �ൿ �ð�, ���� �÷��� ���� ���� ���)
    // LAT: Last Action Time (�ֱ� �ൿ �ð�, ���� �ֱ� ����� ���۶� ���� ������ ���� ���)
 
    /// <summary>
    /// ���� �󸶳� ���Ҵ��� (����)
    /// </summary>
    public static float EyeClosedCAT = 0;
    
    /// <summary>
    /// ���� �󸶳� ���Ҵ��� (�ֱ�)
    /// </summary>
    public static float EyeClosedLAT = 0; 

    /// <summary>
    /// ���� �󸶳� �����̴��� (����)
    /// </summary>
    public static int EyeBlinkCAT = 0;

    /// <summary>
    /// ���� �󸶳� �����̴��� (�ֱ�)
    /// </summary>
    public static int EyeBlinkLAT = 0; 

    /// <summary>
    /// �Ӹ��� �󸶳� �����̴��� (����)
    /// </summary>
    public static float HeadMovementCAT = 0;

    /// <summary>
    /// �Ӹ��� �󸶳� �����̴��� (�ֱ�)
    /// </summary>
    public static float HeadMovementLAT = 0; 

    /// <summary>
    /// ���� �󸶳� �����̴��� (����)
    /// </summary>
    public static int BodyMovementCAT = 0;

    /// <summary>
    /// ���� �󸶳� �����̴��� (�ֱ�)
    /// </summary>
    public static int BodyMovementLAT = 0; 

    /// <summary>
    /// �������� �󸶳� �����ִ��� (����)
    /// </summary>
    public static float LeftStateCAT = 0;

    /// <summary>
    /// �������� �󸶳� �����ִ��� (�ֱ�)
    /// </summary>
    public static float LeftStateLAT = 0; 

    /// <summary>
    /// ���������� �󸶳� �����ִ��� (����)
    /// </summary>
    public static float RightStateCAT = 0;
    
    /// <summary>
    /// ���������� �󸶳� �����ִ��� (�ֱ�)
    /// </summary>
    public static float RightStateLAT = 0; 

    /// <summary>
    /// �������� �󸶳� �����ִ��� (����)
    /// </summary>
    public static float MiddleStateCAT = 0;

    /// <summary>
    /// �������� �󸶳� �����ִ��� (�ֱ�)
    /// </summary>
    public static float MiddleStateLAT = 0; 

    /// <summary>
    /// ������ �󸶳� �����ִ��� (����)
    /// </summary>
    public static float LeftLookCAT = 0;

    /// <summary>
    /// ������ �󸶳� �����ִ��� (�ֱ�)
    /// </summary>
    public static float LeftLookLAT = 0;

    /// <summary>
    /// ���� ���� �󸶳� �����ִ��� (����)
    /// </summary>
    public static float LeftFrontLookCAT = 0;

    /// <summary>
    /// ���� ���� �󸶳� �����ִ��� (�ֱ�)
    /// </summary>
    public static float LeftFrontLookLAT = 0;

    /// <summary>
    /// �������� �󸶳� �����ִ��� (����)
    /// </summary>
    public static float RightLookCAT = 0;

    /// <summary>
    /// �������� �󸶳� �����ִ��� (�ֱ�)
    /// </summary>
    public static float RightLookLAT = 0;

    /// <summary>
    /// ������ ���� �󸶳� �����ִ��� (����)
    /// </summary>
    public static float RightFrontLookCAT = 0;

    /// <summary>
    /// ������ ���� �󸶳� �����ִ��� (�ֱ�)
    /// </summary>
    public static float RightFrontLookLAT = 0;

    /// <summary>
    /// ���� �󸶳� �����ִ��� (����)
    /// </summary>
    public static float FrontLookCAT = 0;

    /// <summary>
    /// ���� �󸶳� �����ִ��� (�ֱ�)
    /// </summary>
    public static float FrontLookLAT = 0;

    /// <summary>
    /// ���� �󸶳� �����ִ��� (����)
    /// </summary>
    public static float UpLookCAT = 0;

    /// <summary>
    /// ���� �󸶳� �����ִ��� (�ֱ�)
    /// </summary>
    public static float UpLookLAT = 0;

    /// <summary>
    /// �Ʒ��� �󸶳� �����ִ��� (����)
    /// </summary>
    public static float DownLookCAT = 0;

    /// <summary>
    /// �Ʒ��� �󸶳� �����ִ��� (�ֱ�)
    /// </summary>
    public static float DownLookLAT = 0;
    #endregion

    /// <summary>
    /// ����� ���������� ����Ǵ� �Լ�
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
    public static float blinkSpeed = 0.1f;    // �����̴� �ӵ�
    #endregion

}