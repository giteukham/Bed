using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set;}

    [field: Header("Cat SFX")]
    [field: SerializeField] public EventReference catMeow {get; private set;}

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference blanketMoving {get; private set;}

    [field: Header("Radio SFX")]
    [field: SerializeField] public EventReference radio { get; private set; }

    [field: Header("EyeStart SFX")]
    [field: SerializeField] public EventReference eyeStart { get; private set; }

    [field: Header("EyeEnd SFX")]
    [field: SerializeField] public EventReference eyeEnd { get; private set; }

    [field: Header("Lag1 SFX")]
    [field: SerializeField] public EventReference lag1 { get; private set; }

    [field: Header("Lag2 SFX")]
    [field: SerializeField] public EventReference lag2 { get; private set; }

    private void Awake() 
    {
        if (instance != null) Debug.LogError("Audio Manager?? ??? ????");
        instance = this;
    }

    public void PlayOneShot(EventReference _sound, Vector3 _pos) 
    {
        RuntimeManager.PlayOneShot(_sound, _pos);
    }
    //비둘기 꽈매기 가오리 왜가리
}
