using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayPanel : Panel
{
    [Header("Heartbox")]
    [SerializeField] private Transform heartRoot;
    [SerializeField] private Image heartPrefab;
    [SerializeField] private Sprite heartSprite;
    [SerializeField] private Sprite brokenHeartSprite;
    [SerializeField] private float heartFallDistance = 100;
    [SerializeField] private float heartFallDuration = 2;
    // [SerializeField] private float heartFadeDuration = 1;
    private List<Image> hearts;

    [Header("Level")]
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Booster")]
    private Button boosterButtonPrefab;
    private List<Button> boosters;

    private void OnEnable()
    {
        Observer.Subscribe(ObserverEvent.CubeBlocked, BreakHeart);
        Observer.Subscribe(ObserverEvent.OnBackToMenu, ClearHearts);
    }
    private void OnDisable()
    {
        Observer.Unsubscribe(ObserverEvent.CubeBlocked, BreakHeart);
        Observer.Unsubscribe(ObserverEvent.OnBackToMenu, ClearHearts);
    }
    public override void UpdateVisual()
    {
        ClearHearts();
        hearts = new();
        for (int i = 0; i < LevelManager.Instance.MaxHeart; i++)
        {
            Image image = Instantiate(heartPrefab, heartRoot);
            image.sprite = heartSprite;
            hearts.Add(image);
        }
        levelText.text = LevelManager.Instance.LevelName;
    }
    private void ClearHearts()
    {
        if (hearts != null)
        {
            foreach (var heart in hearts)
            {
                if (heart != null)
                {
                    Destroy(heart.gameObject);
                }
            }

            if (heartRoot != null)
            {
                for (int i = heartRoot.childCount - 1; i >= 0; i--)
                {
                    Transform child = heartRoot.GetChild(i);
                    child.DOKill();
                    Destroy(child.gameObject);
                }
            }

            hearts.Clear();
        }
    }
    private void BreakHeart()
    {
        Image heart = hearts[hearts.Count - 1];
        RectTransform rect = heart.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = heart.GetComponent<CanvasGroup>();
        heart.sprite = brokenHeartSprite;
        hearts.Remove(heart);
        Sequence sq = DOTween.Sequence();
        sq.Join(rect.DOAnchorPosY(rect.anchoredPosition.y - heartFallDistance, heartFallDuration).SetEase(Ease.OutQuart));
        sq.Join(canvasGroup.DOFade(0, heartFallDuration));
    }
    public void OpenSettingInGame()
    {
        Observer.Publish(ObserverEvent.OnOpenSettingInGame);
    }
}