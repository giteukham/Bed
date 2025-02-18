
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PillowSound : MonoBehaviour
{
    [SerializeField] private GameObject pillowSoundPosition;
    [SerializeField] private GameObject sourcePosition;
    private Coroutine headMoveSFXCoroutine;

    private void Start()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.headMove, transform.position);
    }

    private void Update()
    {
        pillowSoundPosition.transform.localPosition = new Vector3(sourcePosition.transform.localPosition.x, pillowSoundPosition.transform.localPosition.y, pillowSoundPosition.transform.localPosition.z);
        AudioManager.Instance.SetPosition(AudioManager.Instance.headMove, pillowSoundPosition.transform.localPosition);
        
        if (PlayerConstant.isRightState || PlayerConstant.isLeftState)  AudioManager.Instance.SetParameter(AudioManager.Instance.headMove, "Lowpass", 1.6f);
        else AudioManager.Instance.SetParameter(AudioManager.Instance.headMove, "Lowpass", 4.5f);
        
        if (PlayerConstant.isShock || PlayerConstant.isPlayerStop) HeadMoveVolume(false);
    }

    public void PlaySound()
    {
        // --------�Ӹ� ������ �Ҹ�
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
