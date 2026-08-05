using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingInGamePanel : Panel
{
    public static event Action OnBackToMenu;

    [SerializeField] private Button BGMButton;
    [SerializeField] private Button SFXButton;
    [SerializeField] private Button HapticButton;
    [SerializeField] private Sprite onBGMSprite;
    [SerializeField] private Sprite offBGMSprite;
    [SerializeField] private Sprite onSFXSprite;
    [SerializeField] private Sprite offSFXSprite;
    [SerializeField] private Sprite onHapticSprite;
    [SerializeField] private Sprite offHapticSprite;

    private void OnEnable()
    {
        DataManager.OnSettingsChanged += UpdateSettingButtons;
    }

    private void OnDisable()
    {
        DataManager.OnSettingsChanged -= UpdateSettingButtons;
    }

    public override void UpdateVisual()
    {
        UpdateSettingButtons();
    }

    public void BackToMenu()
    {
        LevelManager.Instance.DestroyLevel();
        OnBackToMenu?.Invoke();
    }

    public void CloseSettingInGame()
    {
        UIManager.Instance.ClosePanel(GameConfig.SETTING_IN_GAME_PANEL);
    }

    public void ToggleBGM()
    {
        AudioManager.Instance?.ChangeBGMState();
    }

    public void ToggleSFX()
    {
        AudioManager.Instance?.ChangeSFXState();
    }

    public void ToggleHaptic()
    {
        DataManager.Instance?.ToggleHapticEnabled();
    }

    private void UpdateSettingButtons()
    {
        DataManager dataManager = DataManager.Instance;
        if (dataManager == null)
        {
            return;
        }

        SetButtonSprite(BGMButton, dataManager.BgmEnabled ? onBGMSprite : offBGMSprite);
        SetButtonSprite(SFXButton, dataManager.SfxEnabled ? onSFXSprite : offSFXSprite);
        SetButtonSprite(HapticButton, dataManager.HapticEnabled ? onHapticSprite : offHapticSprite);
    }

    private void SetButtonSprite(Button button, Sprite sprite)
    {
        if (button == null || button.image == null || sprite == null)
        {
            return;
        }

        button.image.sprite = sprite;
    }
}
