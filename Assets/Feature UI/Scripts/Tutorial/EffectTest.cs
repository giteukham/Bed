using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTest : MonoBehaviour
{
    public GameObject effect;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (effect.activeSelf == true)
            {
                effect.GetComponent<ITutorialEffect>().OffEffect();
            }
            else
            {
                effect.SetActive(true);
            }
        }
    }
}
