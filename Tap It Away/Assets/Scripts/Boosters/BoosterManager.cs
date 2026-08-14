using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BoosterManager : Singleton<BoosterManager>
{
    private BoosterLoader boosterLoader;
    private Dictionary<BoosterType, (BoosterBase, BoosterSO)> boosterDict;
    private List<BoosterType> unlockedBoosters = new();
    public List<BoosterType> UnlockedBoosters => unlockedBoosters;
    public bool IsInitialized { get; private set; }

    public override async void Awake()
    {
        base.Awake();
        await InitializeBooster();
        SyncUnlocksForCurrentLevel();
        await InitializeUnlockBoosterList();
        IsInitialized = true;
    }

    public async UniTask WaitUntilInitialized()
    {
        await UniTask.WaitUntil(() => IsInitialized);
    }

    private async UniTask InitializeBooster()
    {
        boosterLoader = new();
        BoosterSO hintBoosterSO = await boosterLoader.LoadBoosterSOAsync(BoosterType.Hint);
        BoosterSO ghostCubeBoosterSO = await boosterLoader.LoadBoosterSOAsync(BoosterType.GhostCube);
        boosterDict = new Dictionary<BoosterType, (BoosterBase, BoosterSO)>
        {
            {BoosterType.Hint, (new HintBooster(hintBoosterSO), hintBoosterSO)},
            {BoosterType.GhostCube, (new GhostCubeBooster(ghostCubeBoosterSO), ghostCubeBoosterSO)}
        };
    }
    private async UniTask InitializeUnlockBoosterList()
    {
        unlockedBoosters = new();
        unlockedBoosters = await boosterLoader.LoadUnlockBooster();
    }
    public BoosterSO GetBoosterSO(BoosterType boosterType)
    {
        if (!boosterDict.TryGetValue(boosterType, out var boosterValue))
        {
            Debug.LogWarning($"Booster not found: {boosterType}");
            return null;
        }
        BoosterSO boosterSO = boosterValue.Item2;
        return boosterSO;
    }

    public int GetBoosterCount(BoosterType boosterType)
    {
        UserBoosterData boosterData = GetUserBoosterData(boosterType);
        return boosterData != null ? boosterData.count : 0;
    }

    public bool IsBoosterUnlocked(BoosterType boosterType)
    {
        SyncUnlocksForCurrentLevel();
        UserBoosterData boosterData = GetUserBoosterData(boosterType);
        return boosterData != null && boosterData.isUnlocked;
    }

    private void OnEnable()
    {
        Observer.Subscribe<BoosterType>(ObserverEvent.BoosterActive, HandleBoosterActive);
    }

    private void OnDisable()
    {
        Observer.Unsubscribe<BoosterType>(ObserverEvent.BoosterActive, HandleBoosterActive);
    }

    private async void HandleBoosterActive(BoosterType boosterType)
    {
        await WaitUntilInitialized();
        await ActiveBooster(boosterType);
    }

    private async UniTask ActiveBooster(BoosterType boosterType)
    {
        if (!boosterDict.TryGetValue(boosterType, out var boosterValue))
        {
            Debug.LogWarning($"Booster not found: {boosterType}");
            return;
        }

        if (!IsBoosterUnlocked(boosterType))
        {
            Debug.Log($"Booster is locked: {boosterType}");
            return;
        }

        if (!TryUseBooster(boosterType))
        {
            Debug.Log($"No booster left: {boosterType}");
            return;
        }

        BoosterBase booster = boosterValue.Item1;
        await booster.StartBooster();
    }

    private bool TryUseBooster(BoosterType boosterType)
    {
        UserBoosterData boosterData = GetUserBoosterData(boosterType);
        if (boosterData == null || boosterData.count <= 0)
        {
            Observer.Publish(ObserverEvent.BoosterCountChanged, boosterType);
            return false;
        }

        boosterData.count--;
        DataManager.Instance.SaveUserData();
        Observer.Publish(ObserverEvent.BoosterCountChanged, boosterType);
        return true;
    }

    public void SyncUnlocksForCurrentLevel()
    {
        if (boosterDict == null || DataManager.Instance == null)
        {
            return;
        }

        UserData userData = DataManager.Instance.CurrentUserData;
        if (userData == null)
        {
            DataManager.Instance.LoadUserData();
            userData = DataManager.Instance.CurrentUserData;
        }

        if (userData == null)
        {
            return;
        }

        bool changed = false;

        if (userData.userBoosterDataList == null)
        {
            userData.userBoosterDataList = new List<UserBoosterData>();
            changed = true;
        }

        int currentLevel = DataManager.Instance.GetCurrentLevelNumber();

        foreach (var boosterPair in boosterDict)
        {
            BoosterType boosterType = boosterPair.Key;
            BoosterSO boosterSO = boosterPair.Value.Item2;
            if (boosterSO == null)
            {
                continue;
            }

            UserBoosterData boosterData = GetOrCreateUserBoosterData(userData, boosterType, ref changed);
            bool shouldUnlock = currentLevel >= boosterSO.unlockLevel;

            if (!boosterData.isUnlocked && shouldUnlock)
            {
                boosterData.isUnlocked = true;
                boosterData.count += boosterSO.initialCount;
                changed = true;
            }

            if (boosterData.isUnlocked && !unlockedBoosters.Contains(boosterType))
            {
                unlockedBoosters.Add(boosterType);
            }
        }

        if (changed)
        {
            DataManager.Instance.SaveUserData();
        }
    }

    public bool TryGetUnshownUnlockedBooster(out BoosterType boosterType)
    {
        boosterType = default;

        if (boosterDict == null)
        {
            return false;
        }

        foreach (BoosterType currentBoosterType in boosterDict.Keys)
        {
            UserBoosterData boosterData = GetUserBoosterData(currentBoosterType);
            if (boosterData != null && boosterData.isUnlocked && !boosterData.tutorialShown)
            {
                boosterType = currentBoosterType;
                return true;
            }
        }

        return false;
    }

    public void MarkBoosterTutorialShown(BoosterType boosterType)
    {
        UserBoosterData boosterData = GetUserBoosterData(boosterType);
        if (boosterData == null || boosterData.tutorialShown)
        {
            return;
        }

        boosterData.tutorialShown = true;
        DataManager.Instance.SaveUserData();
    }

    private UserBoosterData GetOrCreateUserBoosterData(UserData userData, BoosterType boosterType, ref bool changed)
    {
        foreach (UserBoosterData boosterData in userData.userBoosterDataList)
        {
            if (boosterData != null && boosterData.boosterType == boosterType)
            {
                return boosterData;
            }
        }

        UserBoosterData newBoosterData = new(boosterType, false, 0);
        userData.userBoosterDataList.Add(newBoosterData);
        changed = true;
        return newBoosterData;
    }

    private UserBoosterData GetUserBoosterData(BoosterType boosterType)
    {
        UserData userData = DataManager.Instance.CurrentUserData;
        if (userData == null)
        {
            DataManager.Instance.LoadUserData();
            userData = DataManager.Instance.CurrentUserData;
        }

        if (userData == null || userData.userBoosterDataList == null)
        {
            return null;
        }

        foreach (UserBoosterData boosterData in userData.userBoosterDataList)
        {
            if (boosterData != null && boosterData.boosterType == boosterType)
            {
                return boosterData;
            }
        }

        return null;
    }
    [ContextMenu("Test Hint Booster")]
    private async UniTask ActiveHintBooster()
    {
        await ActiveBooster(BoosterType.Hint);
    }
    [ContextMenu("Test Ghost Cube Booster")]
    private async UniTask ActiveGhostCubeBooster()
    {
        await ActiveBooster(BoosterType.GhostCube);
    }
}
