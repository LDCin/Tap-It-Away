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
    }
    private void OnDisable()
    {
        Observer.Unsubscribe(ObserverEvent.CubeBlocked, BreakHeart);
    }
    public override void UpdateVisual()
    {
        if (hearts != null)
        {
            hearts.Clear();
        }
        hearts = new();
        for (int i = 0; i < LevelManager.Instance.MaxHeart; i++)
        {
            Image image = Instantiate(heartPrefab, heartRoot);
            heartPrefab.sprite = heartSprite;
            hearts.Add(image);
        }
        levelText.text = LevelManager.Instance.LevelName;
    }
    private void BreakHeart()
    {
        Image heart = hearts[hearts.Count - 1];
        RectTransform rect = heart.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = heart.GetComponent<CanvasGroup>();
        heart.sprite = brokenHeartSprite;
        Sequence sq = DOTween.Sequence();
        sq.Join(rect.DOAnchorPosY(rect.anchoredPosition.y - heartFallDistance, heartFallDuration).SetEase(Ease.OutQuart));
        sq.Join(canvasGroup.DOFade(0, heartFallDuration));
        sq.OnComplete(() =>
        {
            hearts.Remove(heart);
            // heart.gameObject.SetActive(false);
        });
    }
    public void OpenSettingInGame()
    {
        Observer.Publish(ObserverEvent.OnOpenSettingInGame);
    }
    
}