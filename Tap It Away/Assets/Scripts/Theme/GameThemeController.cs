using UnityEngine;
using UnityEngine.UI;

public class GameThemeController : Singleton<GameThemeController>
{
    [SerializeField] private GameThemeSO lightTheme;
    [SerializeField] private GameThemeSO darkTheme;
    [SerializeField] private GameThemeType currentThemeType = GameThemeType.Light;
    [SerializeField] private GameThemeBackgroundType currentBackgroundType = GameThemeBackgroundType.Menu;
    [SerializeField] private Image backgroundImage;

    public GameThemeType CurrentThemeType => currentThemeType;
    public GameThemeBackgroundType CurrentBackgroundType => currentBackgroundType;
    public GameThemeSO CurrentTheme => GetTheme(currentThemeType);

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        LoadThemeFromUserData();
        ApplyCurrentTheme();
    }

    public void ToggleTheme()
    {
        currentThemeType = currentThemeType == GameThemeType.Light
            ? GameThemeType.Dark
            : GameThemeType.Light;

        SaveThemeToUserData();
        ApplyCurrentTheme();
    }

    public void SetTheme(GameThemeType themeType)
    {
        if (currentThemeType == themeType)
        {
            ApplyCurrentTheme();
            return;
        }

        currentThemeType = themeType;
        SaveThemeToUserData();
        ApplyCurrentTheme();
    }

    private void LoadThemeFromUserData()
    {
        if (DataManager.Instance == null || DataManager.Instance.CurrentUserData == null)
        {
            return;
        }

        currentThemeType = DataManager.Instance.CurrentUserData.themeType;
    }

    private void SaveThemeToUserData()
    {
        DataManager.Instance?.SetThemeType(currentThemeType);
    }

    public void SetBackgroundType(GameThemeBackgroundType backgroundType)
    {
        currentBackgroundType = backgroundType;
        ApplyCurrentTheme();
    }

    private void ApplyCurrentTheme()
    {
        GameThemeSO theme = CurrentTheme;
        ApplyBackgrounds(theme);
        Observer.Publish(ObserverEvent.ThemeChanged, theme);
    }

    private void ApplyBackgrounds(GameThemeSO theme)
    {
        if (theme == null)
        {
            return;
        }

        if (backgroundImage == null)
        {
            return;
        }

        Sprite backgroundSprite = currentBackgroundType == GameThemeBackgroundType.Gameplay
            ? theme.gameplayBackgroundSprite
            : theme.menuBackgroundSprite;

        if (backgroundSprite != null)
        {
            backgroundImage.sprite = backgroundSprite;
        }
    }

    private GameThemeSO GetTheme(GameThemeType themeType)
    {
        return themeType == GameThemeType.Dark ? darkTheme : lightTheme;
    }
}
