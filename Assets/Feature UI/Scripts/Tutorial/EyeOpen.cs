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
    [SerializeField] private float runTimeFadeDuration = 0.2f;  // ���̵� ��/�ƿ� �ð�
    [SerializeField] private float moveDuration = 0.6f; // �̵� �ð�
    [SerializeField] private float moveOffset = 10f;  // ���� �ö󰡴� �Ÿ�
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

        //������ ��Ÿ��
        sequence.Append(canvasGroup.DOFade(1, runTimeFadeDuration));

        //��¦ ���� �̵� (RectTransform�� anchoredPosition ���)
        sequence.Append(GetComponent<RectTransform>().DOAnchorPosY(GetComponent<RectTransform>().anchoredPosition.y + moveOffset, moveDuration));

        //������ �����
        sequence.Append(canvasGroup.DOFade(0, runTimeFadeDuration));

        //�� ��ġ ������ ����ġ
        //sequence.Append(GetComponent<RectTransform>().DOAnchorPosY(GetComponent<RectTransform>().anchoredPosition.y, 0.01f));
        sequence.OnComplete(() => GetComponent<RectTransform>().anchoredPosition = Vector2.zero);

        //���� �ݺ� ����
        sequence.SetLoops(-1);
    }
}
