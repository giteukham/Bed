using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Unity.VisualScripting;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set;}

    #region FMOD Events
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

    [field: Header("ElevatorCrash SFX")]
    [field: SerializeField] public EventReference elevatorCrash { get; private set; }

    [field: Header("ElevatorMove SFX")]
    [field: SerializeField] public EventReference elevatorMove { get; private set; }

    [field: Header("ElevatorFast SFX")]
    [field: SerializeField] public EventReference elevatorFast { get; private set; }

    [field: Header("Elevator Stop SFX")]
    [field: SerializeField] public EventReference elevatorStop { get; private set; }

    [field: Header("Elevator Hit SFX")]
    [field: SerializeField] public EventReference hit { get; private set; }
    
    [field: Header("Elevator Door Open SFX")]
    [field: SerializeField] public EventReference elevatorDoorOpen { get; private set; }

    [field: Header("Elevator Ding SFX")]
    [field: SerializeField] public EventReference elevatorDing { get; private set; }

    [field: Header("Creepy Vocal 1 SFX")]
    [field: SerializeField] public EventReference creepyVocal1 { get; private set; }

    [field: Header("Creepy Vocal 2 SFX")]
    [field: SerializeField] public EventReference creepyVocal2 { get; private set; }

    [field: Header("Creepy Laugh 1 SFX")]
    [field: SerializeField] public EventReference creepyLaugh1 { get; private set; }

    [field: Header("Creepy Laugh 2 SFX")]
    [field: SerializeField] public EventReference creepyLaugh2 { get; private set; }

    [field: Header("Creepy Laugh 3 SFX")]
    [field: SerializeField] public EventReference creepyLaugh3 { get; private set; }

    [field: Header("Creepy Laugh 4 SFX")]
    [field: SerializeField] public EventReference creepyLaugh4 { get; private set; }

    [field: Header("Creepy Scream SFX")]
    [field: SerializeField] public EventReference creepyScream { get; private set; }

    #endregion

    // Key 이벤트 참조 값, Value 이벤트 인스턴스
    [SerializeField]private Dictionary<EventReference, EventInstance> eventInstances = new(); 

    private void Awake() 
    {
        if (instance != null) Debug.LogError("Audio Manager already exists");
        instance = this;
    }
    
    /// <summary>
    /// 소리 꺼지기 전까지 실행
    /// </summary>
    public void PlaySound(EventReference _eventRef, Vector3 _pos)
    {
        //if (eventInstances.ContainsKey(_eventRef)) return;

        EventInstance eventInstance = RuntimeManager.CreateInstance(_eventRef);
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(_pos));
        eventInstances[_eventRef] = eventInstance;
        eventInstance.start();

        StartCoroutine(MonitorPlayback(_eventRef));
    }

    private IEnumerator MonitorPlayback(EventReference _eventRef)
    {
        EventInstance eventInstance = eventInstances[_eventRef];
        PLAYBACK_STATE playbackState;
        do
        {
            eventInstance.getPlaybackState(out playbackState);
            yield return null;
        }
        while (playbackState != PLAYBACK_STATE.STOPPED);
        StopSound(_eventRef, FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    /// <summary>
    /// 소리 위치 업데이트
    /// </summary>
    public void UpdateSoundPosition(EventReference _eventRef, Vector3 _pos)
    {
        if(!eventInstances.ContainsKey(_eventRef)) return;
        EventInstance eventInstance = eventInstances[_eventRef];
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(_pos));
    }

    /// <summary>
    /// 소리 끄기
    /// </summary>
    /// <param name="_mode">소리 끄는 모드 IMMEDIATE == 일반, ALLOWFADEOUT == 페이드 아웃</param>
    public void StopSound(EventReference _eventRef, FMOD.Studio.STOP_MODE _mode)
    {
        if (eventInstances.ContainsKey(_eventRef))
        {
            eventInstances[_eventRef].stop(_mode);
            eventInstances[_eventRef].release();
            eventInstances.Remove(_eventRef);
        }
    }

    /// <summary>
    /// 모든 소리 다 끄기
    /// </summary>
    /// /// <param name="_mode">소리 끄는 모드, IMMEDIATE == 일반, ALLOWFADEOUT  == 페이드 아웃</param>
    public void StopAllSounds(FMOD.Studio.STOP_MODE _mode)
    {
        foreach (EventInstance eventInstance in eventInstances.Values)
        {
            eventInstance.stop(_mode);
            eventInstance.release();
        }
        eventInstances.Clear();
    }
    
    /// <summary>
    /// 소리 일시정지
    /// </summary>
    public void PauseSound(EventReference _eventRef)
    {
        if (eventInstances.TryGetValue(_eventRef, out EventInstance eventInstance)) eventInstance.setPaused(true);
    }

    /// <summary>
    /// 모든 소리 일시정지
    /// </summary>
    public void PauseAllSounds()
    {
        foreach (EventInstance eventInstance in eventInstances.Values) eventInstance.setPaused(true);
    }

    /// <summary>
    /// 소리 재개
    /// </summary>
    public void ResumeSound(EventReference _eventRef)
    {
        if (eventInstances.TryGetValue(_eventRef, out EventInstance eventInstance)) eventInstance.setPaused(false);
    }

    /// <summary>
    /// 모든 소리 재개
    /// </summary>
    public void ResumeAllSounds()
    {
        foreach (EventInstance eventInstance in eventInstances.Values) eventInstance.setPaused(false);
    }
}
