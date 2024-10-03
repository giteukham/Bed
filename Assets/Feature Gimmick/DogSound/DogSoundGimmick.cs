using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogSoundGimmick : Gimmick
{
    public override GimmickType Type { get; protected set; } = GimmickType.Object;
    public override float Probability { get; set; } = 100;

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
        SettingVariables();
        StartCoroutine(MainCode());
    }

    public override void Deactivate()
    {
        gimmickManager.LowerProbability(this);
        gimmickManager.objectGimmick = null;
        gameObject.SetActive(false);
    }

    public override void UpdateProbability()
    {
        Probability = 100;
    }

    private IEnumerator MainCode()
    {
        //개 짖다가
        AudioManager.instance.PlaySound(AudioManager.instance.dogBark, transform.position);

        yield return new WaitForSeconds(21);

        //소리 끝나면 바로 개 비명 지름(맞는 소리 넣으면 뭔가 이상할듯)
        AudioManager.instance.PlaySound(AudioManager.instance.dogWhine, transform.position);

        yield return new WaitForSeconds(10);
        //스트레스 데미지 주고 끝냄
        Deactivate();
    }
}
