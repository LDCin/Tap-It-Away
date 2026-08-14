using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoosterButton : Button
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite lockIcon;
    [SerializeField] private TextMeshProUGUI unlockLevelText;
    [SerializeField] private GameObject countFrame;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private GameObject moreBoosterIcon;

    private BoosterType boosterType;
    public BoosterType BoosterType => boosterType;

    protected override void OnEnable()
    {
        base.OnEnable();
        Observer.Subscribe<BoosterType>(ObserverEvent.BoosterCountChanged, HandleBoosterCountChanged);
        Observer.Subscribe<GameThemeSO>(ObserverEvent.ThemeChanged, ApplyTheme);
    }

    protected override void OnDisable()
    {
        Observer.Unsubscribe<BoosterType>(ObserverEvent.BoosterCountChanged, HandleBoosterCountChanged);
        Observer.Unsubscribe<GameThemeSO>(ObserverEvent.ThemeChanged, ApplyTheme);
        base.OnDisable();
    }

    public void SetBoosterButton(BoosterType boosterType)
    {
        this.boosterType = boosterType;

        bool isUnlocked = BoosterManager.Instance.IsBoosterUnlocked(boosterType);
        BoosterSO boosterSO = BoosterManager.Instance.GetBoosterSO(boosterType);
        ApplyTheme(GameThemeController.Instance != null ? GameThemeController.Instance.CurrentTheme : null);
        SetUnlockState(isUnlocked, boosterSO);
        SetCount(BoosterManager.Instance.GetBoosterCount(boosterType));
    }

    private void ApplyTheme(GameThemeSO theme)
    {
        if (theme == null || backgroundImage == null || theme.boosterButtonSprite == null)
        {
            return;
        }

        backgroundImage.sprite = theme.boosterButtonSprite;
    }

    private void HandleBoosterCountChanged(BoosterType boosterType)
    {
        if (this.boosterType != boosterType)
        {
            return;
        }

        SetCount(BoosterManager.Instance.GetBoosterCount(boosterType));
    }

    private void SetCount(int count)
    {
        if (!BoosterManager.Instance.IsBoosterUnlocked(boosterType))
        {
            return;
        }

        bool hasBooster = count > 0;

        if (countText != null)
        {
            countText.text = count.ToString();
        }

        if (countFrame != null)
        {
            countFrame.SetActive(hasBooster);
        }

        if (moreBoosterIcon != null)
        {
            moreBoosterIcon.SetActive(!hasBooster);
        }
    }

    private void SetUnlockState(bool isUnlocked, BoosterSO boosterSO)
    {
        if (icon != null)
        {
            if (!isUnlocked && lockIcon != null)
            {
                icon.sprite = lockIcon;
            }
            else if (boosterSO != null && boosterSO.icon != null)
            {
                icon.sprite = boosterSO.icon;
            }
        }

        if (unlockLevelText != null)
        {
            unlockLevelText.gameObject.SetActive(!isUnlocked);
            if (!isUnlocked && boosterSO != null)
            {
                unlockLevelText.text = $"Level {boosterSO.unlockLevel:00}";
            }
        }

        if (!isUnlocked)
        {
            if (countFrame != null)
            {
                countFrame.SetActive(false);
            }

            if (moreBoosterIcon != null)
            {
                moreBoosterIcon.SetActive(false);
            }
        }
    }
}
