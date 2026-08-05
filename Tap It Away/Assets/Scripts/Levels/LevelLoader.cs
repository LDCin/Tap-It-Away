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
    private List<CubeMover> cubeList;
    private Vector3 spawnRootInitialLocalPosition;
    private Quaternion spawnRootInitialLocalRotation;
    private Vector3 spawnRootInitialLocalScale;
    public List<CubeMover> CubeList => cubeList;
    private void Awake()
    {
        cubeList = new();
        CacheSpawnRootTransform();
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
        DestroyLevel();
        ResetSpawnRootTransform();
        LevelData levelData = LoadLevelFromTextAsset(levelDataFile);
        foreach (var cube in levelData.cubes)
        {
            Cube newCube = Instantiate(cubePrefab, spawnRoot.transform);
            newCube.InitByCubeData(cube);
            cubeList.Add(newCube.GetComponent<CubeMover>());
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
            Cube newCube = Instantiate(cubePrefab, spawnRoot.transform);
            newCube.InitByCubeData(cube);
            cubeList.Add(newCube.gameObject.GetComponent<CubeMover>());
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
            if (cubeMover != null)
            {
                Destroy(cubeMover.gameObject);
            }
        }

        cubeList.Clear();
        DestroyRemainingSpawnRootChildren();
        ResetSpawnRootTransform();
    }
    public int GetCubeCount()
    {
        return cubeList.Count;
    }

    private void CacheSpawnRootTransform()
    {
        if (spawnRoot == null)
        {
            return;
        }

        Transform rootTransform = spawnRoot.transform;
        spawnRootInitialLocalPosition = rootTransform.localPosition;
        spawnRootInitialLocalRotation = rootTransform.localRotation;
        spawnRootInitialLocalScale = rootTransform.localScale;
    }

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

    private void DestroyRemainingSpawnRootChildren()
    {
        if (spawnRoot == null)
        {
            return;
        }

        Transform rootTransform = spawnRoot.transform;
        for (int i = rootTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(rootTransform.GetChild(i).gameObject);
        }
    }
}
