using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AbstractGimmick;

public class ClapGimcik : Gimmick
{
    [SerializeField]
    private NewGimmickManager gimmickManager;

    public override GimmickType Type { get; protected set; } = GimmickType.Unreal;

    public override float Probability { get; set; } = 100;

    public Light houseLight;

    public Animator animator;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        timeLimit += Time.deltaTime;
    }

    private void ClapSoundPlay()
    {
        // 박수 소리는 애니메이션 이벤트로 실행
        AudioManager.instance.PlayOneShot(AudioManager.instance.handClap, this.transform.position);
    }

    public override void Activate()
    {
        SettingVariables();
        StartCoroutine(MainCode());
    }

    public override void Deactivate()
    {
        gimmickManager.LowerProbability(this);
        gimmickManager.unrealGimmick = null;
        gameObject.SetActive(false);
    }

    public override void UpdateProbability(ExPlayer player)
    {
        Probability = 100;
    }

    private IEnumerator MainCode()
    {
        print("ClapGimcikTest Start !!");

        yield return new WaitForSeconds(3.2f);
        houseLight.enabled = true;
        
        AudioManager.instance.PlayOneShot(AudioManager.instance.switchOn, this.transform.position);
        // 스위치 키는 소리

        yield return new WaitForSeconds(0.4f);
        animator.Play("Clapping");

        yield return new WaitForSeconds(1.5f);
        animator.Play("ClapOff");

        yield return new WaitForSeconds(0.4f);
        houseLight.enabled = false;

        AudioManager.instance.PlayOneShot(AudioManager.instance.switchOff, this.transform.position);
        // 스위치 끄는 소리

        Deactivate();
    }
}