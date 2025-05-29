using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Unity.VisualScripting;
using FMOD.Studio;
using UnityEngine.Assertions;

public class AudioManager : MonoSingleton<AudioManager>
{
    // Key 이벤트 참조 값, Value 이벤트 인스턴스
    private Dictionary<EventReference, EventInstance> eventInstances = new();
    [SerializeField] private AudioLibrary audioLibrary;
    [SerializeField] SoundSettings soundSettings;
    private Coroutine playMonitorCoroutine, playOneShotMonitorCoroutine;
    private FMOD.Studio.Bus _masterBus, _gimmickBus, _playerBus;
    public FMOD.Studio.Bus MasterBus
    {
        get
        {
            if (!_masterBus.isValid()) _masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
            return _masterBus;
        }
        private set => _masterBus = value;
    }

    public FMOD.Studio.Bus GimmickBus
    {
        get
        {
            if (!_gimmickBus.isValid()) _gimmickBus = FMODUnity.RuntimeManager.GetBus("bus:/Gimmick");
            return _gimmickBus;
        }
        private set => _gimmickBus = value;
    }

    public FMOD.Studio.Bus PlayerBus
    {
        get
        {
            if (!_playerBus.isValid()) _playerBus = FMODUnity.RuntimeManager.GetBus("bus:/Player");
            return _playerBus;
        }
        private set => _playerBus = value;
    }

    /// <summary>
    /// 소리 꺼지기 전까지 실행
    /// </summary>
    public void PlaySound(EventReference _eventRef, Vector3 _pos)
    {
        if (eventInstances.ContainsKey(_eventRef)) return;

        EventInstance eventInstance = RuntimeManager.CreateInstance(_eventRef);
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(_pos));
        eventInstances[_eventRef] = eventInstance;
        eventInstance.start();

        if (playMonitorCoroutine != null) StopCoroutine(playMonitorCoroutine);
        playMonitorCoroutine = StartCoroutine(MonitorPlayback(_eventRef));
    }

    /// <summary>
    /// 소리가 중복 재생 가능
    /// </summary>
    /// <param name="_eventRef"></param>
    /// <param name="_pos"></param>
    public void PlayOneShot(EventReference _eventRef, Vector3 _pos)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(_eventRef);
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(_pos));
        eventInstances[_eventRef] = eventInstance;
        eventInstance.start();
        
        if (playOneShotMonitorCoroutine != null) StopCoroutine(playOneShotMonitorCoroutine);
        playOneShotMonitorCoroutine = StartCoroutine(MonitorPlayback(_eventRef));
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
    /// 소리 위치 설정
    /// </summary>
    public void SetPosition(EventReference _eventRef, Vector3 _pos)
    {
        if (eventInstances.TryGetValue(_eventRef, out EventInstance eventInstance)) eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(_pos));
    }

    /// <summary>
    /// 소리 끄기
    /// </summary>
    /// <param name="_mode">소리 끄는 모드 IMMEDIATE == 일반, ALLOWFADEOUT == 페이드 아웃</param>
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

    /// <summary>
    /// 볼륨 조절
    /// </summary>
    public void VolumeControl(EventReference _eventRef, float _volume)
    {
        if (eventInstances.TryGetValue(_eventRef, out EventInstance eventInstance)) eventInstance.setVolume(_volume);
    }

    /// <summary>
    /// 모든 소리 볼륨 %로 맞추기
    /// </summary>
    public void AllVolumeDown(float _volume)
    {   
        MasterBus.setVolume(soundSettings.MasterVolume * _volume);
    }

    /// <summary>
    /// 모든 소리 볼륨 초기화
    /// </summary>
    public void AllVoumeInit()
    {
        MasterBus.setVolume(soundSettings.MasterVolume);
    }

    /// <summary>
    /// 볼륨 값 가져오기
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
    /// 효과음 중복 체크
    /// </summary>
    public bool DuplicateCheck(EventReference _eventRef)
    {
        if (eventInstances.ContainsKey(_eventRef)) return true;
        return false;
    }

    /// <summary>
    /// 이벤트 파라미터 값 설정
    /// </summary>
    /// <param name="_paramName">파라미터 이름</param>
    /// <param name="_value">파라미터 값</param>
    public void SetParameter(EventReference _eventRef, string _paramName, float _value)
    {
        if (eventInstances.TryGetValue(_eventRef, out EventInstance eventInstance)) eventInstance.setParameterByName(_paramName, _value);
    }

    /// <summary>
    /// 시스템 파라미터 값 설정
    /// </summary>
    /// <param name="_paramName"></param>
    /// <param name="_value"></param>
    public void SetParameter(string _paramName, float _value)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(_paramName, _value);
    }

    /// <summary>
    /// 파라미터 값 가져오기
    /// </summary>
    /// <param name="_paramName">파라미터 이름</param>
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
    
    /// <summary>
    /// 소리 길이 가져오기
    /// </summary>
    /// <param name="_eventRef"></param>
    /// <returns></returns>
    public float GetSoundLength(EventReference _eventRef)
    {
        if (eventInstances.TryGetValue(_eventRef, out EventInstance eventInstance))
        {
            if (!eventInstance.isValid()) Debug.LogException(new Exception("EventInstance is not valid"));
            
            eventInstance.getDescription(out EventDescription eventDescription);
            eventDescription.getLength(out int length);
            return length / 1000f;
        }
        
        return 0;
    }

    /// <summary>
    /// 소리 재생 상태 가져오기
    /// </summary>
    /// <param name="_eventRef"></param>
    /// <returns></returns>
    public PLAYBACK_STATE GetPlaybackState(EventReference _eventRef)
    {
        EventInstance eventInstance = eventInstances[_eventRef];
        eventInstance.getPlaybackState(out PLAYBACK_STATE playbackState);
        return playbackState;
    }

    /// <summary>
    /// 소리 타임라인 위치 가져오기
    /// </summary>
    /// <param name="_eventRef"></param>
    /// <param name="_time"></param>
    public void GetTimeLinePosition(EventReference _eventRef, out int _time)
    {
        if (eventInstances.TryGetValue(_eventRef, out EventInstance eventInstance))
            eventInstance.getTimelinePosition(out _time);
        else _time = 0;
    }

    /// <summary>
    /// 소리 타임라인 위치 정하기
    /// </summary>
    /// <param name="_eventRef"></param>
    /// <param name="_time"></param>
    public void SetTimeLinePosition(EventReference _eventRef, int _time)
    {
        if (eventInstances.TryGetValue(_eventRef, out EventInstance eventInstance))
            eventInstance.setTimelinePosition(_time);
    }

    /// <summary>
    /// 모든 소리 타임라인 위치 가져오기
    /// </summary>
    /// <param name="_timeDict"></param>
    public void GetAllTimeLinePosition(out Dictionary<EventReference, int> _timeDict)
    {
        _timeDict = new Dictionary<EventReference, int>();
        foreach (KeyValuePair<EventReference, EventInstance> kvp in eventInstances)
        {
            kvp.Value.getTimelinePosition(out int time);
            _timeDict[kvp.Key] = time;
        }
    }

    /// <summary>
    /// 모든 소리 타임라인 위치 정하기
    /// </summary>
    /// <param name="_timeDict"></param>
    /// <param name="_time"></param>
    public void SetAllTimeLinePosition(Dictionary<EventReference, int> _timeDict, int _time)
    {
        foreach (KeyValuePair<EventReference, EventInstance> kvp in eventInstances)
        {
            if (_timeDict.TryGetValue(kvp.Key, out int time))
            {
                kvp.Value.setTimelinePosition(time + _time);
            }
            else
            {
                kvp.Value.setTimelinePosition(time + _time);
            }
        }
    }

    /// <summary>
    /// 모든 소리 타임라인 위치 되감기
    /// </summary>
    /// <param name="_time"></param>
    public void RewindAllSounds(int _time)
    {
        foreach (EventInstance eventInstance in eventInstances.Values)
        {
            eventInstance.getTimelinePosition(out int time);
            eventInstance.setTimelinePosition(Math.Max(time + _time, 0));
        }
    }

    /// <summary>
    /// 소리 타임라인 위치 되감기
    /// </summary>
    /// <param name="_eventRef"></param>
    /// <param name="_time"></param>
    public void RewindSound(EventReference _eventRef, int _time)
    {
        if (eventInstances.TryGetValue(_eventRef, out EventInstance eventInstance))
        {
            eventInstance.getTimelinePosition(out int time);
            eventInstance.setTimelinePosition(Math.Max(time + _time, 0));
        }
    }
}
