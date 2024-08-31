using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// ����� ������Ʈ�� ���� ī�޶� ����ŷ ��ũ��Ʈ
/// </summary>
public class CamShaker : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;

    [SerializeField]
    private CinemachineVirtualCamera virtualCamera; // ����ī�޶�
    private CinemachineTransposer transposer;       // ����ī�޶��� offset�� �����ϱ� ���� ����

    private IEnumerator currentCoroutine;           // �ڷ�ƾ�� �����ϰ� �����ϱ� ���� ����

    private void Awake()
    {
        //���ο��� �ִ� CinemachineImpulseSource ������Ʈ�� ������
        impulseSource = GetComponent<CinemachineImpulseSource>();

        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        //ShakeCam(10, 1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ShakeCam(5, 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            StopShakeCam();
        }
    }

    /// <summary>
    /// ī�޶� ���� �޼ҵ�
    /// </summary>
    /// <param name="shakeTime"></param> ī�޶� ��鸲 ���� �ð�
    /// <param name="shakePower"></param> ��鸲 ����(0.01 ~ 0.5 ���� ��õ��)
    public void ShakeCam(float shakeTime, float shakePower)
    {
        currentCoroutine = ShakeOffset(shakeTime, shakePower);
        StartCoroutine(currentCoroutine);
    }

    /// <summary>
    /// ī�޶� ����ŷ ����
    /// </summary>
    public void StopShakeCam()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
            //ī�޶� ����ġ
            transposer.m_FollowOffset = Vector3.zero;
        }

    }

    private IEnumerator ShakeOffset(float shakeTime, float shakePower)
    {
        float duration = shakeTime;
        //bool test = false;
        //���ӽð���ŭ ����
        while (duration > 0.0f)
        {
            yield return null;


            /*if (duration < 5f && test == false)
            {
                shakePower *= 10;
                test = true;
            }*/

            transposer.m_FollowOffset = Random.insideUnitSphere * shakePower;
            duration -= Time.deltaTime;
        }
        transposer.m_FollowOffset = Vector3.zero;
    }


    /// <summary>
    /// ī�޶� ��鸲 �� 1ȸ ����
    /// </summary>
    /// <param name="power"></param>
    public void MakeImpulse(float power)
    {
        impulseSource.GenerateImpulseWithForce(power);
    }

    public void DeleteImpulse()
    {
        //�����ϴ� ��� Impulse�� ����� �ڵ�
        //������ ���� ������ ������� �ʰ� ����
        //�ó׸ӽ� 3 ���������� ������ ���ٰ� ��
        CinemachineImpulseManager.Instance.Clear();
    }
}
