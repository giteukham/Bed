using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Windows.Forms;

public class CockroachForTutorial : MonoBehaviour
{
    private Animator animator;
    private bool isMoving = false;
    private Coroutine randomMoveCoroutine;
    
    public float minMoveDistance;
    public float maxMoveDistance;
    public float minMoveDuration;
    public float maxMoveDuration;
    public float minRotateAngle;
    public float maxRotateAngle;
    public float minRotateDuration;
    public float maxRotateDuration; 
    public float minWaitTime;
    public float maxWaitTime ;
    public float minX, maxX, minY, maxY;
    [SerializeField]private Player player;

    [SerializeField]private Transform leftTop, rightTop, leftMid, rightMid, leftBottom, rightBottom;
    private Vector3 initPosition;
    private int prevEyeBlinkCAT, prevEyeBlinkLAT;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        initPosition = transform.localPosition;
    }

    public void Exit()
    {
        if (randomMoveCoroutine != null) StopCoroutine(randomMoveCoroutine);
        Vector3 moveDirection = transform.localRotation * -Vector3.forward;  
        Vector3 targetPosition = transform.localPosition + moveDirection * 5f;
        targetPosition.z = 0.024f;
        if (animator != null) animator.speed = 1.8f;
        else animator = GetComponent<Animator>();
        TutorialManager.Instance.isBlinkTutorialActivate = true;
        transform.DOLocalMove(targetPosition, 3f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                transform.localPosition = initPosition;
                gameObject.SetActive(false);
            });
    }

    public void OnEnable()
    {
        PlayerConstant.isParalysis = true;
        Vector3 moveDirection = transform.localRotation * -Vector3.forward;  
        float moveDistance = Random.Range(minMoveDistance, maxMoveDistance);
        float moveDuration = Random.Range(minMoveDuration, maxMoveDuration);

        Vector3 targetPosition = transform.localPosition + moveDirection * moveDistance;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        targetPosition.z = 0.024f;

        transform.DOLocalMove(targetPosition, 1f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                isMoving = false;
                animator.speed = 0;
                StartCoroutine(MainCode());
            });
    }

    private IEnumerator MainCode()
    {
        yield return new WaitForSeconds(1f);
        PlayerConstant.isParalysis = false;
        randomMoveCoroutine = StartCoroutine(RandomMove());
        prevEyeBlinkCAT = PlayerConstant.EyeBlinkCAT;
        prevEyeBlinkLAT = PlayerConstant.EyeBlinkLAT;
        while(true)
        {
            if (PlayerConstant.EyeBlinkCAT > prevEyeBlinkCAT || PlayerConstant.EyeBlinkLAT > prevEyeBlinkLAT || PlayerConstant.isMovingState || PlayerConstant.headMoveSpeed >= 13f) 
            {
                Exit();
                yield break;
            }
            prevEyeBlinkCAT = PlayerConstant.EyeBlinkCAT;
            prevEyeBlinkLAT = PlayerConstant.EyeBlinkLAT;
            yield return null;
        }
    }

    private IEnumerator RandomMove()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            RandomRotation();
        }
    }

    private void RandomRotation()
    {
        if (isMoving) return; 

        isMoving = true;
        animator.speed = 1.2f;

        float rotateAngle = Random.Range(minRotateAngle, maxRotateAngle);
        float rotateDuration = Random.Range(minRotateDuration, maxRotateDuration);

        Quaternion rotationDelta = Quaternion.Euler(0, rotateAngle, 0);
        Quaternion targetRotation = transform.localRotation * rotationDelta;

        transform.DOLocalRotateQuaternion(targetRotation, rotateDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                MoveAfterRotation();
            });
    }

    private void MoveAfterRotation()
    {
        Vector3 moveDirection = transform.localRotation * -Vector3.forward;  
        float moveDistance = Random.Range(minMoveDistance, maxMoveDistance);
        float moveDuration = Random.Range(minMoveDuration, maxMoveDuration);

        Vector3 targetPosition = transform.localPosition + moveDirection * moveDistance;
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        targetPosition.z = 0.024f;

        transform.DOLocalMove(targetPosition, moveDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                isMoving = false;
                animator.speed = 0;
            });
    }

    private void LeftTopScuttling()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.Cockroach, leftTop.position);
    }

    private void RightTopScuttling()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.Cockroach, rightTop.position);
    }

    private void LeftMidScuttling()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.Cockroach, leftMid.position);
    }

    private void RightMidScuttling()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.Cockroach, rightMid.position);
    }

    private void LeftBottomScuttling()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.Cockroach, leftBottom.position);
    }

    private void RightBottomScuttling()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.Cockroach, rightBottom.position);
    }
}
