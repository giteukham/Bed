using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TestController : MonoBehaviour
{
    public RectTransform topEyelid, bottomEyelid;
    private const int SCROLL_SPEED = 1800;
    private const float SCROLL_THRESHOLD = 0.1f;
    private Vector2 topEyelidPosition, bottomEyelidPosition;
    private bool isBlink = false;
    public bool isEyeOpen = true;
    private float mouseDirectionValue;

    private readonly float TOP_MAX_Y = 1080f;
    private readonly float TOP_MIN_Y = 180f;
    private readonly float BOTTOM_MAX_Y = -1080f;
    private readonly float BOTTOM_MIN_Y = -180f;
    
    public PostProcessVolume postProcessVolume;
    private Vignette vignette;
    private DepthOfField depthOfField;

    private Vector3 lastMousePosition;
    private float mHorizontalSpeed, mVerticalSpeed;
    public Animator animator;
    
    public bool isMoving = false;

    public enum State {
        Middle,
        Left,
        Right
    }
    public State currentState;

    void Start() 
    {
        topEyelidPosition = topEyelid.offsetMin;
        bottomEyelidPosition = bottomEyelid.offsetMax;

        postProcessVolume.profile.TryGetSettings(out vignette);
        postProcessVolume.profile.TryGetSettings(out depthOfField);

        lastMousePosition = Input.mousePosition;

        currentState = State.Middle;
    }

    void Update()
    {
        CheckMovement();
        AdjustMousePosition();
        AdjustEyelids();
        AdjustPostProcessVolume();
        AdjustHeadTurn();
    }

    void AdjustEyelids() 
    {
        float scrollInput = Mathf.Clamp(Input.GetAxis("Mouse ScrollWheel"), -SCROLL_THRESHOLD, SCROLL_THRESHOLD);
 
 
        isEyeOpen = !(topEyelidPosition.y <= TOP_MIN_Y && bottomEyelidPosition.y >= BOTTOM_MIN_Y);

        if (Input.GetMouseButtonDown(2) && !isBlink)
        {
            StartCoroutine(BlinkCoroutine());
            return;
        }

        if(scrollInput == 0 || isBlink) return;

        if ((topEyelidPosition.y >= TOP_MAX_Y && bottomEyelidPosition.y <= BOTTOM_MAX_Y && scrollInput > 0) ||
            (topEyelidPosition.y <= TOP_MIN_Y && bottomEyelidPosition.y >= BOTTOM_MIN_Y && scrollInput < 0)) 
            scrollInput = 0;

        topEyelidPosition.y += scrollInput * SCROLL_SPEED;
        bottomEyelidPosition.y -= scrollInput * SCROLL_SPEED;

        topEyelid.offsetMin = topEyelidPosition;
        bottomEyelid.offsetMax = bottomEyelidPosition;
    }

     IEnumerator BlinkCoroutine()
    {
        isBlink = true;
        float blinkSpeed = 300f; 
        Vector2 originalTopPosition = topEyelidPosition;
        Vector2 originalBottomPosition = bottomEyelidPosition;
        
        while (topEyelidPosition.y > TOP_MIN_Y && bottomEyelidPosition.y < BOTTOM_MIN_Y)
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

    void AdjustPostProcessVolume()
    {
        if (vignette != null && depthOfField != null)
        {
            float topLidNormalized = Mathf.InverseLerp(TOP_MIN_Y, TOP_MAX_Y, topEyelidPosition.y);
            float bottomLidNormalized = Mathf.InverseLerp(BOTTOM_MIN_Y, BOTTOM_MAX_Y, bottomEyelidPosition.y);
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
                case State.Middle:
                    if(mouseDirectionValue < -30) ChangeAnimation(State.Left);
                    if(mouseDirectionValue > 30) ChangeAnimation(State.Right);
                    break;
                case State.Right: 
                    if(mouseDirectionValue < -30) ChangeAnimation(State.Middle);
                    break;
                case State.Left: 
                    if(mouseDirectionValue > 30) ChangeAnimation(State.Middle);
                    break;
            }
        }
    }

    void ChangeAnimation(State _changeState) {
        switch(_changeState) 
        {
            case State.Middle: {
                if(currentState == State.Left) 
                    animator.Play("LeftToMiddle");
                if(currentState == State.Right) 
                    animator.Play("RightToMiddle");
                break;
            }
            case State.Right: {
                animator.Play("MddileToRight");
                break;
            }
            case State.Left: {
                animator.Play("MiddleToLeft");
                break;
            }
        }
        currentState = _changeState;
    }

    void CheckMovement() {
        AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        isMoving =  currentAnimatorStateInfo.IsName("LeftToMiddle") ||
                    currentAnimatorStateInfo.IsName("RightToMiddle") ||
                    currentAnimatorStateInfo.IsName("MddileToRight") ||
                    currentAnimatorStateInfo.IsName("MiddleToLeft") &&
                    currentAnimatorStateInfo.normalizedTime < 0.8f;
    }
}
