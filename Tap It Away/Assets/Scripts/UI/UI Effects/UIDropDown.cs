using DG.Tweening;
using UnityEngine;

public class UIDropDown : UIEffect
{
    [SerializeField]
    private float offsetY = 150f;
    [SerializeField]
    private float duration = 0.4f;
    private Vector2 originalAnchoredPosition;

    private void Awake()
    {
        var rectTransform = GetComponent<RectTransform>();
        originalAnchoredPosition = rectTransform.anchoredPosition;
    }

    public override void ShowEffect(float showDelayTime)
    {
        base.ShowEffect(showDelayTime);
        var rectTransform = GetComponent<RectTransform>();
        
        if (rectTransform != null)
        {
            Vector2 anchoredPosition = originalAnchoredPosition;
            anchoredPosition.y += offsetY;
            rectTransform.anchoredPosition = anchoredPosition;
        }

        rectTransform.DOAnchorPos(originalAnchoredPosition, duration).SetEase(Ease.OutBounce).SetDelay(showDelayTime).SetUpdate(deltaTimeIndependent);
    }
}
