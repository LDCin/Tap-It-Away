using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/Game Theme", fileName = "New Game Theme", order = 2)]
public class GameThemeSO : ScriptableObject
{
    public GameThemeType themeType;
    public Sprite menuBackgroundSprite;
    public Sprite gameplayBackgroundSprite;
    public Sprite boosterButtonSprite;
}
