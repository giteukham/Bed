using System;
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
    private float mouseDirectionValue;

    private readonly float TopMaxY = 1080f;
    private readonly float TopMinY = 180f;
    private readonly float BottomMaxY = -1080f;
    private readonly float BottomMinY = -180f;
    
    public PostProcessVolume postProcessVolume;
    private Vignette vignette;
    private DepthOfField depthOfField;

    private Vector3 lastMousePosition;
    private float mHorizontalSpeed, mVerticalSpeed;
    public Animator animator;
    
    public bool isMoving = false;

    public enum State {
        isMiddle,
        isLeft,
        isRight
    }
    public State currentState;

    void Start() 
    {
        topEyelidPosition = topEyelid.offsetMin;
        bottomEyelidPosition = bottomEyelid.offsetMax;

        postProcessVolume.profile.TryGetSettings(out vignette);
        postProcessVolume.profile.TryGetSettings(out depthOfField);

        lastMousePosition = Input.mousePosition;

        currentState = State.isMiddle;
    }

    void Update()
    {
        IsMovingCheck();
        AdjustMousePosition();
        AdjustEyelids();
        AdjustPostProcessVolume();
        AdjustHeadTurn();
    }

    void AdjustEyelids() 
    {
        float scrollInput = Mathf.Clamp(Input.GetAxis("Mouse ScrollWheel"), -scrollThreshold, scrollThreshold);
 
        if(topEyelidPosition.y <= TopMinY && bottomEyelidPosition.y >= BottomMinY) isEyeOpen = false;
        else isEyeOpen = true;

        if (Input.GetMouseButtonDown(2) && !isBlink)
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
        mouseDirectionValue = currentMousePosition.x - lastMousePosition.x;

        mHorizontalSpeed = (currentMousePosition.x - lastMousePosition.x) / Time.deltaTime;
        mVerticalSpeed = (currentMousePosition.y - lastMousePosition.y) / Time.deltaTime;
        lastMousePosition = currentMousePosition;

        Vector3 currentRotation = gameObject.transform.localEulerAngles;
        float currentRotationX = (currentRotation.x >= 180) ? currentRotation.x - 360 : currentRotation.x;
        float currentRotationY = (currentRotation.y >= 180) ? currentRotation.y - 360 : currentRotation.y;

        float newXRotation = currentRotationX + mVerticalSpeed * 0.0003f;
        float newYRotation = currentRotationY + mHorizontalSpeed * 0.0003f;

        newXRotation = Mathf.Clamp(newXRotation, -10f, 10f);
        newYRotation = Mathf.Clamp(newYRotation, -30f, 30f);

        float maxZRotation = 10f;
        float newZRotation = maxZRotation * Mathf.Sin(Mathf.Deg2Rad * newYRotation);

        gameObject.transform.localEulerAngles = new Vector3(newXRotation, newYRotation, newZRotation); 
    }


    void AdjustHeadTurn() {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if(stateInfo.normalizedTime < 0.8f) return;

        if (Math.Abs(mHorizontalSpeed) > 5500f) { // 이건 마우스 속도
            switch(currentState) 
            {
                case State.isMiddle: {
                    if(mouseDirectionValue < -30)
                    {
                        Debug.Log("중간에서 왼쪽");
                        ChangeAnimation(State.isLeft);
                    }

                    if(mouseDirectionValue > 30) 
                    {
                        Debug.Log("중간에서 오른쪽");
                        ChangeAnimation(State.isRight);
                    }   
                    break;
                }
                case State.isRight: {
                    if(mouseDirectionValue < -30) 
                    {
                        Debug.Log("오른쪽에서 중간");
                        ChangeAnimation(State.isMiddle);
                    }
                    break;
                }
                case State.isLeft: {
                    if(mouseDirectionValue > 30) 
                    {
                        Debug.Log("왼쪽에서 중간");
                        ChangeAnimation(State.isMiddle);
                    }
                    break;
                }
            }
        }
    }

    void ChangeAnimation(State _changeState) {
        switch(_changeState) 
        {
            case State.isMiddle: {
                if(currentState == State.isLeft) 
                    animator.Play("LeftToMiddle");
                if(currentState == State.isRight) 
                    animator.Play("RightToMiddle");
                break;
            }
            case State.isRight: {
                animator.Play("MddileToRight");
                break;
            }
            case State.isLeft: {
                animator.Play("MiddleToLeft");
                break;
            }
        }
        currentState = _changeState;
    }

    void IsMovingCheck() {
        AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimatorStateInfo.IsName("LeftToMiddle") ||
            currentAnimatorStateInfo.IsName("RightToMiddle") ||
            currentAnimatorStateInfo.IsName("MddileToRight") ||
            currentAnimatorStateInfo.IsName("MiddleToLeft") &&
            currentAnimatorStateInfo.normalizedTime < 0.8f )
        {
            isMoving = true;
        }
        else isMoving = false;
    }
}
