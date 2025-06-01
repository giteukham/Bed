using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Unity.VisualScripting;
using FMOD.Studio;
using UnityEngine.Assertions;
using System.Linq;

/// <summary>
/// 하나의 사운드 인스턴스를 추적하기 위한 클래스
/// - EventReference: 참조한 FMOD 이벤트
/// - Guid: 식별용 ID Handle
/// - Instance: FMOD EventInstance
/// - MonitorCoroutine: 재생 상태 감시용 코루틴
/// </summary>
public class TrackedSound
{
    public EventReference EventRef;
    public Guid Id;
    public EventInstance Instance;
    public Coroutine MonitorCoroutine;
}

public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private AudioLibrary audioLibrary; // AudioEntry 목록을 갖는 ScriptableObject

    // 현재 재생 중인 사운드들을 Key별로 저장
    private readonly Dictionary<EventReference, List<TrackedSound>> activeSounds = new();

    // 각 개별 사운드를 Guid로 빠르게 찾기 위해 Dictionary 사용
    private readonly Dictionary<Guid, TrackedSound> guidToSound = new();

    // TrackedSound 재사용을 위한 풀
    private readonly Queue<TrackedSound> soundPool = new();

    [SerializeField] SoundSettings soundSettings;
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

    void Awake()
    {
        for (int i = 0; i < 20; i++)
            soundPool.Enqueue(new TrackedSound());
    }

    private Guid InternalPlay(EventReference evt, Vector3 pos)
    {
        EventInstance instance = RuntimeManager.CreateInstance(evt);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(pos));
        instance.start();

        Guid id = Guid.NewGuid();

        var tracked = soundPool.Count > 0 ? soundPool.Dequeue() : new TrackedSound();
        tracked.EventRef = evt;
        tracked.Id = id;
        tracked.Instance = instance;
        tracked.MonitorCoroutine = StartCoroutine(MonitorSound(id));

        if (!activeSounds.ContainsKey(evt))
            activeSounds[evt] = new List<TrackedSound>();
        activeSounds[evt].Add(tracked);
        guidToSound[id] = tracked;

        return id;
    }
    private IEnumerator MonitorSound(Guid id)
    {
        if (guidToSound.TryGetValue(id, out var tracked))
        {
            EventReference evt = tracked.EventRef;
            EventInstance instance = tracked.Instance;

            instance.getPlaybackState(out var state);
            while (state != PLAYBACK_STATE.STOPPED)
            {
                yield return null;
                instance.getPlaybackState(out state);
            }

            instance.release();
            guidToSound.Remove(id);
            if (activeSounds.ContainsKey(evt))
                activeSounds[evt].RemoveAll(trackedSound => trackedSound.Id == id);
            if (activeSounds[evt].Count == 0)
                activeSounds.Remove(evt);

            soundPool.Enqueue(InitializeTrackedSound(tracked));
        }
    }

    /// <summary>
    /// 재생 중이면 무시 (중복 방지)
    /// </summary>
    public Guid Play(string key, Vector3 pos)
    {
        EventReference evt = audioLibrary.Get(key);
        if (evt.IsNull) 
        {
            Debug.LogWarning($"Audio Library에서 '{key}' 키를 찾을 수 없습니다");
            return Guid.Empty;
        }

        // 이미 재생 중인 사운드가 있는지 확인
        if (activeSounds.TryGetValue(evt, out var soundList))  
        {
            foreach (var s in soundList)
            {
                if (s.Instance.isValid())
                {
                    s.Instance.getPlaybackState(out var state);
                    if (state != PLAYBACK_STATE.STOPPED)
                        return Guid.Empty;
                }
            }
        }
        return InternalPlay(evt, pos);
    }

    /// <summary>
    /// 무조건 재생 (중복 허용)
    /// </summary>
    public Guid PlayForce(string key, Vector3 pos)
    {
        EventReference evt = audioLibrary.Get(key);
        if (evt.IsNull) 
        {
            Debug.LogWarning($"Audio Library에서 '{key}' 키를 찾을 수 없습니다");
            return Guid.Empty;
        }

        return InternalPlay(evt, pos);
    }

    /// <summary>
    /// 루프 오디오 재생 (풀에 넣지 않음)
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Guid PlayLooped(string key, Vector3 pos)
    {
        EventReference evt = audioLibrary.Get(key);
        if (evt.IsNull) 
        {
            Debug.LogWarning($"Audio Library에서 '{key}' 키를 찾을 수 없습니다");
            return Guid.Empty;
        }

        // 이미 재생 중인 사운드가 있는지 확인
        if (activeSounds.TryGetValue(evt, out var soundList))  
        {
            foreach (var s in soundList)
            {
                if (s.Instance.isValid())
                {
                    s.Instance.getPlaybackState(out var state);
                    if (state != PLAYBACK_STATE.STOPPED)
                        return Guid.Empty;
                }
            }
        }

        EventInstance instance = RuntimeManager.CreateInstance(evt);
        instance.start();
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(pos));
        Guid id = Guid.NewGuid();

        var tracked = new TrackedSound();
        tracked.EventRef = evt;
        tracked.Id = id;
        tracked.Instance = instance;
        tracked.MonitorCoroutine = default;

        if (!activeSounds.ContainsKey(evt))
            activeSounds[evt] = new List<TrackedSound>();
        activeSounds[evt].Add(tracked);
        guidToSound[id] = tracked;
        return id;
    }
    

    /// <summary>
    /// 소리 위치 설정
    /// </summary>
    public void SetPosition(Guid id, Vector3 pos)
    {
        if(!TryGetValidTrackedSound(id, out TrackedSound tracked)) return;
        tracked.Instance.set3DAttributes(RuntimeUtils.To3DAttributes(pos));
    }

    /// <summary>
    /// 해당 Guid 인스턴스 정지
    /// </summary>
    /// <param name="_mode">소리 끄는 모드 IMMEDIATE == 일반, ALLOWFADEOUT == 페이드 아웃</param>
    public void StopSound(Guid id, FMOD.Studio.STOP_MODE _mode)
    {
        if(!TryGetValidTrackedSound(id, out TrackedSound tracked)) return;
        EventReference evt = tracked.EventRef;
        RemoveTracked(tracked);
    }

    /// <summary>
    /// 해당하는 EventReference 참조 인스턴스 정지
    /// </summary>
    /// <param name="_mode">소리 끄는 모드 IMMEDIATE == 일반, ALLOWFADEOUT == 페이드 아웃</param>
    public void StopSound(EventReference evt, FMOD.Studio.STOP_MODE _mode)
    {
        if(!TryGetValidTrackedList(evt, out List<TrackedSound> trackedList)) return;
        for (int i = trackedList.Count - 1; i >= 0; i--)
        {
            TrackedSound tracked = trackedList[i];
            RemoveTracked(tracked);
        }
    }

    /// <summary>
    /// 모든 인스턴스 정지
    /// </summary>
    /// /// <param name="_mode">소리 끄는 모드, IMMEDIATE == 일반, ALLOWFADEOUT  == 페이드 아웃</param>
    public void StopAllSounds(FMOD.Studio.STOP_MODE _mode)
    {
        if (activeSounds.Count == 0)
        {
            Debug.Log("현재 재생 중인 사운드 이벤트가 없습니다");
            return;
        }
        var keys = activeSounds.Keys.ToList();
        for (int i = keys.Count -1; i >= 0; i--)
        {
            var evt = keys[i];
            if(!TryGetValidTrackedList(evt, out List<TrackedSound> trackedList)) continue;
            for (int j = trackedList.Count - 1; j >= 0; j--)
            {
                TrackedSound tracked = trackedList[j];
                RemoveTracked(tracked);
            }
            activeSounds[evt].Clear();
        }
    }
    
    /// <summary>
    /// 해당 Guid 인스턴스 일시정지 유무
    /// </summary>
    public void PauseSound(Guid id, bool isPause)
    {
        if(!TryGetValidTrackedSound(id, out TrackedSound tracked)) return;
        tracked.Instance.setPaused(isPause);
    }

    /// <summary>
    /// 해당하는 EventReference 참조 인스턴스 유무
    /// </summary>
    public void PauseSound(EventReference evt, bool isPause)
    {
        if (!TryGetValidTrackedList(evt, out List<TrackedSound> trackedList)) return;
        for (int i = trackedList.Count - 1; i >= 0; i--)
        {
            TrackedSound tracked = trackedList[i];
            if (!tracked.Instance.isValid()) continue;
            tracked.Instance.setPaused(isPause);
        }
    }

    /// <summary>
    /// 모든 인스턴스 일시정지 유무
    /// </summary>
    public void PauseAllSounds(bool isPause)
    {
        if (activeSounds.Count == 0)
        {
            Debug.Log("현재 재생 중인 사운드 이벤트가 없습니다");
            return;
        }

        var keys = activeSounds.Keys.ToList();
        for (int i = keys.Count -1; i >= 0; i--)
        {
            var evt = keys[i];
            if(!TryGetValidTrackedList(evt, out List<TrackedSound> trackedList)) continue;
            for (int j = trackedList.Count - 1; j >= 0; j--)
            {
                TrackedSound tracked = trackedList[j];
                if (!tracked.Instance.isValid()) continue;
                tracked.Instance.setPaused(isPause);
            }
        }
    }

    /// <summary>
    /// 해당 Guid 인스턴스 볼륨 조절
    /// </summary>
    public void VolumeControl(Guid id, float volume)
    {
        if(!TryGetValidTrackedSound(id, out TrackedSound tracked)) return;
        tracked.Instance.setVolume(volume);
    }

    /// <summary>
    /// 해당하는 EventReference 참조 인스턴스 볼륨 조절
    /// </summary>
    public void VolumeControl(EventReference evt, float volume)
    {
        if(!TryGetValidTrackedList(evt, out List<TrackedSound> trackedList)) return;
        for (int i = trackedList.Count - 1; i >= 0; i--)
        {
            TrackedSound tracked = trackedList[i];
            if (!tracked.Instance.isValid()) continue;
            tracked.Instance.setVolume(volume);
        }
    }

    /// <summary>
    /// 마스터 볼륨 % 조절
    /// </summary>
    public void AllVolumeDown(float volume)
    {   
        MasterBus.setVolume(soundSettings.MasterVolume * volume);
    }

    /// <summary>
    /// 마스터 볼륨 기존 값으로 초기화
    /// </summary>
    public void AllVoumeInit()
    {
        MasterBus.setVolume(soundSettings.MasterVolume);
    }

    /// <summary>
    /// 해당 Guid 인스턴스 볼륨 값 가져오기
    /// </summary>
    public float GetVolume(Guid id)
    {
        if(!TryGetValidTrackedSound(id, out TrackedSound tracked)) return default;
        tracked.Instance.getVolume(out float volume);
        return volume;
    }

    /// <summary>
    /// 해당하는 EventReference 참조 인스턴스 볼륨 값 가져오기
    /// </summary>
    public float[] GetVolume(EventReference evt)
    {
        if(!TryGetValidTrackedList(evt, out List<TrackedSound> trackedList)) return default;
        float[] volumes = new float[trackedList.Count];
        for (int i = trackedList.Count - 1; i >= 0; i--)
        {
            TrackedSound tracked = trackedList[i];
            if (!tracked.Instance.isValid()) continue;
            tracked.Instance.getVolume(out float volume);
            volumes[i] = volume;
        }
        return volumes;
    }

    /// <summary>
    /// 효과음 재생 중인지 체크
    /// </summary>
    public bool DuplicateCheck(Guid id)
    {
        if(!TryGetValidTrackedSound(id, out TrackedSound tracked)) return false;
        if (!activeSounds.ContainsKey(tracked.EventRef)) 
            return false;
        else return true;
    }

    public bool DuplicateCheck(string key)
    {
        EventReference evt = audioLibrary.Get(key);
        if (!activeSounds.ContainsKey(evt)) 
            return false;
        else return true;
    }

    /// <summary>
    /// 해당 Guid 인스턴스 파라미터 값 설정
    /// </summary>
    /// <param name="paramName">파라미터 이름</param>
    /// <param name="value">파라미터 값</param>
    public void SetEventParameter(Guid id, string paramName, float value)
    {
        if(!TryGetValidTrackedSound(id, out TrackedSound tracked)) return;
        tracked.Instance.setParameterByName(paramName, value);
    }

    /// <summary>
    /// 해당하는 EventReference 참조 인스턴스 파라미터 값 설정
    /// </summary>
    /// <param name="paramName">파라미터 이름</param>
    /// <param name="value">파라미터 값</param>
    public void SetEventParameter(EventReference evt, string paramName, float value)
    {
        if(!TryGetValidTrackedList(evt, out List<TrackedSound> trackedList)) return;
        for (int i = trackedList.Count - 1; i >= 0; i--)
        {
            TrackedSound tracked = trackedList[i];
            if (!tracked.Instance.isValid()) continue;
            tracked.Instance.setParameterByName(paramName, value);
        }
    }

    /// <summary>
    /// 시스템 파라미터 값 설정
    /// </summary>
    /// <param name="paramName"></param>
    /// <param name="value"></param>
    public void SetSystemParameter(string paramName, float value)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(paramName, value);
    }

    /// <summary>
    /// 해당 Guid 인스턴스 파라미터 값 가져오기
    /// </summary>
    /// <param name="paramName">파라미터 이름</param>
    public float GetParameter(Guid id, string paramName)
    {
        if(!TryGetValidTrackedSound(id, out TrackedSound tracked)) return default;
        tracked.Instance.getParameterByName(paramName, out float value);
        return value;
    }

    /// <summary>
    /// 해당 Guid 인스턴스 파라미터 값 가져오기
    /// </summary>
    /// <param name="paramName">파라미터 이름</param>
    public float[] GetParameter(EventReference evt, string paramName)
    {
        if(!TryGetValidTrackedList(evt, out List<TrackedSound> trackedList)) return default;
        float[] values = new float[trackedList.Count];
        for (int i = trackedList.Count - 1; i >= 0; i--)
        {
            TrackedSound tracked = trackedList[i];
            if (!tracked.Instance.isValid()) continue;
            tracked.Instance.getParameterByName(paramName, out float value);
            values[i] = value;
        }
        return values;
    }
    
    /// <summary>
    /// 해당 Guid 인스턴스 길이 값 가져오기
    /// </summary>
    /// <param name="_eventRef"></
    public float GetSoundLength(Guid id)
    {
        if(!TryGetValidTrackedSound(id, out TrackedSound tracked)) return default;
        tracked.Instance.getDescription(out EventDescription eventDescription);
        eventDescription.getLength(out int length);
        return length / 1000f;
    }

    /// <summary>
    /// 해당하는 EventReference 참조 인스턴스 길이 값 가져오기
    /// </summary>
    /// <param name="_eventRef"></
    public int[] GetSoundLength(EventReference evt)
    {
        if(!TryGetValidTrackedList(evt, out List<TrackedSound> trackedList)) return default;
        int[] lengths = new int[activeSounds[evt].Count];
        for (int i = trackedList.Count - 1; i >= 0; i--)
        {
            TrackedSound tracked = trackedList[i];
            if (!tracked.Instance.isValid()) continue;
            tracked.Instance.getDescription(out EventDescription eventDescription);
            eventDescription.getLength(out int length);
            lengths[i] = length;
        }
        return lengths;
    }

    /// <summary>
    /// 해당 Guid 인스턴스 재생 상태 가져오기
    /// </summary>
    /// <param name="_eventRef"></param>
    /// <returns></returns>
    public PLAYBACK_STATE GetPlaybackState(Guid id)
    {
        if(!TryGetValidTrackedSound(id, out TrackedSound tracked)) return default;
        tracked.Instance.getPlaybackState(out PLAYBACK_STATE playbackState);
        return playbackState;
    }

    /// <summary>
    /// 해당하는 EventReference 참조 인스턴스 재생 상태 가져오기
    /// </summary>
    /// <param name="_eventRef"></param>
    /// <returns></returns>
    public PLAYBACK_STATE[] GetPlaybackState(EventReference evt)
    {
        if(!TryGetValidTrackedList(evt, out List<TrackedSound> trackedList)) return default;
        PLAYBACK_STATE[] states = new PLAYBACK_STATE[activeSounds[evt].Count];
        for (int i = trackedList.Count - 1; i >= 0; i--)
        {
            TrackedSound tracked = trackedList[i];
            if (!tracked.Instance.isValid()) continue;
            tracked.Instance.getPlaybackState(out PLAYBACK_STATE playbackState);
            states[i] = playbackState;
        }
        return states;
    }

    /// <summary>
    /// 해당 Guid 인스턴스가 유효한지 확인하고, 유효한 경우 TrackedSound를 반환
    /// </summary>
    private bool TryGetValidTrackedSound(Guid id, out TrackedSound tracked)
    {
        tracked = null;
        if (!guidToSound.TryGetValue(id, out tracked))
        {
            Debug.Log($"해당 '{id}' ID를 가진 인스턴스를 찾을 수 없습니다");
            return false;
        }
        if (!tracked.Instance.isValid())
        {
            Debug.Log($"해당 '{id}' ID를 가진 인스턴스가 유효하지 않습니다");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 해당 EventReference를 참조하는 TrackedSound 리스트를 가져오고, 유효한 경우 리스트를 반환
    /// </summary>
    private bool TryGetValidTrackedList(EventReference evt, out List<TrackedSound> list)
    {
        list = null;
        if (!activeSounds.TryGetValue(evt, out list))
        {
            Debug.Log($"해당 '{evt}'를 참조한 인스턴스를 찾을 수 없습니다");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 인스턴스를 정리하고 풀에 반환, 요소가 없으면 activeSounds에서 제거
    /// </summary>
    private void RemoveTracked(TrackedSound tracked)
    {
        if (!tracked.Instance.isValid()) return;

        tracked.Instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        tracked.Instance.release();
        guidToSound.Remove(tracked.Id);

        if (activeSounds.TryGetValue(tracked.EventRef, out var list))
            list.RemoveAll(t => t.Id == tracked.Id);
        if (activeSounds[tracked.EventRef].Count == 0)
            activeSounds.Remove(tracked.EventRef);
        soundPool.Enqueue(InitializeTrackedSound(tracked));
    }

    /// <summary>
    /// TrackedSound 초기화
    /// </summary>
    public TrackedSound InitializeTrackedSound(TrackedSound tracked)
    {
        tracked.EventRef = default;
        tracked.Instance = default;
        tracked.MonitorCoroutine = null;
        tracked.Id = Guid.Empty;

        return tracked;
    }
}
