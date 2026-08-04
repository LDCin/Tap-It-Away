using UnityEngine;
using DG.Tweening;

public class Jiggle : UIEffect
{
    [Header("Jiggle Configuration")]
    [SerializeField] private float strength = 0.2f;
    [SerializeField] private float frequency = 2.0f;
    [SerializeField] private bool playOnEnable = true;
    [SerializeField] private bool loopJiggle = true;
    
    private RectTransform _rectTransform;
    private Vector3 _originalScale;
    private Sequence _jiggleSequence;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _originalScale = _rectTransform.localScale;
    }
    
    public override void ShowEffect(float showDelayTime)
    {
        base.ShowEffect(showDelayTime);

        if (playOnEnable)
        {
            StartJiggle(showDelayTime);
        }
    }

    public override void HideEffect(float hideDelayTime)
    {
        base.HideEffect(hideDelayTime);
        StopJiggle();
    }
    
    public void StartJiggle(float delayTime = 0f)
    {
        StopJiggle();
        
        float duration = 1f / frequency;
        
        _jiggleSequence = DOTween.Sequence();
        
        _jiggleSequence.Append(_rectTransform.DOScale(_originalScale * (1f + strength), duration * 0.5f).SetEase(Ease.OutQuad));
        
        _jiggleSequence.Append(_rectTransform.DOScale(_originalScale, duration * 0.5f).SetEase(Ease.InOutQuad));
        
        if (loopJiggle)
        {
            _jiggleSequence.SetLoops(-1);
        }

        _jiggleSequence.SetDelay(delayTime).SetUpdate(deltaTimeIndependent);
    }
    
    public void StopJiggle()
    {
        if (_jiggleSequence != null && _jiggleSequence.IsActive())
        {
            _jiggleSequence.Kill();
            _jiggleSequence = null;
        }
        
        if (_rectTransform != null)
        {
            _rectTransform.localScale = _originalScale;
        }
    }

    public void SetJiggleParameters(float newStrength, float newFrequency)
    {
        strength = newStrength;
        frequency = newFrequency;
        
        if (_jiggleSequence != null && _jiggleSequence.IsActive())
        {
            StartJiggle();
        }
    }
}
