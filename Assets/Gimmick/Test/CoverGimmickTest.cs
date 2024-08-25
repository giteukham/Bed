using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AbstractGimmick;

public class CoverGimcikTest : Gimmick
{
    [SerializeField]
    private NewGimmickManager gimmickManager;

    public override GimmickType Type { get; protected set; } = GimmickType.Unreal;
    public override float Probability { get; set; } = 100;

    public Animator animator;

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
        gimmickManager.unrealGimmick = null;
        gameObject.SetActive(false);
    }

    public override void UpdateProbability(ExPlayer player)
    {
        Probability = 100;
    }
    private IEnumerator MainCode()
    {
        yield return new WaitForSeconds(4);
        AudioManager.instance.PlayOneShot(AudioManager.instance.handCover, this.transform.position);
        animator.Play("CoverEye");

        yield return new WaitForSeconds(0.16f);
        AudioManager.instance.PlayOneShot(AudioManager.instance.roughBreath, this.transform.position);

        yield return new WaitForSeconds(2.68f);
        AudioManager.instance.PlayOneShot(AudioManager.instance.handCoverOff, this.transform.position);

        yield return new WaitForSeconds(0.3f);
        animator.Play("CoverOffEye");

        yield return new WaitForSeconds(0.15f);
        Deactivate();
    }
}