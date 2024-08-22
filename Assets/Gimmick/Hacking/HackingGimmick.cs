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
        //�� ���� ������ ��� ������ �ϰ� ����, �׸��� ���� �� �� �ѹ��� �������� ��
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
        try
        {
            Time.timeScale = 0;
            //�ٸ� �Ҹ� ��� �Ͻ����� Ȥ�� ��� ������ ���� ��������(�̰� ���� ����)
            StartCoroutine(BlackOut());
            yield break;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private IEnumerator BlackOut()
    {
        //�̻��� ������ ������ �Ҹ� �ݺ�
        AudioManager.instance.PlayOneShot(AudioManager.instance.lag1, transform.position);
        yield return new WaitForSecondsRealtime(3);
        //ȭ�� ����������
        colorGrading.postExposure.value = -100;
        colorGrading.active = true;
        yield return new WaitForSecondsRealtime(7);
        //��- �Ÿ��� �Ҹ�
        AudioManager.instance.PlayOneShot(AudioManager.instance.lag2, transform.position);
        //ȭ�� �Ͼ��(�Ϻ� ������Ʈ ���� ���� ���� ����)
        colorGrading.postExposure.value = 100;
        yield return new WaitForSecondsRealtime(6);
        colorGrading.active = false;
        //�÷��̾�� ������ �ִ� �ڵ� ����
        Time.timeScale = 1;
        Deactivate();
    }
}
