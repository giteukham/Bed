using System;
using UnityEngine;

public static class PlayerConstant
{
    #region Player Stats 
    //������
    public static int stressGauge = 0, fearGauge = 0;
    public static int stressGaugeMax = 100, fearGaugeMax = 100;

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
    }
    
    #region Player Eye Control Constants
    public static float blinkSpeed = 0.1f;    // �����̴� �ӵ�
    #endregion

}