using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MouseParticle : MonoBehaviour
{
    [SerializeField]
    private _Particle _particle;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            _particle.transform.position = new Vector3(hit.point.x, hit.point.y + 0.5f, hit.point.z);
        }
    }
}
