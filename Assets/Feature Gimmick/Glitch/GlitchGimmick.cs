using AbstractGimmick;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GlitchGimmick : Gimmick
{
    #region Override Variables
    [field: SerializeField] public override GimmickType type { get; protected set; }
    [field: SerializeField] public override float probability { get; set; }
    [field: SerializeField] public override List<Gimmick> ExclusionGimmickList { get; set; }
    #endregion

    private void Awake()
    {
        try
        {
            gameObject.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void Update()
    {
        try
        {
            timeLimit += Time.deltaTime;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public override void Activate()
    {
        base.Activate();
        StartCoroutine(MainCode());
    }

    public override void Deactivate()
    {
        try
        {
            base.Deactivate();
            gameObject.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public override void UpdateProbability()
    {
        //�� ���� ������ ��� ������ �ϰ� ����, �׸��� ���� �� �� �ѹ��� �������� ��
        try
        {
            probability = 100;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public override void Initialize(){}

    private IEnumerator MainCode()
    {
        try
        {
            StartCoroutine(GlitchOn());
            yield break;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private IEnumerator GlitchOn()
    {
        //��- �Ÿ��� �Ҹ�
        AudioManager.Instance.PlaySound(AudioManager.Instance.lag2, transform.position);
        yield return new WaitForSeconds(6);

        //�÷��̾�� ������ �ִ� �ڵ� ����
        Deactivate();
    }
}
