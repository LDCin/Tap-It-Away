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
    public UserSettingData userSettingData = new();
    public bool BGMEnabled
    {
        get
        {
            return userSettingData.bgmEnabled;
        }
        set
        {
            userSettingData.bgmEnabled = value;
        }
    }
    public bool SFXEnabled
    {
        get
        {
            return userSettingData.sfxEnabled;
        }
        set
        {
            userSettingData.sfxEnabled = value;
        }
    }
    public bool HapticEnabled
    {
        get
        {
            return userSettingData.hapticEnabled;
        }
        set
        {
            userSettingData.hapticEnabled = value;
        }
    }
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

[Serializable]
public class UserSettingData
{
    public bool bgmEnabled;
    public bool sfxEnabled;
    public bool hapticEnabled;
}