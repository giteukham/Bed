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

    [field: Header("radio SFX")]
    [field: SerializeField] public EventReference radio { get; private set; }

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
