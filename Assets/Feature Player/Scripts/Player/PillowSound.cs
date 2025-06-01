
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PillowSound : MonoBehaviour
{
    [SerializeField] private GameObject pillowSoundPosition;
    [SerializeField] private GameObject playerPosition;
    private Coroutine headMoveVolumeSetCoroutine, headMoveLowpassSetCoroutine, headMoveCompressorSetCoroutine;
    private Guid headMoveGuid;
    private void Start()
    {
        headMoveGuid = AudioManager.Instance.PlayLooped(AudioKeys.HeadMove, transform.position);
    }

    private void Update()
    {
        pillowSoundPosition.transform.localPosition = new Vector3(playerPosition.transform.localPosition.x, pillowSoundPosition.transform.localPosition.y, pillowSoundPosition.transform.localPosition.z);
        AudioManager.Instance.SetPosition(headMoveGuid, pillowSoundPosition.transform.localPosition);
        
        if (PlayerConstant.isRightState || PlayerConstant.isLeftState) HeadMoveLowpassSet(false);
        else HeadMoveLowpassSet(true);
        
        if (PlayerConstant.headMoveSpeed > 7) HeadMoveCompressorSet(false);
        else HeadMoveCompressorSet(true);

        if (PlayerConstant.isShock || PlayerConstant.isPlayerStop || !PlayerConstant.isPillowSound) HeadMoveVolume(false);
    }

    public void PlaySound()
    {
        if ((PlayerConstant.headMoveSpeed > 0f || PlayerConstant.isMovingState) &&
            AudioManager.Instance.GetVolume(headMoveGuid) < 1.0f)
        {
            HeadMoveVolume(true);
        }
        else if (AudioManager.Instance.GetVolume(headMoveGuid) > 0.0f)
        {
            HeadMoveVolume(false);
        }
    }

    private void HeadMoveVolume(bool isUp)
    {
        if (headMoveVolumeSetCoroutine != null) StopCoroutine(headMoveVolumeSetCoroutine);
        headMoveVolumeSetCoroutine = StartCoroutine(headMoveVolumeSet(isUp));
    }
    
    IEnumerator headMoveVolumeSet(bool isUp)
    {
        float volume = AudioManager.Instance.GetVolume(headMoveGuid);
        
        if(isUp)
        {
            AudioManager.Instance.PauseSound(headMoveGuid, false);
            while(volume < 1.0f)
            {
                volume += 0.1f;
                volume = Mathf.Clamp(volume, 0.0f, 1.0f);
                AudioManager.Instance.VolumeControl(headMoveGuid, volume);
                yield return new WaitForSeconds(0.1f);
            }
            headMoveVolumeSetCoroutine = null;
        }
        else
        {
            while(volume > 0.0f)
            {
                volume -= 0.1f;
                volume = Mathf.Clamp(volume, 0.0f, 1.0f);
                AudioManager.Instance.VolumeControl(headMoveGuid, volume);
                yield return new WaitForSeconds(0.1f);
            }
            AudioManager.Instance.PauseSound(headMoveGuid, true);
            headMoveVolumeSetCoroutine = null;
        }
    }

    private void HeadMoveLowpassSet(bool isUp)
    {
        if (headMoveLowpassSetCoroutine != null) StopCoroutine(headMoveLowpassSetCoroutine);
        headMoveLowpassSetCoroutine = StartCoroutine(headMoveLowpassSet(isUp));
    }

    IEnumerator headMoveLowpassSet(bool isUp)
    {
        float paramValue = AudioManager.Instance.GetParameter(headMoveGuid, "Lowpass");
        
        if(isUp)
        {
            while(paramValue < 1f)
            {
                paramValue += 0.1f;
                AudioManager.Instance.SetEventParameter(headMoveGuid, "Lowpass", paramValue);
                yield return new WaitForSeconds(0.15f);
            }
            headMoveLowpassSetCoroutine = null;
        }
        else
        {
            while(paramValue > 0f)
            {
                paramValue -= 0.1f;
                AudioManager.Instance.SetEventParameter(headMoveGuid, "Lowpass", paramValue);
                yield return new WaitForSeconds(0.15f);
            }
            headMoveLowpassSetCoroutine = null;
        }
    }

    private void HeadMoveCompressorSet(bool isUp)
    {
        if (headMoveCompressorSetCoroutine != null) StopCoroutine(headMoveCompressorSetCoroutine);
        headMoveCompressorSetCoroutine = StartCoroutine(headMoveCompressorSet(isUp));
    }

    IEnumerator headMoveCompressorSet(bool isUp)
    {
        float paramValue = AudioManager.Instance.GetParameter(headMoveGuid, "Compressor");
        
        if(isUp)
        {
            while(paramValue < 1f)
            {
                paramValue += 0.1f;
                AudioManager.Instance.SetEventParameter(headMoveGuid, "Compressor", paramValue);
                yield return new WaitForSeconds(0.025f);
            }
            headMoveCompressorSetCoroutine = null;
        }
        else
        {
            while(paramValue > 0f)
            {
                paramValue -= 0.1f;
                AudioManager.Instance.SetEventParameter(headMoveGuid, "Compressor", paramValue);
                yield return new WaitForSeconds(0.025f);
            }
            headMoveCompressorSetCoroutine = null;
        }
    }    
}
