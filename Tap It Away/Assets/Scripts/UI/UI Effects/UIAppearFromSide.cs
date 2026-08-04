using DG.Tweening;
using UnityEngine;

public class UIAppearFromSide : UIEffect
{
    [SerializeField] private float deltaX = 1000;
    [SerializeField] private float duration = 0.5f;

    private Vector2 _originalPosition;
    private bool _originalPositionSet = false;
    private CanvasGroup _canvasGroup;

    public void SetDelay(float pDelay)
    {
        showDelayTime = pDelay;
    }

    public override float ClosePanelDuration => duration;

    public override void ShowEffect(float showDelayTime)
    {
        base.ShowEffect(showDelayTime);
        if(_canvasGroup == null)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        if(_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if(_canvasGroup != null)
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.DOFade(1, duration).SetUpdate(deltaTimeIndependent).SetDelay(showDelayTime);
        }
        
        RectTransform rectTransform = GetComponent<RectTransform>();
        if(!_originalPositionSet)
        {
            _originalPosition = rectTransform.anchoredPosition;
            _originalPositionSet = true;
        }
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + deltaX, rectTransform.anchoredPosition.y);
        rectTransform.DOAnchorPosX(_originalPosition.x, duration).SetUpdate(deltaTimeIndependent).OnComplete(() => FinishShowEffect()).SetDelay(showDelayTime);
    }

    public override void HideEffect(float hideDelayTime)
    {
        base.HideEffect(hideDelayTime);

        if(_canvasGroup == null)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (_canvasGroup != null)
        {
            _canvasGroup.DOFade(0, duration).SetUpdate(deltaTimeIndependent).SetDelay(hideDelayTime);
        }

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.DOAnchorPosX(_originalPosition.x - deltaX, duration).SetUpdate(deltaTimeIndependent).OnComplete(() => { rectTransform.anchoredPosition = _originalPosition; }).SetDelay(hideDelayTime);
    }
}
