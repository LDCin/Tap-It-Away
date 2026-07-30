using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BoosterController : MonoBehaviour
{
    private Dictionary<BoosterType, BoosterBase> boosterDict;
    private async UniTask Awake()
    {
        
        boosterDict = new Dictionary<BoosterType, BoosterBase>
        {
            {BoosterType.Hint, new HintBooster()},
            {BoosterType.GhostCube, new GhostCubeBooster()}
        };
    }
    private void OnEnable()
    {

    }
    private void ActiveBooster(BoosterType boosterType)
    {
        if (!boosterDict.TryGetValue(boosterType, out BoosterBase booster))
        {
            Debug.LogWarning($"Booster not found: {boosterType}");
            return;
        }

        booster.Active();
    }
    [ContextMenu("Test Hint Booster")]
    private void ActiveHintBooster()
    {
        ActiveBooster(BoosterType.Hint);
    }
    [ContextMenu("Test Ghost Cube Booster")]
    private void ActiveGhostCubeBooster()
    {
        ActiveBooster(BoosterType.GhostCube);
    }
}