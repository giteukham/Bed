using System.Collections;
using FMODUnity;
using UnityEngine;

public class SoundPlayPrefab : MonoBehaviour
{
    [HideInInspector]public EventReference soundReference;

    void Awake()
    {
        AudioManager.instance.PlaySound(soundReference, transform.position);
    }

    void Update()
    {
        transform.position += 11 * Time.deltaTime * Vector3.down;
        AudioManager.instance.UpdateSoundPosition(soundReference, transform.position);
    }

    void OnDestroy()
    {
        AudioManager.instance.StopSound(soundReference, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
