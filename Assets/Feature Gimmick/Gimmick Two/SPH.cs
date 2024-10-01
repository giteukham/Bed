using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPH : MonoBehaviour
{
    /// <summary>
    /// Poly 6 커널 함수
    /// r은 두 입자 사이의 거리
    /// d는 Smoothing 범위
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
