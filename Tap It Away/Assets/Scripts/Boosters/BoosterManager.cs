using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BoosterManager : MonoBehaviour
{
    private BoosterLoader boosterLoader;
    private Dictionary<BoosterType, (BoosterBase, BoosterSO)> boosterDict;
    private async void Awake()
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