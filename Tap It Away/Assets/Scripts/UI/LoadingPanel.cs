using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : Panel
{
    [SerializeField] private Image progressImage;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private float fillDuration = 1f;

    public float FillDuration => fillDuration;

    private void OnDisable()
    {
        progressImage?.DOKill();
    }

    public override void UpdateVisual()
    {
        if (progressImage != null)
        {
            progressImage.DOKill();
            progressImage.fillAmount = 0f;
            progressImage.DOFillAmount(1f, fillDuration).SetEase(Ease.Linear);
        }

        if (loadingText != null)
        {
            loadingText.text = "Loading...";
        }
    }
}
