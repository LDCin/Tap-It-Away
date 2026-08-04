using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIContinuousRotate : UIEffect
{
    [SerializeField] private float degreesPerSecond = 30f;

    private RectTransform rectTransform;
    private Tween rotateTween;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public override void ShowEffect(float showDelayTime)
    {
        base.ShowEffect(showDelayTime);
        StartRotate(showDelayTime);
    }

    public override void HideEffect(float hideDelayTime)
    {
        base.HideEffect(hideDelayTime);
        StopRotate();
    }

    public void StartRotate(float delayTime = 0f)
    {
        StopRotate();

        if (Mathf.Approximately(degreesPerSecond, 0f))
        {
            return;
        }

        float duration = 360f / Mathf.Abs(degreesPerSecond);
        float direction = Mathf.Sign(degreesPerSecond);

        rotateTween = rectTransform
            .DOLocalRotate(new Vector3(0f, 0f, 360f * direction), duration, RotateMode.LocalAxisAdd)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental)
            .SetDelay(delayTime)
            .SetUpdate(deltaTimeIndependent);
    }

    public void StopRotate()
    {
        if (rotateTween != null && rotateTween.IsActive())
        {
            rotateTween.Kill();
            rotateTween = null;
        }
    }
}
