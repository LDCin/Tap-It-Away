using UnityEngine;
using UnityEngine.UI;

public class SettingInGamePanel : Panel
{
    [SerializeField] private Button BGMButton;
    [SerializeField] private Button SFXButton;
    [SerializeField] private Button HapticButton;
    [SerializeField] private Sprite onBGMSprite;
    [SerializeField] private Sprite offBGMSprite;
    [SerializeField] private Sprite onSFXSprite;
    [SerializeField] private Sprite offSFXSprite;
    [SerializeField] private Sprite onHapticSprite;
    [SerializeField] private Sprite offHapticSprite;

    public override void UpdateVisual()
    {
        UpdateToggleSprites();
    }

    public void CloseSetting()
    {
        Observer.Publish(ObserverEvent.OnCloseSettingInGame);
    }

    public void BackToMenu()
    {
        Observer.Publish(ObserverEvent.OnBackToMenu);
    }

    public void ToggleBGM()
    {
        DataManager.Instance?.ToggleBgmEnabled();
        UpdateToggleSprites();
    }

    public void ToggleSFX()
    {
        DataManager.Instance?.ToggleSfxEnabled();
        UpdateToggleSprites();
    }

    public void ToggleHaptic()
    {
        DataManager.Instance?.ToggleHapticEnabled();
        UpdateToggleSprites();
    }

    public void OnMusicButtonClicked()
    {
        ToggleBGM();
    }

    public void OnSfxButtonClicked()
    {
        ToggleSFX();
    }

    public void OnVibrationButtonClicked()
    {
        ToggleHaptic();
    }

    public void OnExitToHomeButtonClicked()
    {
        BackToMenu();
    }

    public void Toggle()
    {
        CloseSetting();
    }

    private void UpdateToggleSprites()
    {
        DataManager dataManager = DataManager.Instance;
        bool bgmEnabled = dataManager == null || dataManager.BgmEnabled;
        bool sfxEnabled = dataManager == null || dataManager.SfxEnabled;
        bool hapticEnabled = dataManager == null || dataManager.HapticEnabled;

        SetButtonSprite(BGMButton, bgmEnabled ? onBGMSprite : offBGMSprite);
        SetButtonSprite(SFXButton, sfxEnabled ? onSFXSprite : offSFXSprite);
        SetButtonSprite(HapticButton, hapticEnabled ? onHapticSprite : offHapticSprite);
    }

    private void SetButtonSprite(Button button, Sprite sprite)
    {
        if (button == null || sprite == null)
        {
            return;
        }

        Image image = button.image;
        if (image == null || image.color.a <= 0f)
        {
            Image[] childImages = button.GetComponentsInChildren<Image>(true);
            foreach (Image childImage in childImages)
            {
                if (childImage != image)
                {
                    image = childImage;
                    break;
                }
            }
        }

        if (image != null)
        {
            image.sprite = sprite;
        }
    }
}
