using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : Panel
{
    [SerializeField] private Image progress;
    [SerializeField] private float loadingDuration = 2f;

    private Tween _loadingTween;
    public bool IsLoading { get; private set; }
    public bool IsDone { get; private set; }

    public override void UpdateVisual()
    {
        base.UpdateVisual();
        StartLoading();
    }

    private void StartLoading()
    {
        IsLoading = true;
        IsDone = false;

        _loadingTween?.Kill();
        progress.fillAmount = 0f;
        _loadingTween = progress.DOFillAmount(1f, loadingDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                IsLoading = false;
                IsDone = true;
                _loadingTween = null;
            });
    }
}
