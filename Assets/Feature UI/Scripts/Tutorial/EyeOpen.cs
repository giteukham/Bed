using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeOpen : MonoBehaviour
{

    private Sequence sequence;
    private CanvasGroup canvasGroup;
    private CanvasGroup parentCanvasGroup;

    [SerializeField] private GameObject parent;
    [SerializeField] private float runTimeFadeDuration = 0.2f;  // 페이드 인/아웃 시간
    [SerializeField] private float moveDuration = 0.6f; // 이동 시간
    [SerializeField] private float moveOffset = 10f;  // 위로 올라가는 거리
    [SerializeField] private float fadeInDuration = 0.4f;
    [SerializeField] private float fadeOutDuration = 0.4f;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        parentCanvasGroup = parent.GetComponent<CanvasGroup>();
    }

    public void TutorialActivate(bool isActivate)
    {
        if (isActivate) gameObject.SetActive(true);
        else
        {
            parentCanvasGroup.DOFade(0, fadeInDuration).OnComplete(() =>
            {
                canvasGroup.alpha = 0;
                parentCanvasGroup.alpha = 0;
                gameObject.SetActive(false);
            }
            );
        }
    }

    private void OnEnable()
    {
        parent.GetComponent<CanvasGroup>().DOFade(1, fadeInDuration);

        if (sequence != null) sequence.Restart();
        else PlayEffect();
    }

    private void PlayEffect()
    {
        sequence = DOTween.Sequence();

        //서서히 나타남
        sequence.Append(canvasGroup.DOFade(1, runTimeFadeDuration));

        //살짝 위로 이동 (RectTransform의 anchoredPosition 사용)
        sequence.Append(GetComponent<RectTransform>().DOAnchorPosY(GetComponent<RectTransform>().anchoredPosition.y + moveOffset, moveDuration));

        //서서히 사라짐
        sequence.Append(canvasGroup.DOFade(0, runTimeFadeDuration));

        //휠 위치 빠르게 원위치
        //sequence.Append(GetComponent<RectTransform>().DOAnchorPosY(GetComponent<RectTransform>().anchoredPosition.y, 0.01f));
        sequence.OnComplete(() => GetComponent<RectTransform>().anchoredPosition = Vector2.zero);

        //무한 반복 설정
        sequence.SetLoops(-1);
    }
}
