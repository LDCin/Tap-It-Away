using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BoosterManager : Singleton<BoosterManager>
{
    public static event Action<BoosterType, int> OnBoosterCountChanged;

    private BoosterLoader boosterLoader;
    private Dictionary<BoosterType, (BoosterBase, BoosterSO)> boosterDict;
    private List<BoosterType> unlockedBoosters = new();
    public List<BoosterType> UnlockedBoosters => unlockedBoosters;
    public bool IsInitialized { get; private set; }

    public override async void Awake()
    {
        base.Awake();
        await InitializeBooster();
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
        UserBoosterData boosterData = GetUserBoosterData(boosterType);
        return boosterData != null && boosterData.isUnlocked;
    }

    private void OnEnable()
    {
        GameplayPanel.OnBoosterActive += HandleBoosterActive;
    }

    private void OnDisable()
    {
        GameplayPanel.OnBoosterActive -= HandleBoosterActive;
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
            OnBoosterCountChanged?.Invoke(boosterType, 0);
            return false;
        }

        boosterData.count--;
        OnBoosterCountChanged?.Invoke(boosterType, boosterData.count);
        return true;
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
