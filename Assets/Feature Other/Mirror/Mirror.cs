using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    [SerializeField]private Transform playerCamera;
    private Vector3 mirrorCameraIni;// = new Quaternion.Euler(new Vector(0, 180, 0));;

    private void Start()
    {
        mirrorCameraIni = transform.rotation.eulerAngles;
    }
 
    private void Update()
    {
        Debug.Log(InputSystem.MouseDeltaX + ", " + InputSystem.MouseDeltaY);
        //if(playerCamera.rotation.eulerAngles.y < 0) return;
    }
}
