using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;

[CreateAssetMenu(menuName = "Audio/Library")]
public class AudioLibrary : ScriptableObject
{
    [field: SerializeField] public List<AudioEntrySO> Entries { get; private set; }

    private Dictionary<string, EventReference> _map;

    public void Initialize()
    {
        _map = Entries.ToDictionary(e => e.Key, e => e.Event);
    }

    public EventReference Get(string key)   // 키로 이벤트 가져오기
    {
        if (_map == null) Initialize();
        return _map.TryGetValue(key, out var result) ? result : default;
    }

#if UNITY_EDITOR
    public List<string> GetAllKeys()
    {
        return Entries.Select(e => e.Key).ToList();
    }
#endif
}
