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
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
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
}
