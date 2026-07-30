using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserData
{
    public int map;
    public int level;
    public int coins;
    public List<UserBoosterData> userBoosterDataList = new();
}

[Serializable]
public class UserBoosterData
{
    public BoosterType boosterType;
    public bool isUnlocked;
    public int count;

    public UserBoosterData(BoosterType boosterType, bool isUnlocked, int count)
    {
        this.boosterType = boosterType;
        this.isUnlocked = isUnlocked;
        this.count = count;
    }
}