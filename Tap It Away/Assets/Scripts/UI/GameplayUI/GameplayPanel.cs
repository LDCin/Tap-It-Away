using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameplayPanel : Panel
{
    public static event Action<BoosterType> OnBoosterActive;

    [Header("Theme")]
    [SerializeField] private Button themeToggleButton;
    [SerializeField] private Sprite lightThemeToggleSprite;
    [SerializeField] private Sprite darkThemeToggleSprite;

    [Header("Heartbox")]
    [SerializeField] private Transform heartRoot;
    [SerializeField] private Image heartPrefab;
    [SerializeField] private Sprite heartSprite;
    [FormerlySerializedAs("halfBrokenHeartSprite")]
    [SerializeField] private Sprite brokenHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;
    private List<Image> hearts;

    [Header("Level")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI cubeCountText;

    [Header("Booster")]
    [SerializeField] private Transform boosterRoot;
    [SerializeField] private BoosterButton boosterButtonPrefab;
    [SerializeField] private TutorialPanel tutorialPanel;
    private List<BoosterButton> boosters;
    private int currentHeartCount;

    private void OnEnable()
    {
        LevelManager.OnCubeCountChanged += HandleCubeCountChanged;
        LevelManager.OnHeartCountChanged += HandleHeartCountChanged;
        GameThemeController.OnThemeChanged += ApplyTheme;
        if (themeToggleButton != null)
        {
            themeToggleButton.onClick.AddListener(ToggleTheme);
        }
    }

    private void OnDisable()
    {
        LevelManager.OnCubeCountChanged -= HandleCubeCountChanged;
        LevelManager.OnHeartCountChanged -= HandleHeartCountChanged;
        GameThemeController.OnThemeChanged -= ApplyTheme;
        if (themeToggleButton != null)
        {
            themeToggleButton.onClick.RemoveListener(ToggleTheme);
        }
    }

    public override void UpdateVisual()
    {
        ApplyTheme(GameThemeController.Instance != null ? GameThemeController.Instance.CurrentTheme : null);
        ClearHearts();
        hearts = new();
        currentHeartCount = LevelManager.Instance.MaxHeart;
        for (int i = 0; i < LevelManager.Instance.MaxHeart; i++)
        {
            Image image = Instantiate(heartPrefab, heartRoot);
            image.sprite = heartSprite;
            SetImageAlpha(image, 1f);
            hearts.Add(image);
        }
        levelText.text = LevelManager.Instance != null
            ? LevelManager.Instance.LevelDisplayName
            : DataManager.Instance.GetCurrentLevelDisplayName();
        UpdateCubeCount(LevelManager.Instance.CurrentLevelState != null
            ? LevelManager.Instance.CurrentLevelState.RemainingCubeCount
            : 0);
        SpawnBoostersAsync().Forget();
    }

    public void ToggleTheme()
    {
        GameThemeController.Instance?.ToggleTheme();
    }

    public void OpenSettingInGame()
    {
        UIManager.Instance?.OpenPanel(GameConfig.SETTING_IN_GAME_PANEL);
    }

    private void ApplyTheme(GameThemeSO _)
    {
        UpdateThemeToggleSprite();
    }

    private void UpdateThemeToggleSprite()
    {
        if (themeToggleButton == null || themeToggleButton.image == null || GameThemeController.Instance == null)
        {
            return;
        }

        bool isLightTheme = GameThemeController.Instance.CurrentThemeType == GameThemeType.Light;
        Sprite toggleSprite = isLightTheme ? darkThemeToggleSprite : lightThemeToggleSprite;
        if (toggleSprite != null)
        {
            themeToggleButton.image.sprite = toggleSprite;
        }
    }

    private void HandleCubeCountChanged(int remainingCubeCount)
    {
        UpdateCubeCount(remainingCubeCount);
    }

    private void UpdateCubeCount(int remainingCubeCount)
    {
        if (cubeCountText != null)
        {
            cubeCountText.text = remainingCubeCount.ToString();
        }
    }

    private void HandleHeartCountChanged(int remainingHeartCount)
    {
        if (hearts == null)
        {
            currentHeartCount = remainingHeartCount;
            return;
        }

        int lostHeartIndex = remainingHeartCount;
        if (remainingHeartCount < currentHeartCount && lostHeartIndex >= 0 && lostHeartIndex < hearts.Count)
        {
            PlayBrokenHeartEffect(hearts[lostHeartIndex]);
        }

        currentHeartCount = remainingHeartCount;
        UpdateHearts(remainingHeartCount);
    }

    private void UpdateHearts(int remainingHeartCount)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            bool hasHeart = i < remainingHeartCount;
            hearts[i].sprite = hasHeart || emptyHeartSprite == null ? heartSprite : emptyHeartSprite;
            SetImageAlpha(hearts[i], hasHeart || emptyHeartSprite != null ? 1f : 0.25f);
        }
    }

    private void PlayBrokenHeartEffect(Image heart)
    {
        if (heart == null || brokenHeartSprite == null)
        {
            return;
        }

        SpawnBrokenHeart(heart);
    }

    private void SpawnBrokenHeart(Image sourceHeart)
    {
        Image brokenHeart = Instantiate(heartPrefab, heartRoot);
        RectTransform sourceRect = sourceHeart.rectTransform;
        RectTransform brokenRect = brokenHeart.rectTransform;

        brokenHeart.sprite = brokenHeartSprite;
        brokenHeart.raycastTarget = false;
        brokenHeart.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
        SetImageAlpha(brokenHeart, 1f);

        brokenRect.SetAsLastSibling();
        brokenRect.anchorMin = sourceRect.anchorMin;
        brokenRect.anchorMax = sourceRect.anchorMax;
        brokenRect.pivot = sourceRect.pivot;
        brokenRect.sizeDelta = sourceRect.sizeDelta;
        brokenRect.anchoredPosition = sourceRect.anchoredPosition;
        brokenRect.localScale = Vector3.one;

        Vector2 endPosition = brokenRect.anchoredPosition + new Vector2(0f, -90f);
        DOTween.Sequence()
            .Append(brokenRect.DOAnchorPos(endPosition, 0.55f).SetEase(Ease.InQuad))
            .Join(brokenHeart.DOFade(0f, 0.55f))
            .SetLink(brokenHeart.gameObject)
            .OnComplete(() => Destroy(brokenHeart.gameObject));
    }

    private async UniTask SpawnBoostersAsync()
    {
        ClearBoosters();

        if (BoosterManager.Instance == null)
        {
            Debug.LogWarning("BoosterManager is missing.");
            return;
        }

        await BoosterManager.Instance.WaitUntilInitialized();
        BoosterManager.Instance.SyncUnlocksForCurrentLevel();

        if (!isActiveAndEnabled)
        {
            return;
        }

        boosters = new();
        foreach (BoosterType boosterType in Enum.GetValues(typeof(BoosterType)))
        {
            BoosterButton boosterButton = Instantiate(boosterButtonPrefab, boosterRoot);
            boosterButton.SetBoosterButton(boosterType);

            boosterButton.onClick.AddListener(() =>
            {
                OnBoosterActive?.Invoke(boosterType);
            });

            boosters.Add(boosterButton);
        }

        AudioManager.Instance?.RegisterButtonClickSounds(transform);
        ShowBoosterTutorialIfNeeded();
    }

    private void ShowBoosterTutorialIfNeeded()
    {
        if (BoosterManager.Instance == null || !BoosterManager.Instance.TryGetUnshownUnlockedBooster(out BoosterType boosterType))
        {
            return;
        }

        if (tutorialPanel == null)
        {
            Debug.LogWarning("Tutorial panel is missing.");
            return;
        }

        tutorialPanel.Init(boosterType);
        tutorialPanel.Open();
    }

    private void ClearBoosters()
    {
        if (boosters == null)
        {
            return;
        }

        foreach (BoosterButton booster in boosters)
        {
            if (booster != null)
            {
                Destroy(booster.gameObject);
            }
        }

        boosters.Clear();
    }

    private void ClearHearts()
    {
        if (hearts == null)
        {
            return;
        }

        foreach (Image heart in hearts)
        {
            if (heart != null)
            {
                Destroy(heart.gameObject);
            }
        }

        hearts.Clear();
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
