using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuccessPanel : Panel
{
    public static event Action OnNextLevel;

    [SerializeField] private Image congratulationTextImage;
    [SerializeField] private List<Sprite> congratulationTextSprites;

    public override void UpdateVisual()
    {
        ShowRandomCongratulationText();
    }

    public void NextLevel()
    {
        AudioManager.Instance?.StopCurrentSFX();
        LevelManager.Instance.DestroyLevel();
        OnNextLevel?.Invoke();
    }

    private void ShowRandomCongratulationText()
    {
        if (congratulationTextImage == null || congratulationTextSprites == null || congratulationTextSprites.Count == 0)
        {
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, congratulationTextSprites.Count);
        Sprite randomSprite = congratulationTextSprites[randomIndex];
        if (randomSprite == null)
        {
            return;
        }

        congratulationTextImage.sprite = randomSprite;
        congratulationTextImage.gameObject.SetActive(true);
    }
}
