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

    private void OnEnable()
    {
        Observer.Subscribe(ObserverEvent.OnSettingChanged, UpdateSettingButtons);
    }

    private void OnDisable()
    {
        Observer.Unsubscribe(ObserverEvent.OnSettingChanged, UpdateSettingButtons);
    }

    public override void UpdateVisual()
    {
        UpdateSettingButtons();
    }

    public void BackToMenu()
    {
        LevelManager.Instance.DestroyLevel();
        Observer.Publish(ObserverEvent.OnBackToMenu);
    }

    public void CloseSettingInGame()
    {
        UIManager.Instance.ClosePanel(GameConfig.SETTING_IN_GAME_PANEL);
        Observer.Publish(ObserverEvent.OnCloseSettingInGame);
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
