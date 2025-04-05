using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AbstractGimmick;

public class ClapGimcik : Gimmick
{
    #region Override Variables
    [field: SerializeField] public override GimmickType type { get; protected set; }
    [SerializeField] private float _probability;
    public override float probability 
    { 
        get => _probability; 
        set => _probability = Mathf.Clamp(value, 0, 100); 
    }
    [field: SerializeField] public override List<Gimmick> ExclusionGimmickList { get; set; }
    #endregion

    #region Variables
    public Light houseLight;
    public Animator animator;
    #endregion

    private void Awake()
    {
        //gameObject.SetActive(false);
    }

    private void Update()
    {
        
    }

    private void ClapSoundPlay()
    {
        // 박수 소리는 애니메이션 이벤트로 실행
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.handClap, this.transform.position);

        // 테스트로 방 불도 켜지고 꺼지게
        // if(BedRoomLightSwitch.isOn) BedRoomLightSwitch.SwitchAction(false);
        // else BedRoomLightSwitch.SwitchAction(true);
    }

    public override void Activate()
    {
        base.Activate();
        //gameObject.SetActive(true);
        StartCoroutine(MainCode());
    }

    public override void Deactivate()
    {
        base.Deactivate();
        gameObject.SetActive(false);
    }

    public override void UpdateProbability()
    {
        probability = ((PlayerConstant.LeftLookLAT * 2) + (PlayerConstant.LeftLookCAT / 4) + PlayerConstant.LeftFrontLookLAT + (PlayerConstant.LeftFrontLookCAT / 8) + PlayerConstant.LeftStateLAT + (PlayerConstant.LeftStateCAT / 10)) 
                    * (PlayerConstant.isEyeOpen ? 1 : 0);
    }

    public override void Initialize(){}

    private IEnumerator MainCode()
    {
        Door.Set(45, 0.7f); // 방문 열기
        // GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Stress, +5);
        // GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.noise, +5);
        yield return new WaitForSeconds(1.3f);
        LivingRoomLightSwitch.SwitchAction(true);   // 복도 불 켜기

        yield return new WaitForSeconds(0.4f);
        // GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.noise, +5);
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