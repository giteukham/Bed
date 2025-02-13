using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BothClick : Tutorial
{
    [SerializeField] private float runTimeFadeInDuration = 0.1f;  // ���̵� ��
    [SerializeField] private float runTimeFadeOutDuration = 0.3f;  // ���̵� �ƿ�

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        parentCanvasGroup = parent.GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        parent.GetComponent<CanvasGroup>().DOFade(1, fadeInDuration);

        if (sequenceOwn != null && sequenceShake != null)
        {
            sequenceOwn.Restart();
            sequenceShake.Restart();
        }
        else PlayEffect();
    }

    private void PlayEffect()
    {
        sequenceShake = DOTween.Sequence();
        sequenceShake.Append(Shake());

        sequenceOwn = DOTween.Sequence();

        //������ ��Ÿ��
        sequenceOwn.Append(canvasGroup.DOFade(1, runTimeFadeInDuration));

        //������ �����
        sequenceOwn.Append(canvasGroup.DOFade(0, runTimeFadeOutDuration));

        //���� �ݺ� ����
        sequenceOwn.SetLoops(-1);
    }
}
