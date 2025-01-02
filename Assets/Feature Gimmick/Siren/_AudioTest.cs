using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _AudioTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.siren, transform.position);
    }
}
