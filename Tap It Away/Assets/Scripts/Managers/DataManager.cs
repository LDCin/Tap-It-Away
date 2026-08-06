using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DataManager : Singleton<DataManager>
{
    public static event System.Action OnSettingsChanged;
    public static event System.Action<int> OnCoinsChanged;

    private const string UserDataFileName = "user_data.json";

    [SerializeField] private TextAsset userDataFile;
    [SerializeField] private UserData userData = new();
    public UserData CurrentUserData => userData;
    public bool BgmEnabled => userData == null || userData.bgmEnabled;
    public bool SfxEnabled => userData == null || userData.sfxEnabled;
    public bool HapticEnabled => userData == null || userData.hapticEnabled;
    private string UserDataSavePath => Path.Combine(Application.persistentDataPath, UserDataFileName);

    public override void Awake()
    {
        base.Awake();
        if (Instance != this)
        {
            return;
        }

        LoadUserData();
    }
    public void LoadUserData()
    {
        if (File.Exists(UserDataSavePath))
        {
            userData = JsonConvert.DeserializeObject<UserData>(File.ReadAllText(UserDataSavePath));
            return;
        }

        if (userDataFile == null)
        {
            Debug.LogError("User data file is missing.");
            return;
        }

        userData = JsonConvert.DeserializeObject<UserData>(userDataFile.text);
    }

    public void SaveUserData()
    {
        if (userData == null)
        {
            Debug.LogWarning("Cannot save user data because it is not loaded.");
            return;
        }

        string json = SerializeUserData();

        File.WriteAllText(UserDataSavePath, json);
    }

    private string SerializeUserData()
    {
        return JsonConvert.SerializeObject(
            userData,
            Formatting.Indented,
            new StringEnumConverter()
        );
    }

#if UNITY_EDITOR
    [ContextMenu("Load Json To Inspector")]
    public void LoadJsonToInspector()
    {
        if (userDataFile == null)
        {
            Debug.LogError("User data file is missing.");
            return;
        }

        userData = JsonConvert.DeserializeObject<UserData>(userDataFile.text);
        EditorUtility.SetDirty(this);
    }

    [ContextMenu("Save Inspector To Json")]
    public void SaveInspectorToJson()
    {
        if (userDataFile == null)
        {
            Debug.LogError("User data file is missing.");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(userDataFile);
        if (string.IsNullOrEmpty(assetPath))
        {
            Debug.LogError("Cannot find user data asset path.");
            return;
        }

        string json = SerializeUserData();
        File.WriteAllText(assetPath, json);
        File.WriteAllText(UserDataSavePath, json);
        AssetDatabase.ImportAsset(assetPath);
        AssetDatabase.Refresh();
    }

    [ContextMenu("Delete Runtime Save Json")]
    public void DeleteRuntimeSaveJson()
    {
        if (!File.Exists(UserDataSavePath))
        {
            Debug.Log("Runtime user data save does not exist.");
            return;
        }

        File.Delete(UserDataSavePath);
        Debug.Log("Deleted runtime user data save: " + UserDataSavePath);
    }
#endif

    public void SetThemeType(GameThemeType themeType)
    {
        if (userData == null)
        {
            LoadUserData();
        }

        if (userData == null)
        {
            return;
        }

        userData.themeType = themeType;
        SaveUserData();
    }

    public void SetBgmEnabled(bool enabled)
    {
        if (!EnsureUserDataLoaded())
        {
            return;
        }

        userData.bgmEnabled = enabled;
        SaveUserData();
        OnSettingsChanged?.Invoke();
    }

    public void SetSfxEnabled(bool enabled)
    {
        if (!EnsureUserDataLoaded())
        {
            return;
        }

        userData.sfxEnabled = enabled;
        SaveUserData();
        OnSettingsChanged?.Invoke();
    }

    public void SetHapticEnabled(bool enabled)
    {
        if (!EnsureUserDataLoaded())
        {
            return;
        }

        userData.hapticEnabled = enabled;
        SaveUserData();
        OnSettingsChanged?.Invoke();
    }

    public void ToggleBgmEnabled()
    {
        SetBgmEnabled(!BgmEnabled);
    }

    public void ToggleSfxEnabled()
    {
        SetSfxEnabled(!SfxEnabled);
    }

    public void ToggleHapticEnabled()
    {
        SetHapticEnabled(!HapticEnabled);
    }

    private bool EnsureUserDataLoaded()
    {
        if (userData == null)
        {
            LoadUserData();
        }

        return userData != null;
    }

    public int GetCoins()
    {
        if (!EnsureUserDataLoaded())
        {
            return 0;
        }

        return userData.coins;
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0 || !EnsureUserDataLoaded())
        {
            return;
        }

        userData.coins += amount;
        SaveUserData();
        OnCoinsChanged?.Invoke(userData.coins);
    }

    public void AdvanceToNextLevel()
    {
        if (userData == null)
        {
            LoadUserData();
        }

        if (userData == null)
        {
            return;
        }

        userData.level++;
        SaveUserData();
    }

    public int GetCurrentLevelNumber()
    {
        if (userData == null)
        {
            LoadUserData();
        }

        if (userData == null)
        {
            Debug.LogError("User data is not loaded.");
            return 1;
        }

        return (userData.map - 1) * 10 + userData.level;
    }

    public string GetCurrentLevelDisplayName()
    {
        return $"Level {GetCurrentLevelNumber():00}";
    }

    public string GetCurrentLevelName()
    {
        if (userData == null)
        {
            LoadUserData();
        }

        if (userData == null)
        {
            Debug.LogError("User data is not loaded.");
            return string.Empty;
        }

        int mapNumber = userData.map;
        int levelNumber = userData.level;
        string fileName = $"Level {mapNumber}-{levelNumber}";
        Debug.Log("Current level: " + fileName);
        return fileName;
    }
}
