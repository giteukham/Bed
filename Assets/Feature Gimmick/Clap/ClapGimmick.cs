using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AbstractGimmick;

public class ClapGimcik : Gimmick
{
    #region Override Variables
    [field: SerializeField] public override GimmickType type { get; protected set; }
    [field: SerializeField] public override float probability { get; set; } = 100;
    [field: SerializeField] public override List<Gimmick> ExclusionGimmickList { get; set; }
    #endregion

    #region Variables
    public Light houseLight;
    public Animator animator;
    #endregion

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

        // 테스트로 방 불도 켜지고 꺼지게
        if(BedRoomLightSwitch.isOn) BedRoomLightSwitch.SwitchAction(false);
        else BedRoomLightSwitch.SwitchAction(true);
    }

    public override void Activate()
    {
        base.Activate();
        StartCoroutine(MainCode());
    }

    public override void Deactivate()
    {
        base.Deactivate();
        gameObject.SetActive(false);
    }

    public override void UpdateProbability()
    {
        probability = 100;
    }

    public override void Initialize(){}

    private IEnumerator MainCode()
    {
        Door.Set(45, 0.7f); // 방문 열기
        yield return new WaitForSeconds(1.3f);
        LivingRoomLightSwitch.SwitchAction(true);   // 복도 불 켜기

        yield return new WaitForSeconds(0.4f);
        animator.Play("Clapping");

        yield return new WaitForSeconds(1.5f);
        animator.Play("ClapOff");

        yield return new WaitForSeconds(0.4f);
        LivingRoomLightSwitch.SwitchAction(false);  // 복도 불 끄기

        yield return new WaitForSeconds(0.2f);
        Door.Set(0, 0.2f); // 방문 닫기

        yield return new WaitForSeconds(0.4f);
        Deactivate();
    }


}