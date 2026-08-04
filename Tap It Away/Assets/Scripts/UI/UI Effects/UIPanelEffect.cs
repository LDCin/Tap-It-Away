using DG.Tweening;
using UnityEngine;

public class UIPanelEffect : UIEffect
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private bool canInteractWhileFading = true;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public override float ClosePanelDuration => duration;

    public override void ShowEffect(float showDelayTime)
    {
        base.ShowEffect(showDelayTime);

        if (_canvasGroup == null)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        _canvasGroup.DOKill();

        panel.transform.DOKill();

        _canvasGroup.interactable = canInteractWhileFading;

        _canvasGroup.alpha = 0;

        _canvasGroup.DOFade(1, duration).SetUpdate(deltaTimeIndependent).OnComplete(FinishShowEffect).SetDelay(showDelayTime);

        panel.transform.localScale = Vector3.one * 0.8f;
        panel.transform.DOScale(1, duration).SetUpdate(deltaTimeIndependent).SetDelay(showDelayTime).SetEase(Ease.OutBack);
    }

    public override void FinishShowEffect()
    {
        base.FinishShowEffect();
        _canvasGroup.interactable = true;
    }

    public override void HideEffect(float hideDelayTime)
    {
        base.HideEffect(hideDelayTime);

        _canvasGroup.DOFade(0, duration).SetDelay(hideDelayTime).SetUpdate(deltaTimeIndependent);

        panel.transform.DOScale(0.8f, duration).SetUpdate(deltaTimeIndependent).SetDelay(hideDelayTime).SetEase(Ease.InBack);
    }
}
