using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAudioTest : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(UpdateAudio());
    }

    private IEnumerator UpdateAudio()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.catMeow, this.transform.position);
        }
    }
}
