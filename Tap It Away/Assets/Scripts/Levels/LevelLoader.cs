using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private CubePool cubePool;
    [SerializeField] private GameObject spawnRoot;
    private List<CubeMover> cubeList;
    [SerializeField] private Vector3 spawnRootInitialLocalPosition = Vector3.zero;
    [SerializeField] private Quaternion spawnRootInitialLocalRotation = Quaternion.identity;
    [SerializeField] private Vector3 spawnRootInitialLocalScale = Vector3.one;
    public List<CubeMover> CubeList => cubeList;
    private void Awake()
    {
        cubeList = new();
    }
    [ContextMenu("Test Load Level From Text Asset")]
    public LevelData LoadLevelFromTextAsset(TextAsset levelFile)
    {
        LevelData levelData = JsonConvert.DeserializeObject<LevelData>(levelFile.text);
        // Debug.Log(levelData.board.sizeX + " " + levelData.board.sizeY + " " + levelData.board.sizeZ);
        return levelData;
    }
    public void SpawnLevel(TextAsset levelDataFile)
    {
        DestroyLevel();
        ResetSpawnRootTransform();
        LevelData levelData = LoadLevelFromTextAsset(levelDataFile);
        foreach (var cube in levelData.cubes)
        {
            SpawnCube(cube);
        }
    }
    public async UniTask SpawnLevelFromJsonAsync(string jsonFileName)
    {
        DestroyLevel();
        ResetSpawnRootTransform();
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
            SpawnCube(cube);
        }
        Addressables.Release(handle);
    }
    public void DestroyLevel()
    {
        if (cubeList == null)
        {
            return;
        }

        foreach (CubeMover cubeMover in cubeList)
        {
            ReturnCube(cubeMover);
        }

        cubeList.Clear();
        ResetSpawnRootTransform();
    }
    public int GetCubeCount()
    {
        return cubeList.Count;
    }

    // private void CacheSpawnRootTransform()
    // {
    //     if (spawnRoot == null)
    //     {
    //         return;
    //     }

    //     Transform rootTransform = spawnRoot.transform;
    //     spawnRootInitialLocalPosition = rootTransform.localPosition;
    //     spawnRootInitialLocalRotation = rootTransform.localRotation;
    //     spawnRootInitialLocalScale = rootTransform.localScale;
    // }

    public void ResetSpawnRootTransform()
    {
        if (spawnRoot == null)
        {
            return;
        }

        Transform rootTransform = spawnRoot.transform;
        rootTransform.localPosition = spawnRootInitialLocalPosition;
        rootTransform.localRotation = spawnRootInitialLocalRotation;
        rootTransform.localScale = spawnRootInitialLocalScale;
    }

    private void SpawnCube(CubeData cubeData)
    {
        Cube newCube = cubePool.GetCube(cubeData, spawnRoot.transform);

        if (newCube == null)
        {
            return;
        }

        cubeList.Add(newCube.GetComponent<CubeMover>());
    }

    private void ReturnCube(CubeMover cubeMover)
    {
        if (cubeMover == null)
        {
            return;
        }

        Cube cube = cubeMover.GetComponent<Cube>();
        if (cube != null)
        {
            cubePool.ReturnCube(cube);
        }
    }
}
