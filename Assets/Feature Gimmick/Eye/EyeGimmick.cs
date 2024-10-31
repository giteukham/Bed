using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeGimmick : Gimmick
{
    public override GimmickType Type { get; protected set; } = GimmickType.Unreal;
    public override float Probability { get; set; } = 100;


    [SerializeField]
    private GameObject pupil; // 동공
    Quaternion targetQuaternion; // 시선 회전 목표 각도
    float durationTime = 4f;    // 동공 커지는데 걸리는 시간
    float elapsedTime = 0f;     // 동공 커지는 경과 시간

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        timeLimit += Time.deltaTime;
    }

    public override void Activate()
    {
        // 경과 시간 초기화
        elapsedTime = 0f;

        SettingVariables();
        StartCoroutine(MainCode());
        StartCoroutine(MoveEye());
    }

    public override void Deactivate()
    {
        gimmickManager.LowerProbability(this);
        gimmickManager.unrealGimmick = null;
        gameObject.SetActive(false);
        pupil.transform.localScale = new Vector3(0.58f, 0.58f, pupil.transform.localScale.z);
    }

    public override void UpdateProbability()
    {
        Probability = 100;
    }

    private IEnumerator MainCode()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.eyeStart, transform.position);

        Vector3 initialScale = pupil.transform.localScale; // 동공 초기 크기
        Vector3 targetScale = new Vector3(1.1f, 1.1f, pupil.transform.localScale.z); // 동공 목표 크기

        while (timeLimit < 15)
        {
            yield return null;
            if (isDetected == true)
            {
                targetQuaternion = Quaternion.Euler(new Vector3(21.1f, 55.5f, 0));
                while (Quaternion.Angle(transform.rotation, targetQuaternion) > 0.1f)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetQuaternion, 8f * Time.deltaTime);
                    yield return null;
                }
                
                if (elapsedTime < durationTime)
                {
                    pupil.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / durationTime);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                if (elapsedTime >= durationTime)
                {
                    pupil.transform.localScale = targetScale;
                    AudioManager.instance.PlaySound(AudioManager.instance.eyeEnd, transform.position);
                    yield return new WaitForSeconds(1f);
                    Deactivate();
                    yield break;
                }
            }
        }
        Deactivate();
        yield break;
    }

    IEnumerator MoveEye()
    {
        while (true)
        {   
            if(isDetected) yield break;

            // 돌아갈 각도의 최소 값, 최대 값
            float randomX = Random.Range(-3f, 32f);
            float randomY = Random.Range(62f, 82f);

            // 돌아갈 속도
            float rotationSpeed = Random.Range(7f, 12f);

            // 돌아갈 각도를 Quaternion으로 변환
            Vector3 targetRotation = new Vector3(randomX, randomY, 0);
            targetQuaternion = Quaternion.Euler(targetRotation);
            
            // 돌아갈 각도로 부드럽게 회전 시킴
            while (Quaternion.Angle(transform.rotation, targetQuaternion) > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetQuaternion, rotationSpeed * Time.deltaTime);
                yield return null;
            }
            float delay = Random.Range(0.3f, 0.9f);
            yield return new WaitForSeconds(delay);
        }
    }
}
