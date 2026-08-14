using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeTab : Panel
{
    [SerializeField] private List<LevelTreeNode> levelTreeNodes;
    [SerializeField] private Panel noAdsPanel;
    [Header("Play Button")]
    [SerializeField] private Image playButtonImage;
    [SerializeField] private Sprite normalPlayButtonSprite;
    [SerializeField] private Sprite hardPlayButtonSprite;
    [SerializeField] private TMP_Text playLevelText;
    [SerializeField] private TMP_Text coinText;

    [Header("Player Progress")]
    [SerializeField, Min(1)] private int currentLevelNumber = 1;
    [SerializeField] private int[] hardLevelNumbers;

    private void OnEnable()
    {
        Observer.Subscribe<int>(ObserverEvent.CoinCountChanged, UpdateCoinText);
    }

    private void OnDisable()
    {
        Observer.Unsubscribe<int>(ObserverEvent.CoinCountChanged, UpdateCoinText);
    }

    public override void UpdateVisual()
    {
        LoadCurrentLevel();
        RefreshLevelTree();
        RefreshPlayButton();
        UpdateCoinText(DataManager.Instance != null ? DataManager.Instance.GetCoins() : 0);
    }

    public void OnPlayButtonClicked()
    {
        Observer.Publish(ObserverEvent.PlayGame);
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
        UserData userData = DataManager.Instance.CurrentUserData;
        if (userData == null)
        {
            return;
        }

        currentLevelNumber = DataManager.Instance.GetCurrentLevelNumber();
    }

    private void RefreshLevelTree()
    {
        int currentLevelNumber = DataManager.Instance.GetCurrentLevelNumber();
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

        playLevelText.text = DataManager.Instance.GetCurrentLevelDisplayName();

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

    private void UpdateCoinText(int coins)
    {
        if (coinText == null)
        {
            return;
        }

        coinText.text = coins.ToString();
    }
}
