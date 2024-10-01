using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPH : MonoBehaviour
{
    /// <summary>
    /// Poly 6 Ŀ�� �Լ�
    /// r�� �� ���� ������ �Ÿ�
    /// d�� Smoothing ����
    /// </summary>
    /// <param name="r"></param>
    /// <param name="d"></param>
    /// <returns></returns>
    private float CalculatePoly6Kernel(Vector3 r, float d)
    {
        float rLen = r.magnitude;
        if (rLen < 0 || rLen > d) return 0;
        return (315f / (64f * Mathf.PI * Mathf.Pow(d, 9))) * Mathf.Pow((Mathf.Pow(d, 2) - Mathf.Pow(rLen, 2)), 3);
    }
}
