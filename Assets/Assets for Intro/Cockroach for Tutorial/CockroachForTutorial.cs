using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    private Vector3 initPosition;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        initPosition = transform.localPosition;
    }

    public void Exit()
    {
        StopCoroutine(randomMoveCoroutine);

        Vector3 moveDirection = -transform.forward;
        Vector3 targetPosition = transform.localPosition + moveDirection * 1f;
        transform.DOLocalMove(targetPosition, 0.8f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                isMoving = false;
                transform.localPosition = initPosition;
                gameObject.SetActive(false);
            });
    }

    public void OnEnable()
    {
        Vector3 moveDirection = transform.forward;  
        float moveDistance = Random.Range(minMoveDistance, maxMoveDistance);
        float moveDuration = Random.Range(minMoveDuration, maxMoveDuration);

        Vector3 targetPosition = transform.localPosition + moveDirection * moveDistance;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        targetPosition.z = 0.024f;

        transform.DOLocalMove(targetPosition, 0.2f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                isMoving = false;
                animator.Play("Idle");
                randomMoveCoroutine = StartCoroutine(RandomMoveLoop());
            });
    }

    private IEnumerator RandomMoveLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            MoveRandom();
        }
    }

    private void MoveRandom()
    {
        if (isMoving) return; 

        isMoving = true;
        animator.Play("Move");

        float rotateAngle = Random.Range(minRotateAngle, maxRotateAngle);
        float rotateDuration = Random.Range(minRotateDuration, maxRotateDuration);

        Quaternion rotationDelta = Quaternion.Euler(0, rotateAngle, 0);
        Quaternion targetRotation = transform.localRotation * rotationDelta;

        transform.DOLocalRotateQuaternion(targetRotation, rotateDuration)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                MoveAfterRotation();
            });
    }

    private void MoveAfterRotation()
    {
        Vector3 moveDirection = -transform.forward;  
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
                animator.Play("Idle");
            });
    }
}
