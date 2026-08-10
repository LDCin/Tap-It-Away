using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
public class LevelLoader : MonoBehaviour
{
    [SerializeField] private CubeVisual cubePrefab;
    [SerializeField] private GameObject spawnRoot;
    private List<CubeMover> cubeList;
    public List<CubeMover> CubeList => cubeList;
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
    public void SpawnLevel(TextAsset levelDataFile)
    {
        LevelData levelData = LoadLevelFromTextAsset(levelDataFile);
        foreach (var cube in levelData.cubes)
        {
            CubeVisual newCube = Instantiate(cubePrefab, spawnRoot.transform);
            newCube.InitByCubeData(cube, spawnRoot.transform);
            newCube.transform.localPosition = cube.position;
        }
    }
    public async UniTask SpawnLevelFromJsonAsync(string jsonFileName)
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
            CubeVisual newCube = Instantiate(cubePrefab, spawnRoot.transform);
            newCube.InitByCubeData(cube, spawnRoot.transform);
            cubeList.Add(newCube.gameObject.GetComponent<CubeMover>());
        }
        Addressables.Release(handle);
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
