using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
    private void OnEnable()
    {

    }
    private async UniTask ActiveBooster(BoosterType boosterType)
    {
        if (!boosterDict.TryGetValue(boosterType, out var boosterValue))
        {
            Debug.LogWarning($"Booster not found: {boosterType}");
            return;
        }
        BoosterBase booster = boosterValue.Item1;
        await booster.StartBooster();
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
