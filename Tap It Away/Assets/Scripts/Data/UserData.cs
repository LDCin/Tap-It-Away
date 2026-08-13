using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserData
{
    [Min(0)] public int map;
    [Min(1)] public int level;
    [Min(0)] public int coins;
    public List<UserBoosterData> userBoosterDataList = new();
    public UserSettingData userSettingData = new();

    public int GetMap()
    {
        return map;
    }

    public void SetMap(int map)
    {
        this.map = map;
    }

    public int GetLevel()
    {
        return level;
    }

    public void SetLevel(int level)
    {
        this.level = level;
    }

    public void AdvanceLevel()
    {
        level++;
    }

    public int GetCoins()
    {
        return coins;
    }

    public void SetCoins(int coins)
    {
        this.coins = coins;
    }

    public bool GetBgmEnabled()
    {
        return userSettingData.bgmEnabled;
    }

    public void SetBgmEnabled(bool enabled)
    {
        userSettingData.bgmEnabled = enabled;
    }

    public bool GetSfxEnabled()
    {
        return userSettingData.sfxEnabled;
    }

    public void SetSfxEnabled(bool enabled)
    {
        userSettingData.sfxEnabled = enabled;
    }

    public bool GetHapticEnabled()
    {
        return userSettingData.hapticEnabled;
    }

    public void SetHapticEnabled(bool enabled)
    {
        userSettingData.hapticEnabled = enabled;
    }

    public bool BGMEnabled
    {
        get
        {
            return GetBgmEnabled();
        }
        set
        {
            SetBgmEnabled(value);
        }
    }
    public bool SFXEnabled
    {
        get
        {
            return GetSfxEnabled();
        }
        set
        {
            SetSfxEnabled(value);
        }
    }
    public bool HapticEnabled
    {
        get
        {
            return GetHapticEnabled();
        }
        set
        {
            SetHapticEnabled(value);
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
