using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour, ITutorialEffect
{
    private Sequence sequence;
    private CanvasGroup canvasGroup;
    private CanvasGroup parentCanvasGroup;

    [SerializeField] private GameObject parent;
    [SerializeField] private float runTimeFadeInDuration = 0.1f;  // 페이드 인
    [SerializeField] private float runTimeFadeOutDuration = 0.3f;  // 페이드 아웃
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
        sequence.Append(canvasGroup.DOFade(1, runTimeFadeInDuration));

        //서서히 사라짐
        sequence.Append(canvasGroup.DOFade(0, runTimeFadeOutDuration));

        //무한 반복 설정
        sequence.SetLoops(-1);
    }
}
