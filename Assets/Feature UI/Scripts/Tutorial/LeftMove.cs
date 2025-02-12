using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftMove : MonoBehaviour, ITutorialEffect
{
    private Sequence sequence;
    private CanvasGroup canvasGroup;
    private CanvasGroup parentCanvasGroup;

    [SerializeField] private GameObject parent;
    [SerializeField] private float runTimeFadeDuration = 0.2f;  // 페이드 인/아웃 시간
    [SerializeField] private float moveDuration = 0.7f; // 이동 시간
    [SerializeField] private float moveOffset = -300f;  // 왼쪽으로 가는 거리
    [SerializeField] private float fadeInDuration = 0.4f;
    [SerializeField] private float fadeOutDuration = 0.4f;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        parentCanvasGroup = parent.GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        //모든 이펙트 투명화로 시작
        canvasGroup.alpha = 0;
        parentCanvasGroup.alpha = 0;

        //이펙트 전체가 천천히 나타남
        parent.GetComponent<CanvasGroup>().DOFade(1, fadeInDuration);

        if (sequence != null)
        {
            //시퀀스 재시작
            sequence.Restart();
        }
        else
        {
            //이펙트 시퀀스에 등록
            PlayEffect();
        }
    }

    public void OffEffect()
    {
        //이펙트 전체가 천천히 사라짐
        parentCanvasGroup.DOFade(0, fadeInDuration).OnComplete(() =>
        {
            //이펙트 일시정지
            sequence.Pause();
            //이펙트 비활성화
            gameObject.SetActive(false);
        }
        );
    }

    private void PlayEffect()
    {
        sequence = DOTween.Sequence();

        //서서히 나타남
        sequence.Append(canvasGroup.DOFade(1, runTimeFadeDuration));

        //목표로 이동 (RectTransform의 anchoredPosition 사용)
        sequence.Append(GetComponent<RectTransform>().DOAnchorPosX(GetComponent<RectTransform>().anchoredPosition.x + moveOffset, moveDuration));

        //서서히 사라짐
        sequence.Append(canvasGroup.DOFade(0, runTimeFadeDuration));

        //마우스 위치 빠르게 원위치
        sequence.OnComplete(() => GetComponent<RectTransform>().anchoredPosition = Vector2.zero);

        //무한 반복 설정
        sequence.SetLoops(-1);
    }
}
