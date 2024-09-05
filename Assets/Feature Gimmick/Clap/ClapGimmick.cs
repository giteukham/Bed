using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AbstractGimmick;

public class ClapGimcik : Gimmick
{
    [SerializeField]
    private GimmickManager gimmickManager;

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
        // �ڼ� �Ҹ��� �ִϸ��̼� �̺�Ʈ�� ����
        AudioManager.instance.PlaySound(AudioManager.instance.handClap, this.transform.position);
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

    public override void UpdateProbability()
    {
        Probability = 100;
    }

    private IEnumerator MainCode()
    {
        yield return new WaitForSeconds(3.2f);
        LivingRoomLightSwitch.SwitchAction(true);

        yield return new WaitForSeconds(0.4f);
        animator.Play("Clapping");

        yield return new WaitForSeconds(1.5f);
        animator.Play("ClapOff");

        yield return new WaitForSeconds(0.4f);
        LivingRoomLightSwitch.SwitchAction(false);

        Deactivate();
    }
}