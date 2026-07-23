using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Cube cubePrefab;
    [SerializeField] private GameObject spawnRoot;
    private List<Cube> cubeList;
    private void Start()
    {
        cubeList = new();
    }
    [ContextMenu("Test Load Level From TA")]
    public LevelData LoadLevelFromTextAsset(TextAsset levelFile)
    {
        LevelData levelData = JsonConvert.DeserializeObject<LevelData>(levelFile.text);
        // Debug.Log(levelData.board.sizeX + " " + levelData.board.sizeY + " " + levelData.board.sizeZ);
        return levelData;
    }
    public string GetCurrentLevelNameFromData(TextAsset userDataFile)
    {
        UserData userData = JsonConvert.DeserializeObject<UserData>(userDataFile.text);
        int mapNumber = userData.map;
        int levelNumber = userData.level;
        string fileName = $"level_{mapNumber}-{levelNumber}";
        Debug.Log("Current level: " + fileName);
        return fileName;
    }
    public void SpawnLevel(TextAsset levelDataFile)
    {
        LevelData levelData = LoadLevelFromTextAsset(levelDataFile);
        foreach (var cube in levelData.cubes)
        {
            Cube newCube = Instantiate(cubePrefab, spawnRoot.transform);
            newCube.InitByCubeData(cube);
            newCube.transform.localPosition = cube.position;
        }
    }
    public async UniTask SpawnLevelFromJson(string jsonFileName)
    {
        AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(jsonFileName);
        await handle;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Addressables.Release(handle);
            Debug.Log("Fail: Load Level By File Name");
            return;
        }

        LevelData levelData = JsonConvert.DeserializeObject<LevelData>(handle.Result.text);
        foreach (var cube in levelData.cubes)
        {
            Cube newCube = Instantiate(cubePrefab, spawnRoot.transform);
            newCube.InitByCubeData(cube);
            cubeList.Add(newCube);
        }
        Addressables.Release(handle);
    }
    public async UniTask SpawnCurrentLevelFromUserData(TextAsset userData)
    {
        string jsonFilePath = GetCurrentLevelNameFromData(userData);
        await SpawnLevelFromJson(jsonFilePath);
    }
    public void DestroyLevel()
    {
        cubeList.Clear();
    }
    public int GetCubeCount()
    {
        return cubeList.Count;
    }
}
