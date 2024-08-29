using AbstractGimmick;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GlitchGimmick : Gimmick
{
    public override GimmickType Type { get; protected set; } = GimmickType.Unreal;
    public override float Probability { get; set; } = 100;

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
        try
        {
            SettingVariables();
            StartCoroutine(MainCode());
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public override void Deactivate()
    {
        try
        {
            gimmickManager.LowerProbability(this);
            gimmickManager.unrealGimmick = null;
            gameObject.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public override void UpdateProbability()
    {
        //눈 자주 감으면 기믹 나오게 하고 싶음, 그리고 게임 중 단 한번만 나왔으면 함
        try
        {
            Probability = 100;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

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
        //삐- 거리는 소리
        AudioManager.instance.PlaySound(AudioManager.instance.lag2, transform.position);
        yield return new WaitForSeconds(6);

        //플레이어에게 데미지 주는 코드 삽입
        Deactivate();
    }
}
