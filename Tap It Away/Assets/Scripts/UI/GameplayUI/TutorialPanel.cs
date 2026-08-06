using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : Panel
{
    [SerializeField] private Image unlockIcon;
    [SerializeField] private TMP_Text descriptionText;

    private BoosterType boosterType;

    public void Init(BoosterType boosterType)
    {
        this.boosterType = boosterType;

        BoosterSO boosterSO = BoosterManager.Instance != null
            ? BoosterManager.Instance.GetBoosterSO(boosterType)
            : null;

        if (unlockIcon != null)
        {
            unlockIcon.sprite = boosterSO != null ? boosterSO.icon : null;
            unlockIcon.gameObject.SetActive(unlockIcon.sprite != null);
        }

        if (descriptionText != null)
        {
            descriptionText.text = boosterSO != null ? boosterSO.description : string.Empty;
        }
    }

    public void Claim()
    {
        BoosterManager.Instance?.MarkBoosterTutorialShown(boosterType);
        Close();
    }
}
