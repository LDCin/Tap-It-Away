using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIFadeUp : UIEffect
{
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
        
        _canvasGroup.interactable = canInteractWhileFading;

        _canvasGroup.alpha = 0;

        _canvasGroup.DOFade(1, duration).SetUpdate(deltaTimeIndependent).OnComplete(FinishShowEffect).SetDelay(showDelayTime);
    }
    
    public override void FinishShowEffect()
    {
        base.FinishShowEffect();
        _canvasGroup.interactable = true;
    }

    public override void HideEffect(float hideDelayTime)
    {
        _canvasGroup.DOFade(0, duration).SetDelay(hideDelayTime).SetUpdate(deltaTimeIndependent);
    }
}
