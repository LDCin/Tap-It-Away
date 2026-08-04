using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class BoosterLoader
{
    public async UniTask<BoosterSO> LoadBoosterSOAsync(BoosterType boosterType)
    {
        var handle = Addressables.LoadAssetAsync<BoosterSO>(boosterType.ToString());
        await handle;
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Addressables.Release(handle);
            Debug.Log("Not Found Hint SO");
            return null;
        }
        BoosterSO boosterSO = handle.Result;
        return boosterSO;
    }
}