using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SPH
{
    /// <summary>
    /// Poly 6 Ŀ�� �Լ�
    /// r�� �� ���� ������ �Ÿ�
    /// h�� Smoothing ����
    /// </summary>
    /// <param name="r"></param>
    /// <param name="h"></param>
    /// <returns></returns>
    private static float CalculatePoly6Kernel(Vector3 r, float h)
    {
        float rLen = r.magnitude;
        if (rLen < 0 || rLen > h) return 0;
        return (315f / (64f * Mathf.PI * Mathf.Pow(h, 9))) * Mathf.Pow((Mathf.Pow(h, 2) - Mathf.Pow(rLen, 2)), 3);
    }
}
