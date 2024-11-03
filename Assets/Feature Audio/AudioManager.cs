using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Unity.VisualScripting;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set;}

    #region Other FMOD Events
    [field: Header("Cat SFX")]
    [field: SerializeField] public EventReference catMeow {get; private set;}

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

    [field: Header("ToyWalk SFX")]
    [field: SerializeField] public EventReference toyWalk { get; private set; }

    [field: Header("Knock SFX")]
    [field: SerializeField] public EventReference knock { get; private set; }

    [field: Header("CogWheell SFX")]
    [field: SerializeField] public EventReference cogWheel { get; private set; }

    [field: Header("NeckSnap SFX")]
    [field: SerializeField] public EventReference neckSnap { get; private set; }

    [field: Header("Crows SFX")]
    [field: SerializeField] public EventReference crows { get; private set; }

    [field: Header("WoodDrop1 SFX")]
    [field: SerializeField] public EventReference woodDrop1 { get; private set; }

    [field: Header("WoodDrop2 SFX")]
    [field: SerializeField] public EventReference woodDrop2 { get; private set; }

    [field: Header("DogBark SFX")]
    [field: SerializeField] public EventReference dogBark { get; private set; }

    [field: Header("DogWhine SFX")]
    [field: SerializeField] public EventReference dogWhine { get; private set; }

    [field: Header("Mosquito SFX")]
    [field: SerializeField] public EventReference mosquito { get; private set; }

    [field: Header("CatFight SFX")]
    [field: SerializeField] public EventReference catFight { get; private set; }

    [field: Header("Chair1 SFX")]
    [field: SerializeField] public EventReference chair1 { get; private set; }

    [field: Header("Chair2 SFX")]
    [field: SerializeField] public EventReference chair2 { get; private set; }

    #endregion

    #region Player FMOD Events
    [field: Header("Head Move On Pillow SFX")]
    [field: SerializeField] public EventReference headMove {get; private set;}

    [field: Header("Fear Whisper SFX")]
    [field: SerializeField] public EventReference fearHal {get; private set;}

    [field: Header("Stress Hallucination SFX")]
    [field: SerializeField] public EventReference stressHal {get; private set;}

    [field: Header("Player Inhale SFX")]
    [field: SerializeField] public EventReference inhale {get; private set;}

    [field: Header("Player Exhale SFX")]
    [field: SerializeField] public EventReference exhale {get; private set;}

    [field: Header("blanketMoving SFX")]
    [field: SerializeField] public EventReference blanketMoving {get; private set;}
    #endregion

    // Key �̺�Ʈ ���� ��, Value �̺�Ʈ �ν��Ͻ�
    private Dictionary<EventReference, EventInstance> eventInstances = new(); 

    private void Awake() 
    {
        if (instance != null) Debug.LogError("Audio Manager already exists");
        instance = this;
    }
    
    /// <summary>
    /// �Ҹ� ������ ������ ����
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

    public void PlayOneShot(EventReference _eventRef, Vector3 _pos)
    {
        RuntimeManager.PlayOneShot(_eventRef, _pos);
    }

    /// <summary>
    /// �Ҹ� ��ġ ����
    /// </summary>
    public void SetPosition(EventReference _eventRef, Vector3 _pos)
    {
        if (eventInstances.TryGetValue(_eventRef, out EventInstance eventInstance)) eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(_pos));
    }

    /// <summary>
    /// �Ҹ� ����
    /// </summary>
    /// <param name="_mode">�Ҹ� ���� ��� IMMEDIATE == �Ϲ�, ALLOWFADEOUT == ���̵� �ƿ�</param>
    public void StopSound(EventReference _eventRef, FMOD.Studio.STOP_MODE _mode)
    {
        if (eventInstances.TryGetValue(_eventRef, out EventInstance eventInstance))
        {
            eventInstance.stop(_mode);
            eventInstance.release();
            eventInstances.Remove(_eventRef);
        }
    }

    /// <summary>
    /// ��� �Ҹ� �� ����
    /// </summary>
    /// /// <param name="_mode">�Ҹ� ���� ���, IMMEDIATE == �Ϲ�, ALLOWFADEOUT  == ���̵� �ƿ�</param>
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
    /// �Ҹ� �Ͻ�����
    /// </summary>
    public void PauseSound(EventReference _eventRef)
    {
        if (eventInstances.TryGetValue(_eventRef, out EventInstance eventInstance)) eventInstance.setPaused(true);
    }

    /// <summary>
    /// ��� �Ҹ� �Ͻ�����
    /// </summary>
    public void PauseAllSounds()
    {
        foreach (EventInstance eventInstance in eventInstances.Values) eventInstance.setPaused(true);
    }

    /// <summary>
    /// �Ҹ� �簳
    /// </summary>
    public void ResumeSound(EventReference _eventRef)
    {
        if (eventInstances.TryGetValue(_eventRef, out EventInstance eventInstance)) eventInstance.setPaused(false);
    }

    /// <summary>
    /// ��� �Ҹ� �簳
    /// </summary>
    public void ResumeAllSounds()
    {
        foreach (EventInstance eventInstance in eventInstances.Values) eventInstance.setPaused(false);
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    public void VolumeControl(EventReference _eventRef, float _volume)
    {
        if (eventInstances.TryGetValue(_eventRef, out EventInstance eventInstance)) eventInstance.setVolume(_volume);
    }

    /// <summary>
    /// ���� �� ��������
    /// </summary>
    public float GetVolume(EventReference _eventRef)
    {
        if (eventInstances.TryGetValue(_eventRef, out EventInstance eventInstance))
        {
            float volume;
            eventInstance.getVolume(out volume);
            return volume;
        }
        return 0;
    }

    /// <summary>
    /// ȿ���� �ߺ� üũ
    /// </summary>
    public bool DuplicateCheck(EventReference _eventRef)
    {
        if (eventInstances.ContainsKey(_eventRef)) return true;
        return false;
    }

    /// <summary>
    /// �Ķ���� �� ����
    /// </summary>
    /// <param name="_paramName">�Ķ���� �̸�</param>
    /// <param name="_value">�Ķ���� ��</param>
    public void SetParameter(EventReference _eventRef, string _paramName, float _value)
    {
        if (eventInstances.TryGetValue(_eventRef, out EventInstance eventInstance)) eventInstance.setParameterByName(_paramName, _value);
    }

    /// <summary>
    /// �Ķ���� �� ��������
    /// </summary>
    /// <param name="_paramName">�Ķ���� �̸�</param>
    public float GetParameter(EventReference _eventRef, string _paramName)
    {
        if (eventInstances.TryGetValue(_eventRef, out EventInstance eventInstance))
        {
            float value;
            eventInstance.getParameterByName(_paramName, out value);
            return value;
        }
        return 0;
    }
}
