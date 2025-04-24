
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PillowSound : MonoBehaviour
{
    [SerializeField] private GameObject pillowSoundPosition;
    [SerializeField] private GameObject playerPosition;
    private Coroutine headMoveVolumeSetCoroutine, headMoveLowpassSetCoroutine, headMoveCompressorSetCoroutine;

    private void Start()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.headMove, transform.position);
    }

    private void Update()
    {
        pillowSoundPosition.transform.localPosition = new Vector3(playerPosition.transform.localPosition.x, pillowSoundPosition.transform.localPosition.y, pillowSoundPosition.transform.localPosition.z);
        AudioManager.Instance.SetPosition(AudioManager.Instance.headMove, pillowSoundPosition.transform.localPosition);
        
        if (PlayerConstant.isRightState || PlayerConstant.isLeftState) HeadMoveLowpassSet(false);
        else HeadMoveLowpassSet(true);
        
        if (PlayerConstant.headMoveSpeed > 7) HeadMoveCompressorSet(false);
        else HeadMoveCompressorSet(true);

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
        if (headMoveVolumeSetCoroutine != null) StopCoroutine(headMoveVolumeSetCoroutine);
        headMoveVolumeSetCoroutine = StartCoroutine(headMoveVolumeSet(isUp));
    }
    
    IEnumerator headMoveVolumeSet(bool isUp)
    {
        float volume = AudioManager.Instance.GetVolume(AudioManager.Instance.headMove);
        
        if(isUp)
        {
            AudioManager.Instance.ResumeSound(AudioManager.Instance.headMove);
            while(volume < 1.0f)
            {
                volume += 0.1f;
                volume = Mathf.Clamp(volume, 0.0f, 1.0f);
                AudioManager.Instance.VolumeControl(AudioManager.Instance.headMove, volume);
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
                AudioManager.Instance.VolumeControl(AudioManager.Instance.headMove, volume);
                yield return new WaitForSeconds(0.1f);
            }
            AudioManager.Instance.PauseSound(AudioManager.Instance.headMove);
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
        float paramValue = AudioManager.Instance.GetParameter(AudioManager.Instance.headMove, "Lowpass");
        
        if(isUp)
        {
            while(paramValue < 1f)
            {
                paramValue += 0.1f;
                AudioManager.Instance.SetParameter(AudioManager.Instance.headMove, "Lowpass", paramValue);
                yield return new WaitForSeconds(0.15f);
            }
            headMoveLowpassSetCoroutine = null;
        }
        else
        {
            while(paramValue > 0f)
            {
                paramValue -= 0.1f;
                AudioManager.Instance.SetParameter(AudioManager.Instance.headMove, "Lowpass", paramValue);
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
        float paramValue = AudioManager.Instance.GetParameter(AudioManager.Instance.headMove, "Compressor");
        
        if(isUp)
        {
            while(paramValue < 1f)
            {
                paramValue += 0.1f;
                AudioManager.Instance.SetParameter(AudioManager.Instance.headMove, "Compressor", paramValue);
                yield return new WaitForSeconds(0.025f);
            }
            headMoveCompressorSetCoroutine = null;
        }
        else
        {
            while(paramValue > 0f)
            {
                paramValue -= 0.1f;
                AudioManager.Instance.SetParameter(AudioManager.Instance.headMove, "Compressor", paramValue);
                yield return new WaitForSeconds(0.025f);
            }
            headMoveCompressorSetCoroutine = null;
        }
    }    
}
