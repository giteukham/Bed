using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "SoundEvent", menuName = "Sound Event")]
public class SoundEvent : ScriptableObject
{
    public EventReference eventReference;

    public void Play(Vector3 position)
    {
        RuntimeManager.PlayOneShot(eventReference, position);
    }
}