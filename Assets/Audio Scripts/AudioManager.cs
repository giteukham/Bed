using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Unity.VisualScripting;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set;}

    [field: Header("Cat SFX")]
    [field: SerializeField] public EventReference catMeow {get; private set;}


    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference blanketMoving {get; private set;}


    [field: Header("Light Switch On SFX")]
    [field: SerializeField] public EventReference switchOn {get; private set;}

    
    [field: Header("Light Switch Off SFX")]
    [field: SerializeField] public EventReference switchOff {get; private set;}
    

    [field: Header("Hand Clap SFX")]
    [field: SerializeField] public EventReference handClap {get; private set;}


    [field: Header("Hand Cover SFX")]
    [field: SerializeField] public EventReference handCover {get; private set;}

    [field: Header("Rough Breath SFX")]
    [field: SerializeField] public EventReference roughBreath {get; private set;}

    [field: Header("Hand Cover Off SFX")]
    [field: SerializeField] public EventReference handCoverOff {get; private set;}

    [field: Header("WindowOpenClose SFX")]
    [field: SerializeField] public EventReference windowOpenClose {get; private set;}

    [field: Header("PantRustle SFX")]
    [field: SerializeField] public EventReference pantRustle {get; private set;}

    [field: Header("Rapist4Phase SFX")]
    [field: SerializeField] public EventReference rapist4Phase {get; private set;}

    [field: Header("Horny Breath SFX")]
    [field: SerializeField] public EventReference hornyBreath {get; private set;}

    private void Awake() 
    {
        if (instance != null) Debug.LogError("Audio Manager?? ??? ????");
        instance = this;
    }

    public void PlayOneShot(EventReference _sound, Vector3 _pos) 
    {
        RuntimeManager.PlayOneShot(_sound, _pos);
    }
}
