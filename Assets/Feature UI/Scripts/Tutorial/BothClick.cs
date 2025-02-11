using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BothClick : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private float fadeInDuration = 0.1f;  // ���̵� ��
    private float fadeOutDuration = 0.3f;  // ���̵� �ƿ�

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        //ó���� �Ⱥ��̰�
        canvasGroup.DOFade(0, 0.01f);

        PlayEffect();
    }

    private void PlayEffect()
    {
        Sequence sequence = DOTween.Sequence();

        //������ ��Ÿ��
        sequence.Append(canvasGroup.DOFade(1, fadeInDuration));

        //������ �����
        sequence.Append(canvasGroup.DOFade(0, fadeOutDuration));

        //���� �ݺ� ����
        sequence.SetLoops(-1);
    }
}
