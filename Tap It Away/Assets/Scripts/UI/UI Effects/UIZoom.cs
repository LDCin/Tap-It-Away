using DG.Tweening;
using UnityEngine;

public class UIZoom : UIEffect
{
    [SerializeField] private Vector3 startScale = Vector3.zero;
    [SerializeField] private Vector3 endScale = Vector3.one;
    [SerializeField] private bool useCustomCurve = false;
    [SerializeField] private bool useFade = false;
    [SerializeField] private Ease curve = Ease.OutBack;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float closeDuration = 0.5f;

    public override float ClosePanelDuration => closeDuration;

    public override void ShowEffect(float showDelayTime)
    {
        base.ShowEffect(showDelayTime);
        
        transform.DOKill();
        
        if (useCustomCurve)
        {
            transform.localScale = startScale;
            transform.DOScale(endScale, duration).SetEase(curve).SetUpdate(deltaTimeIndependent).SetDelay(showDelayTime).OnComplete(FinishShowEffect);
        }
        else
        {
            transform.localScale = startScale;
            transform.DOScale(endScale, duration).SetUpdate(deltaTimeIndependent).SetDelay(showDelayTime).OnComplete(FinishShowEffect);
        }

        if (!useFade) return;
        var canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.DOKill();
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, duration).SetUpdate(deltaTimeIndependent).SetDelay(showDelayTime);
    }

    public override void HideEffect(float hideDelayTime)
    {
        base.HideEffect(hideDelayTime);
        
        transform.DOKill();
        
        if (useCustomCurve)
        {
            transform.DOScale(startScale, closeDuration).SetEase(curve).SetUpdate(deltaTimeIndependent).SetDelay(hideDelayTime);
        }
        else
        {
            transform.DOScale(startScale, closeDuration).SetUpdate(deltaTimeIndependent).SetDelay(hideDelayTime);
        }

        if (!useFade) return;
        var canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.DOKill();
        canvasGroup.DOFade(0, closeDuration).SetUpdate(deltaTimeIndependent).SetDelay(hideDelayTime);
    }
}
