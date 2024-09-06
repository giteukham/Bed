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
        // ë°•ìˆ˜ ì†Œë¦¬ëŠ” ì• ë‹ˆë©”ì´ì…˜ ì´ë²¤íŠ¸ë¡œ ì‹¤í–‰
        AudioManager.instance.PlaySound(AudioManager.instance.handClap, this.transform.position);

        // Å×½ºÆ®·Î ¹æ ºÒµµ ÄÑÁö°í ²¨Áö°Ô
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
        Door.Set(45, 0.7f); // ¹æ¹® ¿­±â
        yield return new WaitForSeconds(1.3f);
        LivingRoomLightSwitch.SwitchAction(true);   // º¹µµ ºÒ ÄÑ±â

        yield return new WaitForSeconds(0.4f);
        animator.Play("Clapping");

        yield return new WaitForSeconds(1.5f);
        animator.Play("ClapOff");

        yield return new WaitForSeconds(0.4f);
        LivingRoomLightSwitch.SwitchAction(false);  // º¹µµ ºÒ ²ô±â

        yield return new WaitForSeconds(0.2f);
        Door.Set(0, 0.2f); // ¹æ¹® ´İ±â

        yield return new WaitForSeconds(0.4f);
        Deactivate();
    }
}