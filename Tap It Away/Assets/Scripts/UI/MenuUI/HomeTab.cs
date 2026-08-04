using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeTab : Panel
{
    public static event Action OnPlayGame;

    [SerializeField] private List<LevelTreeNode> levelTreeNodes;
    [SerializeField] private Panel noAdsPanel;
    [Header("Play Button")]
    [SerializeField] private Image playButtonImage;
    [SerializeField] private Sprite normalPlayButtonSprite;
    [SerializeField] private Sprite hardPlayButtonSprite;
    [SerializeField] private TMP_Text playLevelText;

    [Header("Player Progress")]
    [SerializeField, Min(1)] private int currentLevelNumber = 1;
    [SerializeField] private int[] hardLevelNumbers;

    private void OnEnable()
    {
        LoadCurrentLevel();
        UpdateVisual();
    }

    public override void UpdateVisual()
    {
        RefreshLevelTree();
        RefreshPlayButton();
    }

    public void OnPlayButtonClicked()
    {
        OnPlayGame?.Invoke();
    }

    public void OnSettingButtonClicked()
    {
        UIManager.Instance?.OpenPanel(GameConfig.SETTING_IN_MENU_PANEL);
    }

    public void OnNoAdsButtonClicked()
    {
        UIManager.Instance?.OpenPanel(GameConfig.NO_ADS_PANEL);
    }

    private void LoadCurrentLevel()
    {
        UserData userData = GetUserData();
        if (userData == null)
        {
            return;
        }

        currentLevelNumber = Mathf.Max(1, userData.level);
    }

    private void RefreshLevelTree()
    {
        if (levelTreeNodes == null)
        {
            return;
        }

        for (int i = 0; i < levelTreeNodes.Count; i++)
        {
            LevelTreeNode node = levelTreeNodes[i];
            if (node == null)
            {
                continue;
            }

            int levelNumber = currentLevelNumber + i;
            LevelDifficulty difficulty = IsHardLevel(levelNumber) ? LevelDifficulty.Hard : LevelDifficulty.Normal;
            bool isUnlocked = i == 0;

            node.SetLevelDetail(levelNumber, difficulty, isUnlocked);
        }
    }

    private void RefreshPlayButton()
    {
        int levelNumber = currentLevelNumber;
        bool isHardLevel = IsHardLevel(levelNumber);

        if (playLevelText != null)
        {
            playLevelText.text = $"Level {levelNumber}";
        }

        if (playButtonImage == null)
        {
            return;
        }

        Sprite sprite = isHardLevel && hardPlayButtonSprite != null ? hardPlayButtonSprite : normalPlayButtonSprite;
        if (sprite != null)
        {
            playButtonImage.sprite = sprite;
        }
    }

    private bool IsHardLevel(int levelNumber)
    {
        if (hardLevelNumbers == null)
        {
            return false;
        }

        for (int i = 0; i < hardLevelNumbers.Length; i++)
        {
            if (hardLevelNumbers[i] == levelNumber)
            {
                return true;
            }
        }

        return false;
    }

    private UserData GetUserData()
    {
        if (DataManager.Instance == null)
        {
            return null;
        }

        if (DataManager.Instance.CurrentUserData == null)
        {
            DataManager.Instance.LoadUserData();
        }

        return DataManager.Instance.CurrentUserData;
    }
}
