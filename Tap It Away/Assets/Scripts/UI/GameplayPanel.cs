using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayPanel : Panel
{
    [Header("Heartbox")]
    [SerializeField] private Transform heartRoot;
    [SerializeField] private Image heartPrefab;
    [SerializeField] private Sprite heartSprite;
    [SerializeField] private Sprite brokenHeartSprite;
    private List<Image> hearts;

    [Header("Level")]
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Booster")]
    private Button boosterButtonPrefab;
    private List<Button> boosters;
    public override void UpdateVisual()
    {
        if (hearts != null)
        {
            hearts.Clear();
        }
        hearts = new();
        for (int i = 0; i < LevelManager.Instance.MaxHeart; i++)
        {
            Image image = Instantiate(heartPrefab, heartRoot);
            heartPrefab.sprite = heartSprite;
            hearts.Add(image);
        }
        levelText.text = LevelManager.Instance.LevelName;
    }
}