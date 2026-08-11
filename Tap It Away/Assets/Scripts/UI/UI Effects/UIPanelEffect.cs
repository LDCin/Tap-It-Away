using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanelEffect : UIEffect
{
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private bool canInteractWhileFading = true;
    private RectTransform panel;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        panel = GetComponent<RectTransform>();
    }

    public override float ClosePanelDuration => duration;

    public override void ShowEffect(float showDelayTime)
    {
        base.ShowEffect(showDelayTime);

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        canvasGroup.DOKill();

        panel.transform.DOKill();

        canvasGroup.interactable = canInteractWhileFading;

        canvasGroup.alpha = 0;

        canvasGroup.DOFade(1, duration).SetUpdate(deltaTimeIndependent).OnComplete(FinishShowEffect).SetDelay(showDelayTime);

        panel.transform.localScale = Vector3.one * 0.8f;
        panel.transform.DOScale(1, duration).SetUpdate(deltaTimeIndependent).SetDelay(showDelayTime).SetEase(Ease.OutBack);
    }

    public override void FinishShowEffect()
    {
        base.FinishShowEffect();
        canvasGroup.interactable = true;
    }

    public override void HideEffect(float hideDelayTime)
    {
        base.HideEffect(hideDelayTime);

        canvasGroup.DOFade(0, duration).SetDelay(hideDelayTime).SetUpdate(deltaTimeIndependent);

        panel.transform.DOScale(0.8f, duration).SetUpdate(deltaTimeIndependent).SetDelay(hideDelayTime).SetEase(Ease.InBack);
    }
}
