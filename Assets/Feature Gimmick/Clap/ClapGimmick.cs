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
        // 박수 소리는 애니메이션 이벤트로 실행
        AudioManager.instance.PlaySound(AudioManager.instance.handClap, this.transform.position);

        // �׽�Ʈ�� �� �ҵ� ������ ������
        if(BedRoomLightSwitch.isOn) BedRoomLightSwitch.SwitchAction(false);
        else BedRoomLightSwitch.SwitchAction(true);
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
        Door.Set(45, 0.7f); // �湮 ����
        yield return new WaitForSeconds(1.3f);
        LivingRoomLightSwitch.SwitchAction(true);   // ���� �� �ѱ�

        yield return new WaitForSeconds(0.4f);
        animator.Play("Clapping");

        yield return new WaitForSeconds(1.5f);
        animator.Play("ClapOff");

        yield return new WaitForSeconds(0.4f);
        LivingRoomLightSwitch.SwitchAction(false);  // ���� �� ����

        yield return new WaitForSeconds(0.2f);
        Door.Set(0, 0.2f); // �湮 �ݱ�

        yield return new WaitForSeconds(0.4f);
        Deactivate();
    }
}