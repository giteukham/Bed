using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set;}

    private void Awake() 
    {
        if (instance != null) Debug.LogError("Audio Manager가 이미 존재");
        instance = this;
    }

    public void PlayOneShot(EventReference _sound, Vector3 _pos) 
    {
        RuntimeManager.PlayOneShot(_sound, _pos);
    }
}
