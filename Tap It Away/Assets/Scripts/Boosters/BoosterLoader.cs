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
    public async UniTask<List<BoosterType>> LoadUnlockBooster()
    {
        List<BoosterType> unlockedBoosters = new();

        UserData userData = DataManager.Instance.CurrentUserData;
        if (userData == null)
        {
            DataManager.Instance.LoadUserData();
            userData = DataManager.Instance.CurrentUserData;
        }

        if (userData == null || userData.userBoosterDataList == null)
        {
            return unlockedBoosters;
        }

        foreach (UserBoosterData boosterData in userData.userBoosterDataList)
        {
            if (boosterData == null || !boosterData.isUnlocked)
            {
                continue;
            }

            BoosterSO boosterSO = await LoadBoosterSOAsync(boosterData.boosterType);
            if (boosterSO != null)
            {
                unlockedBoosters.Add(boosterSO.boosterType);
            }
        }

        return unlockedBoosters;
    }
}