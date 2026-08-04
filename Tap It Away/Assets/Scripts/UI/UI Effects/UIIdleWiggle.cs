using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIIdleWiggle : UIEffect
{
    [SerializeField, Min(0f)] private float interval = 2f;
    [SerializeField, Min(0.01f)] private float wiggleDuration = 0.45f;
    [SerializeField, Min(0f)] private float angle = 8f;
    [SerializeField, Min(1)] private int wiggleCount = 3;
    [SerializeField, Min(1f)] private float scaleMultiplier = 1.08f;

    private RectTransform rectTransform;
    private Vector3 originalEulerAngles;
    private Vector3 originalScale;
    private Sequence wiggleSequence;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalEulerAngles = rectTransform.localEulerAngles;
        originalScale = rectTransform.localScale;
    }

    public override void ShowEffect(float showDelayTime)
    {
        base.ShowEffect(showDelayTime);
        StartWiggle(showDelayTime);
    }

    public override void HideEffect(float hideDelayTime)
    {
        base.HideEffect(hideDelayTime);
        StopWiggle();
    }

    public void StartWiggle(float delayTime = 0f)
    {
        StopWiggle();

        float stepDuration = wiggleDuration / Mathf.Max(1, wiggleCount * 2 + 2);
        wiggleSequence = DOTween.Sequence().SetLoops(-1, LoopType.Restart).SetDelay(delayTime).SetUpdate(deltaTimeIndependent);
        wiggleSequence.Append(rectTransform.DOScale(originalScale * scaleMultiplier, stepDuration).SetEase(Ease.OutSine));

        for (int i = 0; i < wiggleCount; i++)
        {
            wiggleSequence.Append(rectTransform.DOLocalRotate(originalEulerAngles + new Vector3(0f, 0f, angle), stepDuration).SetEase(Ease.InOutSine));
            wiggleSequence.Append(rectTransform.DOLocalRotate(originalEulerAngles + new Vector3(0f, 0f, -angle), stepDuration).SetEase(Ease.InOutSine));
        }

        wiggleSequence.Append(rectTransform.DOLocalRotate(originalEulerAngles, stepDuration).SetEase(Ease.OutSine));
        wiggleSequence.Join(rectTransform.DOScale(originalScale, stepDuration).SetEase(Ease.OutSine));
        wiggleSequence.AppendInterval(interval);
    }

    public void StopWiggle()
    {
        if (wiggleSequence != null && wiggleSequence.IsActive())
        {
            wiggleSequence.Kill();
            wiggleSequence = null;
        }

        if (rectTransform != null)
        {
            rectTransform.localEulerAngles = originalEulerAngles;
            rectTransform.localScale = originalScale;
        }
    }
}
