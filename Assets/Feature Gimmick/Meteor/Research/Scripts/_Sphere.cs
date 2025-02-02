using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class _Sphere : MonoBehaviour
{
    private GameObject sphere;
    
    private Rigidbody rb;
    private Transform tr;
    
    private FractureInPlayMode _fractureInPlayMode;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        _fractureInPlayMode = GetComponent<FractureInPlayMode>();
        //fracture.GenerateChunkInPlayMode( true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //rb.AddForce(new Vector3(100f, 0f, 0f), ForceMode.Impulse);
            //tr.position += new Vector3(100f, 0f, 0f) * Time.deltaTime;
            transform.DOMoveX(10f, 1f);
        }
    }

    private void OnDrawGizmos()
    {
        if (Physics.Raycast(transform.position, Vector3.forward, out var hit))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector3.forward * hit.distance);
            Debug.Log(hit.collider.gameObject.name);
        }
    }
}
