using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    [SerializeField]private Transform playerCamera;
    [SerializeField]private Transform mirrorCamera;
    
    private float playerCameraMinY = 56f;
    private float playerCameraMaxY = 140f;
    private float playerCameraMinX = -30f;
    private float playerCameraMaxX = 30f;
    private float mirrorCameraMinY = 257f;
    private float mirrorCameraMaxY = 227f;
    private float mirrorCameraMinX = 341f;
    private float mirrorCameraMaxX = 330f;
    
    private float playerX, playerY;
 
    private void Update()
    {
        if (playerCamera.rotation.eulerAngles.y > 140) return;
        CanculateMirrorCamera();
    }

    private void CanculateMirrorCamera()
    {
        if (playerCamera.eulerAngles.x > 325) playerX = 360 - playerCamera.eulerAngles.x;
        else if(playerCamera.eulerAngles.x < 330 && playerCamera.eulerAngles.x > 0) playerX = -playerCamera.eulerAngles.x;
        float xT = Mathf.InverseLerp(playerCameraMinX, playerCameraMaxX, playerX);
        float mirrorX = Mathf.Lerp(mirrorCameraMaxX, mirrorCameraMinX, xT);

        playerY = playerCamera.rotation.eulerAngles.y;
        float yT = Mathf.InverseLerp(playerCameraMinY, playerCameraMaxY, playerY);
        float mirrorY = Mathf.Lerp(mirrorCameraMaxY, mirrorCameraMinY, yT);

        mirrorCamera.transform.rotation = Quaternion.Euler(mirrorX, mirrorY, mirrorCamera.rotation.eulerAngles.z);
    }
}
