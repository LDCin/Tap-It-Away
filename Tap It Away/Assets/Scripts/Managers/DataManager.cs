using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DataManager : Singleton<DataManager>
{
    [SerializeField] private TextAsset userDataFile;
    public UserData CurrentUserData { get; private set; }
    public override void Awake()
    {
        base.Awake();
        LoadUserData();
    }
    public void LoadUserData()
    {
        if (userDataFile == null)
        {
            Debug.LogError("User data file is missing.");
            return;
        }

        CurrentUserData = JsonConvert.DeserializeObject<UserData>(userDataFile.text);
    }
    public string GetCurrentLevelName()
    {
        if (CurrentUserData == null)
        {
            LoadUserData();
        }

        if (CurrentUserData == null)
        {
            Debug.LogError("User data is not loaded.");
            return string.Empty;
        }

        int mapNumber = CurrentUserData.map;
        int levelNumber = CurrentUserData.level;
        string fileName = $"Level {mapNumber}-{levelNumber}";
        Debug.Log("Current level: " + fileName);
        return fileName;
    }
}
