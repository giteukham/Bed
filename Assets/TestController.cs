using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TestController : MonoBehaviour
{
    public RectTransform topEyelid, bottomEyelid;
    private int scrollSpeed = 1800;
    private const float scrollThreshold = 0.1f;
    private Vector2 topEyelidPosition, bottomEyelidPosition;
    private bool isBlink = false;
    public bool isEyeOpen = true;

    private readonly float TopMaxY = 1080f;
    private readonly float TopMinY = 180f;
    private readonly float BottomMaxY = -1080f;
    private readonly float BottomMinY = -180f;
    
    public PostProcessVolume postProcessVolume;
    private Vignette vignette;
    private DepthOfField depthOfField;

    private Vector3 lastMousePosition;
    private float mHorizontalSpeed, mVerticalSpeed;


    void Start() 
    {
        topEyelidPosition = topEyelid.offsetMin;
        bottomEyelidPosition = bottomEyelid.offsetMax;

        postProcessVolume.profile.TryGetSettings(out vignette);
        postProcessVolume.profile.TryGetSettings(out depthOfField);

        lastMousePosition = Input.mousePosition;
    }

    void Update()
    {
        AdjustMousePosition();
        AdjustEyelids();
        AdjustPostProcessVolume();
    }

    void AdjustEyelids() 
    {
        float scrollInput = Mathf.Clamp(Input.GetAxis("Mouse ScrollWheel"), -scrollThreshold, scrollThreshold);
 
        if(topEyelidPosition.y <= TopMinY && bottomEyelidPosition.y >= BottomMinY) isEyeOpen = false;
        else isEyeOpen = true;

        if (Input.GetMouseButtonDown(2) && !isBlink) // 마우스 휠
        {
            StartCoroutine(BlinkCoroutine());
            return;
        }

        if(scrollInput == 0 || isBlink) return;

        if(topEyelidPosition.y >= TopMaxY && bottomEyelidPosition.y <= BottomMaxY && scrollInput > 0) scrollInput = 0;
        if(topEyelidPosition.y <= TopMinY && bottomEyelidPosition.y >= BottomMinY && scrollInput < 0) scrollInput = 0; 

        topEyelidPosition.y += scrollInput * scrollSpeed;
        bottomEyelidPosition.y -= scrollInput * scrollSpeed;

        topEyelid.offsetMin = topEyelidPosition;
        bottomEyelid.offsetMax = bottomEyelidPosition;

        IEnumerator BlinkCoroutine()
        {
            isBlink = true;
            float blinkSpeed = 300f; 
            Vector2 originalTopPosition = topEyelidPosition;
            Vector2 originalBottomPosition = bottomEyelidPosition;
            
            while (topEyelidPosition.y > TopMinY && bottomEyelidPosition.y < BottomMinY)
            {
                topEyelidPosition.y -= blinkSpeed;
                bottomEyelidPosition.y += blinkSpeed;
                
                topEyelid.offsetMin = topEyelidPosition;
                bottomEyelid.offsetMax = bottomEyelidPosition;

                yield return new WaitForSeconds(0.02f);
            }

            yield return new WaitForSeconds(0.15f);

            while (topEyelidPosition.y < originalTopPosition.y && bottomEyelidPosition.y > originalBottomPosition.y)
            {
                topEyelidPosition.y += blinkSpeed;
                bottomEyelidPosition.y -= blinkSpeed;

                topEyelid.offsetMin = topEyelidPosition;
                bottomEyelid.offsetMax = bottomEyelidPosition;

                yield return new WaitForSeconds(0.02f);
            }
            isBlink = false;
        }
    }

    void AdjustPostProcessVolume()
    {
        if (vignette != null && depthOfField != null)
        {
            float topLidNormalized = Mathf.InverseLerp(TopMinY, TopMaxY, topEyelidPosition.y);
            float bottomLidNormalized = Mathf.InverseLerp(BottomMinY, BottomMaxY, bottomEyelidPosition.y);
            float eyelidClosure = 1.0f - Mathf.Min(topLidNormalized, bottomLidNormalized);

            vignette.intensity.value = Mathf.Lerp(0.19f, 1.0f, eyelidClosure);
            depthOfField.focalLength.value = Mathf.Lerp(30f, 50f, eyelidClosure);
        }
    }

    void AdjustMousePosition() 
    {
        Vector3 currentMousePosition = Input.mousePosition;

        if(currentMousePosition == null) return;

        mHorizontalSpeed = (currentMousePosition.x - lastMousePosition.x) / Time.deltaTime;
        mVerticalSpeed = (currentMousePosition.y - lastMousePosition.y) / Time.deltaTime;
        lastMousePosition = currentMousePosition;

        Vector3 currentRotation = gameObject.transform.eulerAngles;

        float newXRotation = currentRotation.x + mVerticalSpeed * 0.0003f;
        float newYRotation = currentRotation.y + mHorizontalSpeed * 0.0003f;

        newXRotation = (newXRotation > 180) ? newXRotation - 360 : newXRotation;
        newYRotation = (newYRotation > 180) ? newYRotation - 360 : newYRotation;

        newXRotation = Mathf.Clamp(newXRotation, -13f, 23f);
        newYRotation = Mathf.Clamp(newYRotation, -55f, 55f);

        float maxZRotation = 10f;
        float newZRotation = maxZRotation * Mathf.Sin(Mathf.Deg2Rad * newYRotation);

        gameObject.transform.eulerAngles = new Vector3(newXRotation, newYRotation, newZRotation); 
    }
}
