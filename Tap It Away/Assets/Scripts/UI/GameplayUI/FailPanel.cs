using TMPro;
using UnityEngine;

public class FailPanel : Panel
{
    [SerializeField] private TextMeshProUGUI levelText;

    public override void UpdateVisual()
    {
        if (levelText == null)
        {
            return;
        }

        if (LevelManager.Instance != null)
        {
            levelText.text = LevelManager.Instance.LevelDisplayName;
            return;
        }

        if (DataManager.Instance != null)
        {
            levelText.text = DataManager.Instance.GetCurrentLevelDisplayName();
        }
    }

    public void PlayOn()
    {
    }

    public void Restart()
    {
        AudioManager.Instance?.StopCurrentSFX();
        LevelManager.Instance.DestroyLevel();
        Observer.Publish(ObserverEvent.PlayGame);
    }
}
