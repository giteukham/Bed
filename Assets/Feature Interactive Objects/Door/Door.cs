using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Door : MonoBehaviour
{
    static private GameObject door;
    private float doorInitialAngle = 270f;

    static private bool isRotating = false;
    
    private Coroutine doorKnockCoroutine;

    void Awake()
    {
        door = gameObject;
    }
    void Start()
    {
        Vector3 doorInitialRotation = door.transform.eulerAngles;
        door.transform.eulerAngles = new Vector3(doorInitialRotation.x, doorInitialAngle, doorInitialRotation.z);
    }
    
    public void StartDoorKnock()
    {
        if (doorKnockCoroutine != null) return;
        else doorKnockCoroutine = StartCoroutine(DoorKnock());
    }
    
    public void StopDoorKnock()
    {
        if (doorKnockCoroutine == null) return;
        else StopCoroutine(doorKnockCoroutine);
    }
    
    private IEnumerator DoorKnock()
    {
        while (true)
        {
            float randomNum = Random.Range(2.5f, 5f);
            yield return new WaitForSeconds(randomNum);
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.doorKnock, Door.GetPosition());
        }
    }

    /// <summary>
    /// 문 각도 조절
    /// </summary>
    /// <param name="angle">각도</param>
    /// <param name="time">시간</param>
    
    // 소리 추가
    public static async void Set(float angle, float time)
    {
        if(isRotating) return;

        if(angle > 105) angle = 105;
        if(angle < 0) angle = 0;
        isRotating = true;

        // 270 170
        float elapsedTime = 0f;
        float targetAngle = 270f - angle;
        float prevAngle = door.transform.eulerAngles.y;
        
        Debug.Log(prevAngle);

        if(angle > 0) if ( prevAngle - 270f == 0f ) AudioManager.Instance.PlayOneShot(AudioManager.Instance.doorOpen, GetPosition());

        if ( time < 1.7f) AudioManager.Instance.PlayOneShot(AudioManager.Instance.doorCreak, GetPosition());
        else AudioManager.Instance.PlayOneShot(AudioManager.Instance.doorSlowOpen, GetPosition());

        while (elapsedTime < time)
        {
            float currentAngle = Mathf.SmoothStep(prevAngle, targetAngle, elapsedTime / time);
            door.transform.eulerAngles = new Vector3(door.transform.eulerAngles.x, currentAngle, door.transform.eulerAngles.z);
            elapsedTime += Time.deltaTime;
            await UniTask.Yield();
        }

        if (angle == 0 )
        {
            if (targetAngle - prevAngle <= 20 && 2f < time) AudioManager.Instance.PlayOneShot(AudioManager.Instance.doorSlowClose, GetPosition());
            else AudioManager.Instance.PlayOneShot(AudioManager.Instance.doorClose, GetPosition());
        }

        door.transform.eulerAngles = new Vector3(door.transform.eulerAngles.x, targetAngle, door.transform.eulerAngles.z);
        isRotating = false;
    }

    /// <summary>
    /// 문 각도 조절
    /// </summary>
    /// <param name="angle">각도</param>
    /// <param name="time">시간</param>
    public static async void SetNoSound(float angle, float time)
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

    public static Vector3 GetPosition()
    {
        return door.transform.position;
    }
}
