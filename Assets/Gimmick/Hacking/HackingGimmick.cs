using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class HackingGimmick : Gimmick
{
    [SerializeField]
    private NewGimmickManager gimmickManager;

    [SerializeField]
    private PostProcessVolume processVolume;
    private ColorGrading colorGrading;

    //������ �������ε� �׽�Ʈ�� ���ؼ� �޸� ������� ��� ������
    public override GimmickType Type { get; protected set; } = GimmickType.Human;
    public override float Probability { get; set; } = 100;

    private void Awake()
    {
        try
        {
            processVolume.profile.TryGetSettings<ColorGrading>(out colorGrading);
            gameObject.SetActive(false);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void Update()
    {
        try
        {
            //Ÿ�� �����Ͽ� ���� �ȹ���
            timeLimit += Time.unscaledDeltaTime;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public override void Activate()
    {
        try
        {
            print("��ŷ��� ����");
            SettingVariables();
            StartCoroutine(MainCode());
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public override void Deactivate()
    {
        try
        {
            gimmickManager.LowerProbability(this);
            gimmickManager.humanGimmick = null;
            gameObject.SetActive(false);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public override void UpdateProbability(ExPlayer player)
    {
        try
        {
            Probability = 100;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private IEnumerator MainCode()
    {
        while (timeLimit < 10)
        {
            //WaitForSecondsRealtime�� Time.timeScale�� ���� �ȹ���
            yield return new WaitForSecondsRealtime(0.1f);
            try
            {
                print("�׽�Ʈ �ڵ� ������");
                //��� ���� ���� ��� ���� ���� ����
                if (isDetected == true)
                {
                    Time.timeScale = 0;
                    //�ٸ� �Ҹ� ��� �Ͻ����� Ȥ�� ��� ������ ���� ��������(�̰� ���� ����)
                    //���� ȭ�� ���� ���ɸ����� �Ҹ�(���� ������ ������)
                    StartCoroutine(BlackOut());
                    yield break;
                }
                else
                {
                    Time.timeScale = 1;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        Deactivate();
    }

    private IEnumerator BlackOut()
    {
        colorGrading.active = true;
        yield return new WaitForSecondsRealtime(10);
        //��ҿ��� ���������� ���� �Ҹ�
        //���������� ���� �Ҹ��� ���缭 ȭ�� ��ȯ ������
        yield return new WaitForSecondsRealtime(0.5f);
        colorGrading.active = false;
        //�÷��̾�� ������ �ִ� �ڵ� ����
        Time.timeScale = 1;
        Deactivate();
    }
}
