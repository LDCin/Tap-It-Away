using UnityEngine;
using UnityEngine.UI;

public class SettingInMenuPanel : Panel
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
        DataManager.OnSettingsChanged += UpdateAudioButtons;
    }

    private void OnDisable()
    {
        DataManager.OnSettingsChanged -= UpdateAudioButtons;
    }

    public override void UpdateVisual()
    {
        UpdateAudioButtons();
    }

    public void CloseSetting()
    {
        UIManager.Instance.ClosePanel(GameConfig.SETTING_IN_MENU_PANEL);
    }
    public void ToggleBGM()
    {
        AudioManager.Instance?.ChangeBGMState();
        UpdateAudioButtons();
    }
    public void ToggleSFX()
    {
        AudioManager.Instance?.ChangeSFXState();
        UpdateAudioButtons();
    }
    public void ToggleHaptic()
    {
        DataManager.Instance?.ToggleHapticEnabled();
        UpdateAudioButtons();
    }

    private void UpdateAudioButtons()
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
