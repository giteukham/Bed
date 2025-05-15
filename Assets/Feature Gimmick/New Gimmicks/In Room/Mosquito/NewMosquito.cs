using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using FMODUnity;
using UnityEngine;

public class NewMosquito : SoundOnlyGimmick, IEarGimmick
{
    public override GimmickType type { get; protected set; }
    public override float probability { get; set; }
    public override List<Gimmick> ExclusionGimmickList { get; set; }
    protected override EventReference soundEvent { get; set; }

    [SerializeField]
    private Transform leftEar;
    
    [SerializeField]
    private Transform rightEar;

    private bool isActive = false;

    public override void UpdateProbability()
    {
        probability = 100f;
    }

    public override void Initialize() { }

    
    private void Start()
    {
        soundEvent = AudioManager.Instance.mosquitoInRoom;
    }

    public override void Activate()
    {
        base.Activate();
        
        StartCoroutine(SetMosquitoSoundPosition());
        PlaySound();
    }

    private void PlaySound()
    {
        AudioManager.Instance.PlaySound(soundEvent, transform.position);
        
        // SetParameter는 무조건 소리 재생 후에 호출 해야 적용됨
        if(PlayerConstant.isLeftState && !PlayerConstant.isRightState)
            AudioManager.Instance.SetParameter(AudioManager.Instance.mosquitoInRoom, "isLeftState", 1);
        else if (PlayerConstant.isRightState && !PlayerConstant.isLeftState)
            AudioManager.Instance.SetParameter(AudioManager.Instance.mosquitoInRoom, "isLeftState", 0);
        else if (!PlayerConstant.isLeftState && !PlayerConstant.isRightState)
        {   // 왼쪽 오른쪽도 아닌 정면을 볼땐 무작위로 재생
            int randomInt = UnityEngine.Random.Range(0, 2);
            AudioManager.Instance.SetParameter(AudioManager.Instance.mosquitoInRoom, "isLeftState", randomInt);
        }
    }
    
    private IEnumerator SetMosquitoSoundPosition()
    {
        isActive = true;
        
        var random = new Transform[2] {leftEar, rightEar};
        int randomInt = UnityEngine.Random.Range(0, 2);
        
        while (true)
        {
            if (isActive == false) yield break;
            
            if (PlayerConstant.isLeftState && !PlayerConstant.isRightState)
            {
                AudioManager.Instance.SetPosition(soundEvent, rightEar.position);
            }
            else if (PlayerConstant.isRightState && !PlayerConstant.isLeftState)
            {
                AudioManager.Instance.SetPosition(soundEvent, leftEar.position);
            }
            else if (!PlayerConstant.isLeftState && !PlayerConstant.isRightState)
            {
                AudioManager.Instance.SetPosition(soundEvent, random[randomInt].position);
            }

            yield return null;
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();
        isActive = false;
    }


}
