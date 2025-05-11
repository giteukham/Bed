
using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public abstract class SoundOnlyGimmick : Gimmick
{
    protected abstract EventReference soundEvent { get; set; }
    
    private Coroutine deactivateCoroutine;

    public abstract override void UpdateProbability();

    public abstract override void Initialize();

    public override void Activate()
    {
        base.Activate();
        AudioManager.Instance.PlaySound(soundEvent, transform.position);
        deactivateCoroutine = StartCoroutine(DeactivateSoundWhenFinished());
    }
    
    public override void Deactivate()
    {
        base.Deactivate();
        AudioManager.Instance.StopSound(soundEvent, STOP_MODE.IMMEDIATE);
        
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
            deactivateCoroutine = null;
        }
    }

    private IEnumerator DeactivateSoundWhenFinished()
    {
        yield return new WaitForSeconds(AudioManager.Instance.GetSoundLength(soundEvent));
        Deactivate();
    }
}
