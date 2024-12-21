using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathSound : MonoBehaviour
{
    public void InhaleSound()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.inhale, transform.position);
    }

    public void ExhaleSound()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.exhale, transform.position);
    }
}