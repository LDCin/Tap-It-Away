using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class SuccessPanel : Panel
{
    public static event Action OnNextLevel;

    [SerializeField] private Image congratulationTextImage;
    [SerializeField] private List<Sprite> congratulationTextSprites;
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private UIFlyingItemsEffect coinRewardEffect;

    [Header("Coin Reward")]
    [SerializeField] private RectTransform nextLevelCoinIcon;
    [SerializeField] private RectTransform moreRewardCoinIcon;
    [SerializeField, Min(0)] private int nextLevelCoinReward = 10;
    [SerializeField, Min(0)] private int moreRewardCoinReward = 30;
    private bool isCoinRewardClaimed;

    private void OnEnable()
    {
        DataManager.OnCoinsChanged += UpdateCoinText;
        isCoinRewardClaimed = false;
    }

    private void OnDisable()
    {
        DataManager.OnCoinsChanged -= UpdateCoinText;
    }

    public override void UpdateVisual()
    {
        ShowRandomCongratulationText();
        UpdateCoinText(DataManager.Instance != null ? DataManager.Instance.GetCoins() : 0);
    }

    public void NextLevel()
    {
        AudioManager.Instance?.StopCurrentSFX();
        ClaimCoinReward(nextLevelCoinIcon, nextLevelCoinReward, ContinueToNextLevel);
    }

    public void MoreReward()
    {
        AudioManager.Instance?.StopCurrentSFX();
        ClaimCoinReward(moreRewardCoinIcon, moreRewardCoinReward, ContinueToNextLevel);
    }

    private void ContinueToNextLevel()
    {
        LevelManager.Instance.DestroyLevel();
        OnNextLevel?.Invoke();
    }

    private void ShowRandomCongratulationText()
    {
        if (congratulationTextImage == null || congratulationTextSprites == null || congratulationTextSprites.Count == 0)
        {
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, congratulationTextSprites.Count);
        Sprite randomSprite = congratulationTextSprites[randomIndex];
        if (randomSprite == null)
        {
            return;
        }

        congratulationTextImage.sprite = randomSprite;
        congratulationTextImage.gameObject.SetActive(true);
    }

    private void ClaimCoinReward(RectTransform spawnPoint, int rewardAmount, Action onComplete)
    {
        if (isCoinRewardClaimed)
        {
            return;
        }

        isCoinRewardClaimed = true;
        int coinCountBeforeReward = DataManager.Instance != null ? DataManager.Instance.GetCoins() : 0;
        DataManager.Instance?.AddCoins(rewardAmount);

        if (coinRewardEffect != null && rewardAmount > 0)
        {
            coinRewardEffect.SetSpawnOrigin(spawnPoint);
            coinRewardEffect.Play(coinCountBeforeReward, rewardAmount, onComplete);
            return;
        }

        onComplete?.Invoke();
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
