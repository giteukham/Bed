using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using FMODUnity;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class Whisper : SoundOnlyGimmick, IEarGimmick
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
        if (GameManager.Instance.isDemo) probability = 100f;
    }
    
    public override void Activate()
    {
        base.Activate();
        StartCoroutine(ActivateBreathing());
        AudioManager.Instance.PlaySound(soundEvent, transform.position);
    }
    
    public override void Initialize() { }

    private void Start()
    {
        soundEvent = AudioManager.Instance.whisperInRoom;
    }
    
    private IEnumerator ActivateBreathing()
    {
        isActive = true;

        var random = new Transform[2] {leftEar, rightEar};
        var randomInt = Random.Range(0, 2);
        
        while (true)
        {
            if (isActive == false) yield break;
            
            if (PlayerConstant.isLeftState)
            {
                AudioManager.Instance.SetPosition(soundEvent, rightEar.position);
            }
            else if (PlayerConstant.isRightState)
            {
                AudioManager.Instance.SetPosition(soundEvent, leftEar.position);
            }
            else if (PlayerConstant.isMiddleState)
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
