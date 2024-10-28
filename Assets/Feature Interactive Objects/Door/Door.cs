using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Door : MonoBehaviour
{
    static private GameObject door;
    private float doorInitialAngle = 270f;

    static private bool isRotating = false;

    void Awake()
    {
        door = gameObject;
    }
    void Start()
    {
        Vector3 doorInitialRotation = door.transform.eulerAngles;
        door.transform.eulerAngles = new Vector3(doorInitialRotation.x, doorInitialAngle, doorInitialRotation.z);
    }

    /// <summary>
    /// 문 각도 조절
    /// </summary>
    /// <param name="angle">각도</param>
    /// <param name="time">시간</param>
    public static async void Set(float angle, float time)
    {
        if(isRotating) return;

        if(angle > 105) angle = 105;
        if(angle < 0) angle = 0;
        isRotating = true;

        // 270 170
        float elapsedTime = 0f;
        float targetAngle = 270f - angle;
        float pastAngle = door.transform.eulerAngles.y;

        while (elapsedTime < time)
        {
            float currentAngle = Mathf.SmoothStep(pastAngle, targetAngle, elapsedTime / time);
            door.transform.eulerAngles = new Vector3(door.transform.eulerAngles.x, currentAngle, door.transform.eulerAngles.z);
            elapsedTime += Time.deltaTime;
            await UniTask.Yield();
        }

        door.transform.eulerAngles = new Vector3(door.transform.eulerAngles.x, targetAngle, door.transform.eulerAngles.z);
        isRotating = false;
    } 
}
