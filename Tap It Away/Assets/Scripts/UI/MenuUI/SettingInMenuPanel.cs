using System;
using System.Collections.Generic;
using TMPro;
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
    public void CloseSetting()
    {
        UIManager.Instance.ClosePanel(GameConfig.SETTING_IN_MENU_PANEL);
    }
    public void ToggleBGM()
    {

    }
    public void ToggleSFX()
    {

    }
    public void ToggleHaptic()
    {

    }
}