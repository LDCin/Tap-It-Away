using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/Booster", fileName = "New Booster Data", order = 1)]
public class BoosterSO : ScriptableObject
{
    public BoosterType boosterType;
    public Sprite icon;
    public string description;
    public int activeCount;
    public int price;
}