using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum LevelDifficulty
{
    Normal,
    Hard
}

public class LevelTreeNode : MonoBehaviour
{
    [SerializeField] private TMP_Text levelNumberText;
    [SerializeField] private Image bgImage;
    [SerializeField] private Image skullImage;
    [SerializeField] private Image hardBannerImage;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite lockSprite;
    [SerializeField] private Sprite hardSprite;
    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color hardTextColor = Color.white;
    [SerializeField] private Color lockedTextColor = Color.white;
    [SerializeField, Range(0f, 1f)] private float bannerHiddenAlpha = 0f;
    [SerializeField, Range(0f, 1f)] private float bannerVisibleAlpha = 1f;
    [SerializeField, Range(0f, 1f)] private float bannerBlinkAlpha = 0.25f;
    [SerializeField, Min(0.01f)] private float bannerBlinkDuration = 0.35f;

    private Tween hardBannerTween;

    public void SetLevelDetail(int levelNumber, LevelDifficulty difficulty, bool isUnlocked)
    {
        bool isHardLevel = isUnlocked && difficulty == LevelDifficulty.Hard;

        if (levelNumberText != null)
        {
            levelNumberText.text = levelNumber.ToString("00");
            levelNumberText.color = GetTextColor(difficulty, isUnlocked);
        }

        if (bgImage != null)
        {
            Sprite sprite = GetSprite(difficulty, isUnlocked);
            if (sprite != null)
            {
                bgImage.sprite = sprite;
            }
        }

        SetHardImage(skullImage, isHardLevel);
        SetHardBanner(isHardLevel);
    }

    private Sprite GetSprite(LevelDifficulty difficulty, bool isUnlocked)
    {
        if (!isUnlocked && lockSprite != null)
        {
            return lockSprite;
        }

        return difficulty == LevelDifficulty.Hard && hardSprite != null ? hardSprite : normalSprite;
    }

    private void SetHardImage(Image image, bool isHardLevel)
    {
        if (image == null)
        {
            return;
        }

        image.gameObject.SetActive(isHardLevel);
    }

    private void SetHardBanner(bool isHardLevel)
    {
        if (hardBannerImage == null)
        {
            return;
        }

        hardBannerImage.gameObject.SetActive(true);
        hardBannerTween?.Kill();

        if (!isHardLevel)
        {
            SetImageAlpha(hardBannerImage, bannerHiddenAlpha);
            return;
        }

        SetImageAlpha(hardBannerImage, bannerVisibleAlpha);
        hardBannerTween = hardBannerImage.DOFade(bannerBlinkAlpha, bannerBlinkDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetLink(hardBannerImage.gameObject);
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    private Color GetTextColor(LevelDifficulty difficulty, bool isUnlocked)
    {
        if (!isUnlocked)
        {
            return lockedTextColor;
        }

        return difficulty == LevelDifficulty.Hard ? hardTextColor : normalTextColor;
    }

    [ContextMenu("Test: Normal Level 1")]
    public void TestNormalLevel()
    {
        SetLevelDetail(1, LevelDifficulty.Normal, true);
    }

    [ContextMenu("Test: Hard Level 5")]
    public void TestHardLevel()
    {
        SetLevelDetail(5, LevelDifficulty.Hard, true);
    }

    [ContextMenu("Test: Locked Level 6")]
    public void TestLockedLevel()
    {
        SetLevelDetail(6, LevelDifficulty.Normal, false);
    }

    private void OnDisable()
    {
        hardBannerTween?.Kill();
        hardBannerTween = null;
    }
}
