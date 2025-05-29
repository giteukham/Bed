using FMODUnity;
using UnityEngine;

public enum AudioCategory 
{
    Player,
    Gimmick
}

[CreateAssetMenu(menuName = "Audio/Entry")]
public class AudioEntrySO : ScriptableObject
{
    [field: SerializeField] public string Key { get; set; } // 키
    [field: SerializeField] public EventReference Event { get; private set; } // FMOD 이벤트
    [field: SerializeField] public AudioCategory Category { get; private set; } //카테고리
}
