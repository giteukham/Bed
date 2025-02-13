using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Tutorial : MonoBehaviour
{
    protected Sequence sequenceOwn;
    protected Sequence sequenceShake;
    protected CanvasGroup canvasGroup;
    protected CanvasGroup parentCanvasGroup;

    [SerializeField] protected GameObject parent;
    [SerializeField] protected float fadeInDuration = 0.4f;
    [SerializeField] protected float fadeOutDuration = 0.4f;

    public void TutorialActivate(bool isActivate)
    {
        if (isActivate)
        {
            parent.SetActive(true);
        }
        else
        {
            parentCanvasGroup.DOFade(0, fadeInDuration).OnComplete(() =>
            {
                sequenceOwn.Pause();
                sequenceShake.Pause();
                canvasGroup.alpha = 0;
                parentCanvasGroup.alpha = 0;
                parent.SetActive(false);
            }
            );
        }
    }

    public Tween Shake()
    {
        return parent.transform.DOShakePosition(
            duration: 2f,
            strength: new Vector3(0.7f, 0.7f, 0.7f),
            vibrato: 30,
            randomness: 90,
            fadeOut: false).SetEase(Ease.InQuart).SetLoops(-1, LoopType.Yoyo);
    }
}
