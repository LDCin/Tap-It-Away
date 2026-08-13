using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuccessPanel : Panel
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image congratulationText;
    [SerializeField] private List<Sprite> congratulationTextList;
    public override void UpdateVisual()
    {
        int randomIdx = Random.Range(0, congratulationTextList.Count - 1);
        congratulationText.sprite = congratulationTextList[randomIdx];
        levelText.text = DataManager.Instance.GetCurrentLevelName();
    }
    public void NextLevel()
    {
        DataManager.Instance.AdvanceToNextLevel();
    }
}