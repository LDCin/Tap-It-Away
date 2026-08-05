using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayPanel : Panel
{
    public static event Action<BoosterType> OnBoosterActive;

    [Header("Heartbox")]
    [SerializeField] private Transform heartRoot;
    [SerializeField] private Image heartPrefab;
    [SerializeField] private Sprite heartSprite;
    [SerializeField] private Sprite brokenHeartSprite;
    private List<Image> hearts;

    [Header("Level")]
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Booster")]
    [SerializeField] private Transform boosterRoot;
    [SerializeField] private BoosterButton boosterButtonPrefab;
    private List<BoosterButton> boosters;
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
        SpawnBoostersAsync().Forget();
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

        if (!isActiveAndEnabled)
        {
            return;
        }

        boosters = new();
        List<BoosterType> unlockedBoosters = BoosterManager.Instance.UnlockedBoosters;
        if (unlockedBoosters == null)
        {
            return;
        }

        foreach (var unlockedBooster in unlockedBoosters)
        {
            BoosterType boosterType = unlockedBooster;

            BoosterButton boosterButton = Instantiate(boosterButtonPrefab, boosterRoot);
            boosterButton.SetBoosterButton(boosterType);

            boosterButton.onClick.AddListener(() =>
            {
                OnBoosterActive?.Invoke(boosterType);
            });

            boosters.Add(boosterButton);
        }
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
}
