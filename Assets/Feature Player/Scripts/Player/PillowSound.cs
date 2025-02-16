
using System;
using System.Collections;
using UnityEngine;

public class PillowSound : MonoBehaviour
{
    private Coroutine headMoveSFXCoroutine;

    private void Update()
    {
        AudioManager.Instance.SetPosition(AudioManager.Instance.headMove, transform.position);
        
        if (PlayerConstant.isRightState || PlayerConstant.isLeftState)  AudioManager.Instance.SetParameter(AudioManager.Instance.headMove, "Lowpass", 1.6f);
        else AudioManager.Instance.SetParameter(AudioManager.Instance.headMove, "Lowpass", 4.5f);
        
        if (PlayerConstant.isShock || PlayerConstant.isPlayerStop) HeadMoveVolume(false);
    }

    public void PlaySound()
    {
        // --------赣府 框流烙 家府
        if ((PlayerConstant.headMoveSpeed > 0f || PlayerConstant.isMovingState) &&
            AudioManager.Instance.GetVolume(AudioManager.Instance.headMove) < 1.0f)
        {
            HeadMoveVolume(true);
        }
        else if (AudioManager.Instance.GetVolume(AudioManager.Instance.headMove) > 0.0f)
        {
            HeadMoveVolume(false);
        }
    }

    private void HeadMoveVolume(bool isUp)
    {
        if (headMoveSFXCoroutine != null) StopCoroutine(headMoveSFXCoroutine);
        headMoveSFXCoroutine = StartCoroutine(headMoveSFXSet(isUp));
    }
    
    IEnumerator headMoveSFXSet(bool _Up)
    {
        float volume = AudioManager.Instance.GetVolume(AudioManager.Instance.headMove);
        
        if(_Up)
        {
            AudioManager.Instance.ResumeSound(AudioManager.Instance.headMove);
            while(volume < 1.0f)
            {
                volume += 0.1f;
                volume = Mathf.Clamp(volume, 0.0f, 1.0f);
                AudioManager.Instance.VolumeControl(AudioManager.Instance.headMove, volume);
                yield return new WaitForSeconds(0.1f);
            }
            headMoveSFXCoroutine = null;
        }
        else
        {
            while(volume > 0.0f)
            {
                volume -= 0.1f;
                volume = Mathf.Clamp(volume, 0.0f, 1.0f);
                AudioManager.Instance.VolumeControl(AudioManager.Instance.headMove, volume);
                yield return new WaitForSeconds(0.1f);
            }
            AudioManager.Instance.PauseSound(AudioManager.Instance.headMove);
            headMoveSFXCoroutine = null;
        }
    }  
}
